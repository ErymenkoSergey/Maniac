using System.Collections;
using UnityEngine;

public class InteractivePlace : MonoBehaviour
{
    [SerializeField] private AudioClip _teleportSound;
    [SerializeField] private bool _isTeleporting;
    private Transform _spawnPoint;

    public void SetNewPlace(Transform transform)
    {
        Debug.Log($"OnTriggerStay SetNewPlace 0 ");
        _spawnPoint = transform;
        _isTeleporting = true;
        StartCoroutine(Rollback());
    }

    private IEnumerator Rollback()
    {
        yield return new WaitForSeconds(3f);
        _isTeleporting = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isTeleporting)
            return;

        //if (_teleportSound != null)
        //{
        //    Debug.Log("OnTriggerStay _teleportSound");
        //    AudioSource.PlayClipAtPoint(_teleportSound, transform.position);
        //}

        if (other.CompareTag(bl_PlayerSettings.LocalTag))
        {
            bl_FirstPersonController fpc = other.GetComponent<bl_FirstPersonController>();

            //if (_isRandomPoint)
            //{
            //    var randomPoint = Random.Range(0, _spawnPoints.Length);
            //    fpc.SetPosition(_spawnPoints[randomPoint]);
            //}
            //else
            {
                fpc.SetPosition(_spawnPoint);
            }

            //_source = GetComponent<AudioSource>();
            //_source.PlayOneShot(_teleportSound, 0.5f);
            if (_teleportSound != null)
                AudioSource.PlayClipAtPoint(_teleportSound, transform.position);
        }

        //if (other.CompareTag(bl_PlayerSettings.RemoteTag))
        //{
        //    var fpc = other.GetComponent<bl_PlayerNetwork>();
        //    Debug.Log($"OnTriggerStay {other.gameObject.name} 3 ");
        //    fpc.SetNewPosition(_spawnPoint);


        //}
        //if (other.CompareTag(bl_PlayerSettings.LocalTag))
        //{
        //    var fpc = other.GetComponent<bl_PlayerNetwork>();
        //    Debug.Log($"OnTriggerStay {other.gameObject.name} 3.1 ");
        //    fpc.SetNewPosition(_spawnPoint);

        //    //if (_teleportSound != null)
        //    //{
        //    //    Debug.Log("OnTriggerStay _teleportSound");
        //    //    AudioSource.PlayClipAtPoint(_teleportSound, transform.position);
        //    //}
        //}
    }
}
