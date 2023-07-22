using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lovatto.MobileInput;
using static bl_GameInput;

public class bl_MobileInput
{
    // Set this to false to make the input unresponsive
    public static bool Interactable = true;
    public static List<int> ignoredTouches { get; private set; } = new List<int>();

    private static int m_Touch = -1;
    private static List<int> touchesList;
    private static Dictionary<string, bl_MobileButton> mobileButtons = new Dictionary<string, bl_MobileButton>();

    public static void Initialize()
    {
        touchesList = new List<int>();
        ignoredTouches = new List<int>();
        m_Touch = -1;
        Interactable = true;
    }

    public static void AddMobileButton(bl_MobileButton button)
    {
        if (mobileButtons.ContainsKey(button.ButtonName))
        {
            Debug.LogWarning($"A button with the name '{button.ButtonName}' is already registered, buttons with the same name are not allowed.");
            return;
        }

        mobileButtons.Add(button.ButtonName, button);
    }

    public static void RemoveMobileButton(bl_MobileButton button)
    {
        if (!mobileButtons.ContainsKey(button.ButtonName)) { return; }

        mobileButtons.Remove(button.ButtonName);
    }

    public static bl_MobileButton Button(string buttonName)
    {
        if (!mobileButtons.ContainsKey(buttonName))
        { /*Debug.LogWarning($"The button '{buttonName}' is not registered in the mobile input buttons.");*/
            return null;
        }
        return mobileButtons[buttonName];
    }

    /// <summary>
    /// is the button pressed
    /// </summary>
    /// <param name="buttonName"></param>
    /// <returns></returns>
    public static bool GetButton(string buttonName)
    {
        if (!Interactable)
            return false;

        if (!mobileButtons.ContainsKey(buttonName))
        {
            Debug.LogWarning($"The button '{buttonName}' is not registered in the mobile input buttons."); return false;
        }
        Debug.Log($"MOBILE GetButton {buttonName}");
#if UNITY_EDITOR
        if (bl_MobileInputSettings.Instance.UseKeyboardOnEditor)
        {
            return Input.GetKey(mobileButtons[buttonName].fallBackKey);
        }
#endif
        return mobileButtons[buttonName].isButton();
    }

    public static bool GetButtonDown(string buttonName)
    {
        if (!Interactable)
            return false;

        if (!mobileButtons.ContainsKey(buttonName)) { Debug.LogWarning($"The button '{buttonName}' is not registered in the mobile input buttons."); return false; }
#if UNITY_EDITOR
        if (bl_MobileInputSettings.Instance.UseKeyboardOnEditor)
        {
            return Input.GetKeyDown(mobileButtons[buttonName].fallBackKey);
        }
#endif
        Debug.Log($"MOBILE GetButtonDown {buttonName}");
        ActionButton(buttonName);

        return mobileButtons[buttonName].isButtonDown();
    }

    private static void ActionButton(string name)
    {
        if (name == "Fire") Fire();
        if (name == "Aim") Aim();
        if (name == "Crouch") Crouch();
        if (name == "Jump") Jump();

        if (name == "Interact") Interact();
        if (name == "Reload") Reload();
        if (name == "WeaponSlot") WeaponSlot(1);
        if (name == "QuickMelee") QuickMelee();

        if (name == "QuickNade") QuickNade();
        if (name == "Pause") Pause();
        if (name == "Scoreboard") Scoreboard();
        if (name == "SwitchFireMode") SwitchFireMode();

        if (name == "GeneralChat") GeneralChat();
        if (name == "TeamChat") TeamChat();
    }

    public static bool GetButtonUp(string buttonName)
    {
        if (!Interactable)
            return false;

        if (!mobileButtons.ContainsKey(buttonName)) { Debug.LogWarning($"The button '{buttonName}' is not registered in the mobile input buttons."); return false; }
#if UNITY_EDITOR
        if (bl_MobileInputSettings.Instance.UseKeyboardOnEditor)
        {
            return Input.GetKeyUp(mobileButtons[buttonName].fallBackKey);
        }
#endif
        Debug.Log($"MOBILE GetButtonDown {buttonName}");

        return mobileButtons[buttonName].isButtonUp();
    }

    /// <summary>
    /// Detect is the auto fire is triggered (lets say like if it's pressed)
    /// </summary>
    /// <returns></returns>
    public static bool AutoFireTriggered()
    {
        if (!Interactable)
            return false;

        if (bl_AutoFire.Instance == null)
            return false;

        return bl_AutoFire.Instance.isTriggered();
    }

    public static int GetUsableTouch()
    {
        if (Input.touches.Length <= 0)
        {
            m_Touch = -1;
            return m_Touch;
        }
        List<int> list = GetValuesFromTouches(Input.touches).Except<int>(ignoredTouches).ToList<int>();
        if (list.Count <= 0)
        {
            m_Touch = -1;
            return m_Touch;
        }
        if (!list.Contains(m_Touch))
        {
            m_Touch = list[0];
        }
        return m_Touch;
    }

    public static List<int> GetValuesFromTouches(Touch[] touches)
    {
        if (touchesList == null)
        {
            touchesList = new List<int>();
        }
        else
        {
            touchesList.Clear();
        }
        for (int i = 0; i < touches.Length; i++)
        {
            touchesList.Add(touches[i].fingerId);
        }
        return touchesList;
    }

    public static float TouchPadSensitivity { get => bl_MobileInputSettings.Instance.touchPadHorizontalSensitivity; set => bl_MobileInputSettings.Instance.touchPadHorizontalSensitivity = value; }

    public static bool EnableAutoFire { get => bl_MobileInputSettings.Instance.useAutoFire; set => bl_MobileInputSettings.Instance.useAutoFire = value; }

}