using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : NetworkBehaviour
{
    public static readonly UnityEvent<Transform> OnPlayerSpawn = new UnityEvent<Transform>();

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

        if (!IsOwner)
            attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;
        
        if (IsOwner)
            OnPlayerSpawn?.Invoke(transform);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        Vector2 inputDirection = PlayerInputs.ComputeInputDirection();

        direction.Value = inputDirection;
    }

    private void FixedUpdate()
    {
        if (!IsOwner || MoveDirection.magnitude <= 0.15f)
            return;

        Vector2 newPosition = (Vector2)transform.position + (MoveDirection * (speed * Time.fixedDeltaTime));

        attachedRigidbody.MovePosition(newPosition);
    }
}
