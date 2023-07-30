using System.Collections;
using UnityEngine;

public class CheckedManiac : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private float _volume;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag(bl_PlayerSettings.LocalTag))
        if (other.GetComponent<bl_PlayerSettings>().PlayerTeam == Team.Maniac)
        {
            AudioSignal();
        }
    }

    private void AudioSignal()
    {
        _audioSource.PlayOneShot(_clip, _volume);
        StartCoroutine(Destroyng());
    }

    private IEnumerator Destroyng()
    {
        yield return new WaitForSeconds(_clip.length);
        Destroy(gameObject);
    }
}
