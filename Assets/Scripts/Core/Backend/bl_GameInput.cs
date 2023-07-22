#if INPUT_MANAGER
using MFPS.InputManager;
#endif
using UnityEngine;

public class bl_GameInput 
{
    public static bool Fire(GameInputType inputType = GameInputType.Hold)
    {
#if INPUT_MANAGER
        if(inputType == GameInputType.Down)return bl_Input.isButton("SingleFire");
        else return GetInputManager("Fire", inputType);
#else
        return GetButton(KeyCode.Mouse0, inputType);
#endif
    }

    public static bool Run(GameInputType inputType = GameInputType.Hold)
    {
#if INPUT_MANAGER
        if (bl_InputData.Instance.runWithButton)
            return GetInputManager("Run", inputType);
        else
            return Input.GetAxis("Vertical") >= 1f;
#else
        return GetButton(KeyCode.LeftShift, inputType);
#endif
    }

    public static bool Aim(GameInputType inputType = GameInputType.Hold)
    {
#if INPUT_MANAGER
        return GetInputManager("Aim", inputType);
#else
        return GetButton(KeyCode.Mouse1, inputType);
#endif
    }

    public static bool Crouch(GameInputType inputType = GameInputType.Hold)
    {
#if INPUT_MANAGER
        return GetInputManager("Crouch", inputType);
#else
        return GetButton(KeyCode.C, inputType);
#endif
    }

    public static bool Jump(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        return GetInputManager("Jump", inputType);
#else
        return GetButton(KeyCode.Space, inputType);
#endif
    }

    public static bool Interact(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        return GetInputManager("Interact", inputType);
#else
        return GetButton(KeyCode.F, inputType);
#endif
    }

    public static bool Reload(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        return GetInputManager("Reload", inputType);
#else
        return GetButton(KeyCode.R, inputType);
#endif
    }

    public static bool WeaponSlot(int slotID, GameInputType inputType = GameInputType.Down) // смена оружия 
    {
#if INPUT_MANAGER
        return GetInputManager($"Weapon{slotID}", inputType);
#else
        return GetButton($"{slotID}", inputType);
#endif
    }

    public static bool QuickMelee(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        return GetInputManager("FastKnife", inputType);
#else
        return GetButton(KeyCode.V, inputType);
#endif
    }

    public static bool QuickNade(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        return GetInputManager("QuickNade", inputType);
#else
        return GetButton(KeyCode.G, inputType);
#endif
    }

    public static bool Pause(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        if (bl_Input.isGamePad)
        {
            return bl_Input.isStartPad;
        }
#endif
        return GetButton(KeyCode.Escape, inputType);
    }

    public static bool Scoreboard(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        return GetInputManager("Scoreboard", inputType);
#else
        return GetButton(KeyCode.Tab, inputType);
#endif
    }

    public static bool SwitchFireMode(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        return GetInputManager("FireType", inputType);
#else
        return GetButton(KeyCode.B, inputType);
#endif
    }

    public static bool GeneralChat(GameInputType inputType = GameInputType.Down)
    {        
#if INPUT_MANAGER
        return GetInputManager("GeneralChat", inputType);
#else
        return GetButton(KeyCode.T, inputType);
#endif
    }

    public static bool TeamChat(GameInputType inputType = GameInputType.Down)
    {
#if INPUT_MANAGER
        return GetInputManager("TeamChat", inputType);
#else
        return GetButton(KeyCode.Y, inputType);
#endif
    }

    public static float Vertical
    {
        get
        {
            if (!bl_RoomMenu.Instance.isCursorLocked || bl_GameData.Instance.isChating) return 0;
#if !INPUT_MANAGER
            return Input.GetAxis("Vertical");
#else
            return bl_Input.VerticalAxis;
#endif
        }
    }

    public static float Horizontal
    {
        get
        {
            if (!bl_RoomMenu.Instance.isCursorLocked || bl_GameData.Instance.isChating) return 0;
#if !INPUT_MANAGER
            return Input.GetAxis("Horizontal");
#else
            return bl_Input.HorizontalAxis;
#endif
        }
    }

    public static float MouseX
    {
        get
        {
            if (!bl_RoomMenu.Instance.isCursorLocked || bl_GameData.Instance.isChating) return 0;

            return Input.GetAxis("Mouse X");
        }
    }

    public static float MouseY
    {
        get
        {
            if (!bl_RoomMenu.Instance.isCursorLocked || bl_GameData.Instance.isChating)
                return 0;

            return Input.GetAxis("Mouse Y");
        }
    }

    public static bool GetButton(KeyCode key, GameInputType inputType)
    {
        if (!bl_RoomMenu.Instance.isCursorLocked || bl_GameData.Instance.isChating) 
            return false;

            if (inputType == GameInputType.Hold) { return Input.GetKey(key); }
        else if(inputType == GameInputType.Down) { return Input.GetKeyDown(key); }
        else { return Input.GetKeyUp(key); }
    }

    public static bool GetButton(string key, GameInputType inputType)
    {
        if (!bl_RoomMenu.Instance.isCursorLocked || bl_GameData.Instance.isChating) return false;

        if (inputType == GameInputType.Hold) { return Input.GetKey(key); }
        else if (inputType == GameInputType.Down) { return Input.GetKeyDown(key); }
        else { return Input.GetKeyUp(key); }
    }

#if INPUT_MANAGER
    public static bool GetInputManager(string key, GameInputType inputType)
    {
    if (!bl_RoomMenu.Instance.isCursorLocked || bl_GameData.Instance.isChating) return false;
        if(inputType == GameInputType.Hold) { return bl_Input.isButton(key); }
        else if (inputType == GameInputType.Down) { return bl_Input.isButtonDown(key); }
        else { return bl_Input.isButtonUp(key); }
    }
#endif
}

public enum GameInputType
{
    Down,
    Up,
    Hold,
}