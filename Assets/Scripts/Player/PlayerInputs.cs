using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs
{
    public static Vector2 ComputeInputDirection()
    {
        Vector2 gamepadDirection = ComputeGamepadDirection();
        Vector2 keyboardDirection = ComputeKeyboardDirection();

        return (gamepadDirection + keyboardDirection).normalized;
    }

    private static Vector2 ComputeGamepadDirection()
    {
        if (Gamepad.current == null)
            return Vector2.zero;
        
        Vector2 input = new Vector2(Gamepad.current.leftStick.x.ReadValue(), Gamepad.current.leftStick.y.ReadValue());

        return input.magnitude >= 0.15f ? input.normalized : Vector2.zero;
    }

    private static Vector2 ComputeKeyboardDirection()
    {
        Vector2 inputDirection = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            inputDirection.y = 1.0f;
        if (Keyboard.current.sKey.isPressed)
            inputDirection.y = -1.0f;
        if (Keyboard.current.dKey.isPressed)
            inputDirection.x = 1.0f;
        if (Keyboard.current.aKey.isPressed)
            inputDirection.x = -1.0f;

        return inputDirection.normalized;
    }

    public static bool CheckForPickupInput()
    {
        bool gamepad = CheckGamepadPickup();
        bool keyboard = CheckKeyboardPickup();

        return gamepad || keyboard;
    }

    private static bool CheckGamepadPickup()
    {
        if (Gamepad.current == null)
            return false;
        
        return Gamepad.current.buttonSouth.wasPressedThisFrame;
    }
    
    private static bool CheckKeyboardPickup()
    {
        return Keyboard.current.eKey.wasPressedThisFrame;
    }

    public static bool CheckForSpeedIncrease()
    {
        return Keyboard.current.rightArrowKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.dpad.right.wasPressedThisFrame);
    }
    
    public static bool CheckForSpeedDecrease()
    {
        return Keyboard.current.leftArrowKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.dpad.left.wasPressedThisFrame);
    }
    
    public static bool CheckForZoomIncrease()
    {
        return Keyboard.current.upArrowKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.dpad.up.wasPressedThisFrame);
    }
    
    public static bool CheckForZoomDecrease()
    {
        return Keyboard.current.downArrowKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.dpad.down.wasPressedThisFrame);
    }
    
    public static bool CheckForUseItem()
    {
        return Keyboard.current.fKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame);
    }
    
    public static bool CheckForSwapMap()
    {
        return Keyboard.current.enterKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame);
    }
}
