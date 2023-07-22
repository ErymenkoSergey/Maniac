using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static bl_Projectile;

[RequireComponent(typeof(AudioSource))]
public class bl_JumpMine : MonoBehaviour
{
    public ProjectileType m_Type = ProjectileType.Jumper;

    [Range(0, 125)] public float JumpForce;
    [SerializeField] private AudioClip JumpSound;
    public GameObject explosion;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bl_PlayerSettings.LocalTag))
        {
            bl_FirstPersonController fpc = other.GetComponent<bl_FirstPersonController>();
            fpc.PlatformJump(JumpForce);
            if (JumpSound != null) { AudioSource.PlayClipAtPoint(JumpSound, transform.position); }
            if (explosion != null) { Instantiate(explosion, transform); }

            DestroyMine();
        }
    }

    private void DestroyMine()
    {
        Destroy(gameObject);
    }
    
}