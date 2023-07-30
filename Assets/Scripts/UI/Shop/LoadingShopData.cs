using UnityEngine;
using UnityEngine.UI;
using static ShopData;

public class LoadingShopData : MonoBehaviour
{
    [SerializeField] private ShopData _shopData;
    [SerializeField] private GameObject _uiPrefabContent;
    [SerializeField] private Transform _contentSkins, _contentWeapon;
    [SerializeField] private Image _iconSkin;

    private void Start()
    {
        OnVisualCharacter(_shopData.CurrentIndexSkin);
        LoadUIContent(_shopData.SkinShopItems, _contentSkins);
    }

    private void LoadUIContent(ShopItem[] items, Transform transform)
    {
        for (int i = 0; i < items.Length; i++)
        {
            Instantiate(_uiPrefabContent, transform).GetComponent<UIShopPrefab>().SetData(items[i], i, this);
        }
    }

    private void OnVisualCharacter(int index)
    {
        _iconSkin.sprite = _shopData.SkinShopItems[index].Icon;
    }

    public void SelectSkin(int id)
    {
        _shopData.SetSkinPlayer(id);
        OnVisualCharacter(id);
    }
}
