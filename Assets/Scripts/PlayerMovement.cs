using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D attachedRigidbody;
    
    public Vector2 MoveDirection => direction.Value;
    private NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    
    private float speed = 5.0f;

    private void Start()
    {
        attachedRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        Vector2 inputDirection = ComputeInputDirection();

        direction.Value = inputDirection;
    }

    private void FixedUpdate()
    {
        if (!IsOwner || MoveDirection.magnitude <= 0.15f)
            return;
        
        Vector2 newPosition = (Vector2)transform.position + (MoveDirection * (speed * Time.fixedDeltaTime));
        
        attachedRigidbody.MovePosition(newPosition);
    }

    private Vector2 ComputeInputDirection()
    {
        Vector2 gamepadDirection = ComputeGamepadDirection();
        Vector2 keyboardDirection = ComputeKeyboardDirection();

        return (gamepadDirection + keyboardDirection).normalized;
    }

    private Vector2 ComputeGamepadDirection()
    {
        Vector2 input = new Vector2(Gamepad.current.leftStick.x.ReadValue(), Gamepad.current.leftStick.y.ReadValue());

        return input.magnitude >= 0.15f ? input.normalized : Vector2.zero;
    }
    
    private Vector2 ComputeKeyboardDirection()
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
}
