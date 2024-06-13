using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : NetworkBehaviour
{
    public static readonly UnityEvent<Transform> OnPlayerSpawn = new UnityEvent<Transform>();
    public readonly UnityEvent<bool> OnPlayerGoThroughAirlock = new UnityEvent<bool>();
    
    private Rigidbody2D attachedRigidbody;
    private AimHandler aimHandler;
    private FuelHandler fuelHandler;

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
        fuelHandler = GetComponent<FuelHandler>();

        if (!IsOwner)
            attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;

        if (IsOwner)
        {
            OnPlayerSpawn?.Invoke(transform);
            aimHandler = GetComponent<AimHandler>();
            GetComponent<CheatCodes>().OnPrimaryCheat.AddListener((s) =>
            {
                jetPackSpeed += s * 0.5f;
                Debug.Log($"Zuzu : Jetpack Speed updated : {jetPackSpeed}");
            });
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

    private bool resetJetPackVelocity = true;
    private Vector2 jetPackVelocity = Vector2.zero;
    private float jetPackSpeed = 3.0f;
    private float jetPackMaxSpeed = 6.0f;
    
    //TODO Problem here, but fuck me i cant find the solution
    private void ComputeMovingDirectionInSpace()
    {
        Vector2 inputDirection = PlayerInputs.ComputeInputDirection();
        if (resetJetPackVelocity)
        {
            jetPackVelocity = attachedRigidbody.velocity;
            resetJetPackVelocity = false;
        }

        if (fuelHandler.IsFuelEmpty)
        {
            direction.Value = inputDirection.normalized * 0.4f;
            return;
        }
        
        Vector2 tmpVelocity = jetPackVelocity + inputDirection.normalized * (jetPackSpeed * Time.deltaTime);

        if (jetPackVelocity.magnitude >= jetPackMaxSpeed && tmpVelocity.magnitude > jetPackVelocity.magnitude)
            tmpVelocity = jetPackVelocity;

        jetPackVelocity = tmpVelocity;
        direction.Value = inputDirection;
        
    }

    private void ComputeMovingDirectionInShip()
    {
        Vector2 inputDirection = PlayerInputs.ComputeInputDirection();
        direction.Value = inputDirection;
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

        resetJetPackVelocity = true;
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
                attachedRigidbody.velocity = SpaceshipSingleton.Instance.CurrentVelocity;
            else
            {
                jetPackVelocity = Vector2.zero;
                attachedRigidbody.velocity = Vector2.zero;
            }

            fuelHandler.UpdateFuelDisplayState(isInSpace);
        }
    }

    public void ApplyKnockBack(Vector2 knockDirection, float power)
    {
        Debug.Log($"Zuzu : ApplyKnockBack : isInSpace : {isInSpace} / direction : {knockDirection} / power : {power}");
        
        if (!isInSpace)
            return;
        
        jetPackVelocity += knockDirection.normalized * power;
    }
}
