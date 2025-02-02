﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using MFPS.Runtime.AI;

public class bl_AIShooterWeapon : bl_PhotonHelper
{
    [Header("Settings")]
    public bool ForceFireWhenTargetClose = true;
    [Range(0, 5)] public int Grenades = 3;
    [SerializeField, Range(10, 100)] private float GrenadeSpeed = 50;
    [Range(10, 100)] public float MinumumDistanceForGranades = 20;

    [Header("Weapons")]
    public List<bl_AIWeapon> aiWeapons = new List<bl_AIWeapon>();

    [Header("References")]
    [SerializeField] private GameObject Grenade = null;
    [SerializeField] private Transform GrenadeFirePoint = null;
    [SerializeField] private AudioSource FireSource = null;

    private int bullets;
    private bool canFire = true;
    private float attackTime;
    private int FollowingShoots = 0;
    public bool isFiring { get; set; }
    private bl_AIShooterReferences AiReferences;
    private Animator Anim;
    private GameObject bullet;
    private bl_ObjectPooling Pooling;
    private bl_AIWeapon Weapon;
    private int WeaponID = -1;
    private BulletData m_BulletData = new BulletData();
    private bl_AIShooterAgent AI;
#if UMM
    private bl_MiniMapItem miniMapItem;
#endif

    private void Awake()
    {
        AiReferences = GetComponent<bl_AIShooterReferences>();
        AI = GetComponent<bl_AIShooterAgent>();
        Anim = GetComponentInChildren<Animator>();
        Pooling = bl_ObjectPooling.Instance;
        bl_PhotonCallbacks.PlayerEnteredRoom += OnPhotonPlayerConnected;
#if UMM
        miniMapItem = GetComponent<bl_MiniMapItem>();
#endif
    }

    void Start()
    {
        attackTime = Time.time;
        if (PhotonNetwork.IsMasterClient)
        {
            WeaponID = Random.Range(0, aiWeapons.Count);
            photonView.RPC(nameof(SyncAIWeapon), RpcTarget.All, WeaponID);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        bl_PhotonCallbacks.PlayerEnteredRoom -= OnPhotonPlayerConnected;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Fire(FireReason fireReason = FireReason.Normal)
    {
        if (!canFire || AI.ObstacleBetweenTarget)
            return;
        if (ForceFireWhenTargetClose && AI.CachedTargetDistance < 10) { fireReason = FireReason.Forced; }
        if (fireReason == FireReason.Normal)
        {
            if (!AI.playerInFront)
                return;
        }
        if (Weapon == null) return;
        if (AI.Target != null && AI.Target.name.Contains("(die)"))
        {
            AI.KillTheTarget();
            return;
        }
        if (attackTime > Time.time) return;
       
        if (Grenades > 0 && AI.TargetDistance >= MinumumDistanceForGranades && FollowingShoots > 5)
        {
            if ((Random.Range(0, 200) > 165))
            {
                StartCoroutine(ThrowGrenade(false, Vector3.zero, Vector3.zero));
                attackTime = Time.time + 3.3f;
                return;
            }
        }

        Anim.Play($"Fire{Weapon.Info.Type.ToString()}", 1, 0);
        attackTime = (fireReason == FireReason.OnMove) ? Time.time + Random.Range(Weapon.Info.FireRate * 2, Weapon.Info.FireRate * 5) : Time.time + Weapon.Info.FireRate;
        switch (Weapon.Info.Type)
        {
            case GunType.Shotgun:
                int bulletCounter = 0;
                do
                {
                    FireSingleProjectile(FirePoint.position, AI.TargetPosition, false);
                    bulletCounter++;
                } while (bulletCounter < Weapon.bulletsPerShot);
                break;
            default:
                FireSingleProjectile(FirePoint.position, AI.TargetPosition, false);
                break;
        }
        PlayFireEffects();
        bullets--;
        FollowingShoots++;
        photonView.RPC(nameof(RpcAIFire), RpcTarget.Others, FirePoint.position, AI.TargetPosition);

        if (bullets <= 0)
        {
            canFire = false;
            StartCoroutine(Reload());
        }
        else
        {
            if (FollowingShoots > GetMaxFollowingShots())
            {
                if (Random.Range(0, 15) > 12)
                {
                    attackTime += Random.Range(0.01f, 5);
                    FollowingShoots = 0;
                }
            }
        }
        isFiring = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void FireSingleProjectile(Vector3 origin, Vector3 direction, bool remote)
    {
        if (Weapon == null) return;

        bullet = Pooling.Instantiate(Weapon.BulletName, origin, transform.root.rotation);
        bullet.transform.LookAt(direction);
        //build bullet data
        m_BulletData.Damage = Weapon.Info.Damage;
        m_BulletData.isNetwork = remote;
        m_BulletData.Position = transform.position;
        m_BulletData.WeaponID = Weapon.GunID;
        if (AI.behaviorSettings.weaponAccuracy == AIWeaponAccuracy.Pro)
        {
            m_BulletData.Spread = 2f;
            m_BulletData.MaxSpread = 3f;
        }
        else if (AI.behaviorSettings.weaponAccuracy == AIWeaponAccuracy.Casual)
        {
            m_BulletData.Spread = 3f;
            m_BulletData.MaxSpread = 6f;
        }
        else
        {
            m_BulletData.Spread = 5f;
            m_BulletData.MaxSpread = 10f;
        }
        m_BulletData.Speed = 300;
        m_BulletData.Range = Weapon.Info.Range;
        m_BulletData.WeaponName = Weapon.Info.Name;
        m_BulletData.ActorViewID = photonView.ViewID;
        m_BulletData.MFPSActor = AI.BotMFPSActor;
        bullet.GetComponent<bl_Bullet>().SetUp(m_BulletData);
        bullet.GetComponent<bl_Bullet>().AISetUp(AI.AIName, photonView.ViewID, AI.AITeam);
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlayFireEffects()
    {
        if (FireSource.enabled)
        {
            FireSource.pitch = Random.Range(0.85f, 1.1f);
            FireSource.clip = Weapon.fireSound;
            FireSource.Play();
        }
        if (Weapon.MuzzleFlash != null) { Weapon.MuzzleFlash.Play(); }
#if UMM
        ShowMiniMapItem();
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnDeath()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 
    /// </summary>
    IEnumerator ThrowGrenade(bool network, Vector3 velocity, Vector3 forward)
    {
        Anim.SetInteger("UpperState", 2);
        Anim.Play("FireGrenade", 1, 0);
        attackTime = Time.time + Weapon.Info.FireRate;
        yield return new WaitForSeconds(0.2f);
        GameObject bullet = Instantiate(Grenade, GrenadeFirePoint.position, transform.root.rotation) as GameObject;

        m_BulletData.Damage = 100;
        m_BulletData.isNetwork = network;
        m_BulletData.Position = transform.position;
        m_BulletData.WeaponID = 3;
        m_BulletData.Spread = 2f;
        m_BulletData.MaxSpread = 3f;
        m_BulletData.Speed = GrenadeSpeed;
        m_BulletData.Range = 5;
        m_BulletData.WeaponName = "Grenade";
        bullet.GetComponent<bl_Projectile>().SetUp(m_BulletData);
        bullet.GetComponent<bl_Projectile>().AISetUp(photonView.ViewID, AI.AITeam, AI.AIName);
        if (!network)
        {
            Rigidbody r = bullet.GetComponent<Rigidbody>();
            velocity = GetVelocity(AI.TargetPosition);
            r.velocity = velocity;
            r.AddRelativeTorque(Vector3.right * -5500.0f);
            forward = AI.TargetPosition - r.transform.position;
            r.transform.forward = forward;
            Grenades--;
            photonView.RPC(nameof(FireGrenadeRPC), RpcTarget.Others, velocity, forward);
        }
        else
        {
            Rigidbody r = bullet.GetComponent<Rigidbody>();
            r.velocity = velocity;
            r.AddRelativeTorque(Vector3.right * -5500.0f);
            r.transform.forward = forward;
        }
#if UMM
        ShowMiniMapItem();
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Reload()
    {
        photonView.RPC(nameof(RpcReload), RpcTarget.Others);
        yield return new WaitForSeconds(0.25f);
        Anim.SetInteger("UpperState", 2);
        yield return StartCoroutine(PlayReloadSound());
        Anim.SetInteger("UpperState", 0);
        bullets = Weapon.Bullets;
        canFire = true;
    }

    [PunRPC]
    void FireGrenadeRPC(Vector3 velocity, Vector3 forward)
    {
        StartCoroutine(ThrowGrenade(true, velocity, forward));
    }

    [PunRPC]
    void RpcAIFire(Vector3 pos, Vector3 look)
    {
        if (Weapon == null) return;
        Anim.Play($"Fire{Weapon.Info.Type.ToString()}", 1, 0);      
        switch (Weapon.Info.Type)
        {
            case GunType.Shotgun:
                int bulletCounter = 0;
                do
                {
                    FireSingleProjectile(pos, look, true);
                    bulletCounter++;
                } while (bulletCounter < Weapon.bulletsPerShot);
                break;
            default:
                FireSingleProjectile(pos, look, true);
                break;
        }
        PlayFireEffects();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    [PunRPC]
    void SyncAIWeapon(int ID)
    {
        WeaponID = ID;
        Weapon = aiWeapons[ID];
        foreach (bl_AIWeapon item in aiWeapons)
        {
            item.gameObject.SetActive(false);
        }
        bullets = Weapon.Bullets;
        Weapon.gameObject.SetActive(true);
        Weapon.Initialize(this);
        Anim.SetInteger("GunType", (int)Weapon.Info.Type);
    }

    [PunRPC]
    IEnumerator RpcReload()
    {
        Anim.SetInteger("UpperState", 2);
        yield return StartCoroutine(PlayReloadSound());
        Anim.SetInteger("UpperState", 0);
    }

    /// <summary>
    /// 
    /// </summary>
    IEnumerator PlayReloadSound()
    {
        for (int i = 0; i < Weapon.reloadSounds.Length; i++)
        {
            FireSource.clip = Weapon.reloadSounds[i];
            FireSource.Play();
            yield return new WaitForSeconds(0.5f);
        }
    }

#if UMM
    void ShowMiniMapItem()
    {
        if (AI.isTeamMate) return;
#if KSA
        if (bl_KillStreakHandler.Instance.activeAUVs > 0) return;
#endif
        if (miniMapItem != null && !miniMapItem.isTeamMateBot())
        {
            CancelInvoke(nameof(HideMiniMapItem));
            miniMapItem.ShowItem();
            Invoke(nameof(HideMiniMapItem), 0.25f);
        }
    }
    void HideMiniMapItem()
    {
        if (AI.isTeamMate) return;
        if (miniMapItem != null)
        {
            miniMapItem.HideItem();
        }
    }
#endif

    public void OnPhotonPlayerConnected(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && WeaponID != -1)
        {
            photonView.RPC(nameof(SyncAIWeapon), newPlayer, WeaponID);
        }
    }

    private Vector3 GetVelocity(Vector3 target)
    {
        Vector3 velocity;
        Vector3 toTarget = target - transform.position;
        float speed = 15;
        // Set up the terms we need to solve the quadratic equations.
        float gSquared = Physics.gravity.sqrMagnitude;
        float b = speed * speed + Vector3.Dot(toTarget, Physics.gravity);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

        // Check whether the target is reachable at max speed or less.
        if (discriminant < 0)
        {
            velocity = toTarget;
            velocity.y = 0;
            velocity.Normalize();
            velocity.y = 0.7f;

            Debug.DrawRay(transform.position, velocity * 3.0f, Color.blue);

            velocity *= speed;
            return velocity;
        }

        float discRoot = Mathf.Sqrt(discriminant);

        // Highest shot with the given max speed:
        float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        float T = 0;
        T = T_max;


        // Convert from time-to-hit to a launch velocity:
        velocity = toTarget / T - Physics.gravity * T / 2f;

        return velocity;
    }

    private int GetMaxFollowingShots()
    {
        return Weapon.maxFollowingShots;
    }

    public Transform FirePoint
    {
        get
        {
            if (Weapon != null)
            {
                return Weapon.FirePoint;
            }
            return transform;
        }
    }

    public enum FireReason
    {
        Normal,
        OnMove,
        Forced,
    }
}