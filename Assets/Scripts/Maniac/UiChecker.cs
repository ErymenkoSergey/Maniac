using UnityEngine;

public class UiChecker : MonoBehaviour
{
    [SerializeField] private GameObject _mobileControler;
    [SerializeField] private GameObject _pcControlerWeapons;
    [SerializeField] private GameObject _pcControlerBullets;

    private void Awake()
    {
        Debug.Log($"Application {Application.platform}");
        CheckPlatform();
    }

    private void CheckPlatform()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            SetControl(true);
        }
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            SetControl(false);
        }
    }

    private void SetControl(bool isTach)
    {
        _mobileControler.SetActive(isTach);
        _pcControlerWeapons.SetActive(!isTach);
        _pcControlerBullets.SetActive(!isTach);
    }
}
