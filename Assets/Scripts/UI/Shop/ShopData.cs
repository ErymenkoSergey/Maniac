using System;
using UnityEngine;

//[CreateAssetMenu(menuName = "ShopData")]
public class ShopData : ScriptableObject
{
    [SerializeField] private ShopItem[] _skinShopItems;
    public ShopItem[] SkinShopItems => _skinShopItems;

    [SerializeField] private ShopItem[] _weaponShopItem;
    public ShopItem[] WeaponShopItem => _weaponShopItem;

    public int CurrentIndexSkin;
    public int CurrentIndexWeapon;

    public bl_PlayerNetwork GetSkinHiding()
    {
        return SkinShopItems[CurrentIndexSkin].Prefab.GetComponent<bl_PlayerNetwork>();
    }

    public bl_PlayerNetwork GetSkinManiac()
    {
        return SkinShopItems[CurrentIndexSkin].Prefab.GetComponent<bl_PlayerNetwork>();
    }

    public void SetSkinPlayer(int index)
    {
        var skinHiding = SkinShopItems[index].Prefab.GetComponent<bl_PlayerNetwork>();
        skinHiding.GetComponent<bl_PlayerSettings>().PlayerTeam = Team.Hiding;
        bl_GameData.Instance.Player1 = skinHiding;

        var skinManiac = SkinShopItems[index].Prefab.GetComponent<bl_PlayerNetwork>();
        skinManiac.GetComponent<bl_PlayerSettings>().PlayerTeam = Team.Maniac;
        bl_GameData.Instance.Player2 = skinManiac;
    }

    [Serializable]
    public struct ShopItem
    {
        public string Name;
        public Sprite Icon;
        public GameObject Prefab;
        public bool _isAcquired;
    }

    private static bl_PhotonNetwork PhotonGameInstance = null;
    public static bool isDataCached = false;
    private static bool isCaching = false;
    private static ShopData m_instance;
    public static ShopData Instance
    {
        get
        {
            if (m_instance == null && !isCaching)
            {
                if (!isDataCached && Application.isPlaying)
                {
                    Debug.Log("GameData was cached synchronous, that could cause bottleneck on load, try caching it asynchronous with AsyncLoadData()");
                    isDataCached = true;
                }
                m_instance = Resources.Load("ShopData", typeof(ShopData)) as ShopData;
            }

            //check that there's an instance of the Photon object in scene
            if (PhotonGameInstance == null && Application.isPlaying)
            {
                if (bl_RoomMenu.Instance != null && bl_RoomMenu.Instance.isApplicationQuitting) return m_instance;

                PhotonGameInstance = bl_PhotonNetwork.Instance;
                if (PhotonGameInstance == null)
                {
                    try
                    {
                        var pgo = new GameObject("PhotonGame");
                        PhotonGameInstance = pgo.AddComponent<bl_PhotonNetwork>();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return m_instance;
        }
    }
}