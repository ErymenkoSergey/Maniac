using UnityEngine;

public class Interactive : bl_NetworkItem
{
    [SerializeField] private InteractivePlace _placeInteraction;
    [SerializeField] private Transform _pointSpawn;
    [SerializeField] private int _indexTeleport;

    public int GetIndex() => _indexTeleport;

    public void UseObject()
    {
        _placeInteraction.SetNewPlace(_pointSpawn);
    }
}
