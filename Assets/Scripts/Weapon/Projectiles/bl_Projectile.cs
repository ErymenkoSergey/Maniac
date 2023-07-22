using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(AudioSource))]
public class bl_Projectile : MonoBehaviour
{
    public ProjectileType m_Type = ProjectileType.Grenade;
    public bool OnHit = false;
    public int TimeToExploit = 10;
    public int ID { get; set; }
    public string mName { get; set; }
    [HideInInspector]
    public bool isNetwork = false;
    private float speed = 75.0f;              // bullet speed
    private Vector3 velocity = Vector3.zero; // bullet velocity    
    private Vector3 direction;               // direction bullet is travelling
    private float impactForce;        // force applied to a rigid body object
    public bool AIFrom { get; set; }
    public int AIViewID { get; set; }
    private Team AITeam;
    private string BotName;
    public GameObject explosion;   // instanced explosion
    private float damage;

    public void SetUp(BulletData s)
    {
        damage = s.Damage;
        impactForce = s.ImpactForce;
        speed = s.Speed;
        ID = s.WeaponID;
        mName = s.WeaponName;
        isNetwork = s.isNetwork;
        direction = transform.TransformDirection(0, 0, 1);

        velocity = speed * transform.forward;

        GetComponent<Rigidbody>().velocity = velocity + direction;
        if (!OnHit)
        {
            InvokeRepeating("Counter", 1, 1);
        }
    }

    public void AISetUp(int viewID, Team team, string botName)
    {
        AIFrom = true;
        AIViewID = viewID;
        AITeam = team;
        BotName = botName;
    }

    private void OnCollisionEnter(Collision enterObject)
    {
        if (!OnHit)
            return;

        switch (enterObject.transform.tag)
        {
            case "Projectile":            
                break;

            default:
                Destroy(gameObject, 0);//GetComponent<Rigidbody>().useGravity = false;
                ContactPoint contact = enterObject.contacts[0];
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, contact.normal);

                GameObject e = Instantiate(explosion, contact.point, rotation) as GameObject;
                if (m_Type == ProjectileType.Grenade)
                {
                    bl_ExplosionDamage blast = e.GetComponent<bl_ExplosionDamage>();
                    if (blast != null)
                    {
                        blast.isNetwork = isNetwork;
                        blast.WeaponID = ID;
                        blast.explosionDamage = damage;
                    }
                }
                else if (m_Type == ProjectileType.Molotov)
                {
                    bl_DamageArea da = e.GetComponent<bl_DamageArea>();
                    da.SetInfo(AIFrom ? BotName : PhotonNetwork.NickName, isNetwork);
                }
                if (enterObject.rigidbody)
                {
                    enterObject.rigidbody.AddForce(transform.forward * impactForce, ForceMode.Impulse);
                }
                else if (m_Type == ProjectileType.Jumper)
                {

                }

                break;
        }
    }

    private void FixedUpdate()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().AddTorque(Vector3.up * 12);
        }
    }

    private void Counter()
    {
        if (m_Type == ProjectileType.Jumper)
            return;

        TimeToExploit--;

        if (TimeToExploit <= 0)
        {
            GameObject e = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            if (m_Type == ProjectileType.Grenade)
            {
                bl_ExplosionDamage blast = e.GetComponent<bl_ExplosionDamage>();
                if (blast != null)
                {
                    blast.isNetwork = isNetwork;
                    blast.WeaponID = ID;
                    blast.explosionDamage = damage;
                    blast.ActorViewID = AIFrom ? AIViewID : bl_GameManager.LocalPlayerViewID;
                    Team t = (AIFrom) ? AITeam : PhotonNetwork.LocalPlayer.GetPlayerTeam();
                    if (AIFrom) { blast.AISetUp(AIViewID, t, BotName); }
                }
            }
            else if (m_Type == ProjectileType.Molotov)
            {

            }
            CancelInvoke("Counter");
            Destroy(gameObject);
        }
    }

    [Serializable]
    public enum ProjectileType
    {
        Grenade,
        Molotov,
        Smoke,
        Jumper,
    }
}