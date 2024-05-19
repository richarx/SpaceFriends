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
        return Gamepad.current.buttonSouth.wasPressedThisFrame;
    }
    
    private static bool CheckKeyboardPickup()
    {
        return Keyboard.current.eKey.wasPressedThisFrame;
    }

    public static bool CheckForSpeedIncrease()
    {
        return Keyboard.current.rightArrowKey.wasPressedThisFrame;
    }
    
    public static bool CheckForSpeedDecrease()
    {
        return Keyboard.current.leftArrowKey.wasPressedThisFrame;
    }
    
    public static bool CheckForZoomIncrease()
    {
        return Keyboard.current.upArrowKey.wasPressedThisFrame;
    }
    
    public static bool CheckForZoomDecrease()
    {
        return Keyboard.current.downArrowKey.wasPressedThisFrame;
    }
    
    public static bool CheckForResetObjective()
    {
        return Keyboard.current.spaceKey.wasPressedThisFrame;
    }
}
