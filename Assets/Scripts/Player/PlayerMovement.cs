using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : NetworkBehaviour
{
    public static readonly UnityEvent<Transform> OnPlayerSpawn = new UnityEvent<Transform>();
    public readonly UnityEvent<bool> OnPlayerGoThroughAirlock = new UnityEvent<bool>();
    
    private Rigidbody2D attachedRigidbody;
    private AimHandler aimHandler;

    private bool isInit = false;
    private bool isPositionInit = false;
    
    public bool isLocked = false;

    public bool IsInSpace => isInSpace;
    private bool isInSpace = false;
    
    public Vector2 MoveDirection => direction.Value;
    private NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<float> speed = new NetworkVariable<float>(
        6,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        attachedRigidbody = GetComponent<Rigidbody2D>();

        if (!IsOwner)
            attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;

        if (IsOwner)
        {
            OnPlayerSpawn?.Invoke(transform);
            aimHandler = GetComponent<AimHandler>();
        }

        TryToInitializePosition();

        isInit = true;
    }

    private void Update()
    {
        if (!isInit)
            return;

        if (!isPositionInit)
        {
            TryToInitializePosition();
            return;
        }
        
        UpdateSpeed();

        if (!IsOwner)
            return;

        if (isLocked)
        {
            direction.Value = Vector2.zero;
            return;
        }

        if (isInSpace)
            ComputeMovingDirectionInSpace();
        else
            ComputeMovingDirectionInShip();
    }

    private Vector2 jetPackVelocity = Vector2.zero;
    private float jetPackSpeed = 3.0f;
    private float jetPackMaxSpeed = 6.0f;
    
    private void ComputeMovingDirectionInSpace()
    {
        Vector2 inputDirection = PlayerInputs.ComputeInputDirection();
        jetPackVelocity += inputDirection.normalized * (jetPackSpeed * Time.deltaTime);
        jetPackVelocity = Vector2.ClampMagnitude(jetPackVelocity, jetPackMaxSpeed);
        direction.Value = inputDirection;
    }

    private void ComputeMovingDirectionInShip()
    {
        Vector2 inputDirection = PlayerInputs.ComputeInputDirection();
        direction.Value = inputDirection;
    }

    private void UpdateSpeed()
    {
        if (IsServer && PlayerInputs.CheckForSpeedIncrease())
            speed.Value += 1.0f;

        if (IsServer && PlayerInputs.CheckForSpeedDecrease())
            speed.Value = Mathf.Max(speed.Value - 1.0f, 0.0f);
    }

    private void FixedUpdate()
    {
        if (!isInit)
            return;

        if (!IsOwner)
            return;

        if (isInSpace)
            ApplyMovementInSpace();
        else if (MoveDirection.magnitude > 0.15f)
            ApplyMovementInShip();
    }

    private void ApplyMovementInSpace()
    {
        attachedRigidbody.velocity = jetPackVelocity;
    }

    private void ApplyMovementInShip()
    {
        float moveSpeed = aimHandler.isAiming ? speed.Value / 2.0f : speed.Value;
        
        Vector2 newPosition = (Vector2)transform.position + (MoveDirection * (moveSpeed * Time.fixedDeltaTime));

        attachedRigidbody.MovePosition(newPosition);
    }

    private void TryToInitializePosition()
    {
        if (SpawnPosition.Instance != null)
        {
            transform.position = SpawnPosition.Instance.GetSpawnPosition();
            isPositionInit = true;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SetInSpaceStatusRpc(bool status)
    {
        isInSpace = status;

        if (IsOwner)
        {
            if (isInSpace)
                jetPackVelocity = SpaceshipSingleton.Instance.CurrentVelocity;
            else
            {
                jetPackVelocity = Vector2.zero;
                attachedRigidbody.velocity = Vector2.zero;
            }
        }
    }
}
