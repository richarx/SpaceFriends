using Unity.Netcode;
using UnityEngine;

public class ThrowableItem : NetworkBehaviour
{
    private PickableItem pickableItem;

    private bool isThrown = false;
    private Vector2 direction;
    private Vector2 startPosition;

    private float maxTravelDistance = 5.0f;
    private float speed = 7.5f;
    
    private void Start()
    {
        pickableItem = GetComponent<PickableItem>();
    }

    private void Update()
    {
        if (isThrown)
        {
            if (isArrivedAtDestination())
                FallOnTheGround();
            else
                MoveTowardDestination();
        }
    }

    private void MoveTowardDestination()
    {
        Vector2 position = transform.position.ToVector2() + (direction * (speed * Time.deltaTime));
        transform.position = position;
        UpdatePositionRpc(position);
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
        isThrown = false;
    }

    public void ThrowItem(Vector2 throwDirection)
    {
        pickableItem.DetachItem();
        direction = throwDirection.normalized;
        startPosition = transform.position;
        isThrown = true;
    }
}
