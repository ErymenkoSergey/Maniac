using UnityEngine;

public class bl_MapMods : MonoBehaviour
{
    public bool infinityAmmo = false;

    private void OnEnable()
    {
        bl_EventHandler.onLocalPlayerSpawn += OnLocalPlayerSpawn;
    }

    private void OnDisable()
    {
        bl_EventHandler.onLocalPlayerSpawn -= OnLocalPlayerSpawn;
    }

    void OnLocalPlayerSpawn()
    {
        var p = bl_MFPS.LocalPlayerReferences;

        if (infinityAmmo) p.gunManager.SetInfinityAmmoToAllEquippeds(true);
    }
}