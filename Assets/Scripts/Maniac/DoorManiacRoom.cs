using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class DoorManiacRoom : MonoBehaviour
{
    [SerializeField] private GameObject _door;
    [SerializeField] private float _timer;

    private void Awake()
    {
        _door.transform.DOLocalRotate(new Vector3(0, 90), _timer);
    }
}
