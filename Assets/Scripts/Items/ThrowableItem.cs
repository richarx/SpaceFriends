using Unity.Netcode;
using UnityEngine;

public class ThrowableItem : NetworkBehaviour
{
    private PickableItem pickableItem;

    private Vector2 direction;
    private Vector2 startPosition;

    private float maxTravelDistance = 4.0f;
    private float speed = 7.5f;
    
    private void Start()
    {
        pickableItem = GetComponent<PickableItem>();

        if (IsServer)
        {
            pickableItem.OnItemPickup.AddListener(OnPickup);
        }
    }

    private void Update()
    {
        if (!pickableItem.isBeingHeld)
        {
            if (isArrivedAtDestination())
                FallOnTheGround();
            else
                MoveTowardDestination();
        }
    }

    private void OnPickup(ItemHandler player)
    {
        ResetVelocityRpc(transform.position);
    }
    
    [Rpc(SendTo.Everyone)]
    private void ResetVelocityRpc(Vector2 position)
    {
        transform.position = position;
        direction = Vector2.zero;
        startPosition = Vector2.zero;
    }
    
    [Rpc(SendTo.Everyone)]
    private void SetVelocityRpc(Vector2 position, Vector2 throwDirection)
    {
        startPosition = position;
        direction = throwDirection.normalized;
    }

    public void ThrowItem(Vector2 throwDirection)
    {
        SetVelocityRpc(transform.position, throwDirection);
    }

    private void MoveTowardDestination()
    {
        Vector2 position = transform.position.ToVector2() + (direction * (speed * Time.deltaTime));
        transform.position = position;
        //UpdatePositionRpc(position);
    }

    [Rpc(SendTo.NotMe)]
    private void UpdatePositionRpc(Vector2 position)
    {
        transform.position = position;
    }

    private bool isArrivedAtDestination()
    {
        return (startPosition - transform.position.ToVector2()).magnitude >= maxTravelDistance;
    }

    private void FallOnTheGround()
    {
        if (IsServer)
            ResetVelocityRpc(transform.position);
    }
}
