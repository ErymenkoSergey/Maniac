using UnityEngine;
using static ShopData;
using TMPro;
using UnityEngine.UI;
using System;

public class UIShopPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameSkin;
    [SerializeField] private Image _iconSkin;
    [SerializeField] private int _index;
    [SerializeField] private Button _button;

    private LoadingShopData _loadingShopData;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        _loadingShopData.SelectSkin(_index);
    }

    public void SetData(ShopItem item, int index, LoadingShopData data)
    {
        _nameSkin.text = item.Name; 
        _iconSkin.sprite = item.Icon;
        _index = index;
        _loadingShopData = data;
    }
}
