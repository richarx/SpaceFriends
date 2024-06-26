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
    
    public static Vector2 ComputeAimDirection(Vector2 currentPosition, Camera camera, bool isInSpace)
    {
        if (Gamepad.current != null)
            return ComputeGamepadAimDirection();

        return ComputeKeyboardAimDirection(currentPosition, camera, isInSpace);
    }

    private static Vector2 ComputeGamepadAimDirection()
    {
        if (Gamepad.current == null)
            return Vector2.zero;
        
        Vector2 input = new Vector2(Gamepad.current.rightStick.x.ReadValue(), Gamepad.current.rightStick.y.ReadValue());

        return input.magnitude >= 0.15f ? input.normalized : Vector2.zero;
    }
    
    private static Vector2 ComputeKeyboardAimDirection(Vector2 currentPosition, Camera camera, bool isInSpace)
    {
        Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 position = isInSpace
            ? currentPosition
            : SpaceshipSingleton.Instance.GetPosition() - StaticShipSingleton.Instance.GetPosition() + currentPosition;
        
        return (mousePosition - position).normalized;
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
        return Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    public static bool CheckForUseItem()
    {
        return Keyboard.current.eKey.wasPressedThisFrame || /*Mouse.current.leftButton.wasPressedThisFrame ||*/ (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame);
    }
    
    public static bool CheckForThrowItem()
    {
        return Mouse.current.leftButton.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);
    }
    
    public static bool CheckForAim()
    {
        return Mouse.current.rightButton.isPressed || (Gamepad.current != null && Gamepad.current.leftTrigger.isPressed);
    }

    public static bool CheckForStartGame()
    {
        return Keyboard.current.enterKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);
    }
    
    public static bool CheckForPrimaryCheatUp()
    {
        return Keyboard.current.rightArrowKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.dpad.right.wasPressedThisFrame);
    }
    
    public static bool CheckForPrimaryCheatDown()
    {
        return Keyboard.current.leftArrowKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.dpad.left.wasPressedThisFrame);
    }
    
    public static bool CheckForSecondaryCheatUp()
    {
        return Keyboard.current.upArrowKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.dpad.up.wasPressedThisFrame);
    }
    
    public static bool CheckForSecondaryCheatDown()
    {
        return Keyboard.current.downArrowKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.dpad.down.wasPressedThisFrame);
    }
    
    public static bool CheckForTertiaryCheat()
    {
        return Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame);
    }
}
