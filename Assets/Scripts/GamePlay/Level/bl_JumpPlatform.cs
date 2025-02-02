﻿using UnityEngine;

namespace MFPS.Runtime.Level
{
    [RequireComponent(typeof(AudioSource))]
    public class bl_JumpPlatform : MonoBehaviour
    {
        [Range(0, 125)] public float JumpForce;
        [SerializeField] private AudioClip JumpSound;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(bl_PlayerSettings.LocalTag))
            {
                bl_FirstPersonController fpc = other.GetComponent<bl_FirstPersonController>();
                fpc.PlatformJump(JumpForce);
                if (JumpSound != null) { AudioSource.PlayClipAtPoint(JumpSound, transform.position); }
            }
        }
    }
}