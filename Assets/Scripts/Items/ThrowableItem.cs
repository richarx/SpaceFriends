using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ThrowableItem : NetworkBehaviour
{
    private PickableItem pickableItem;
    [SerializeField] private Collider2D wallCollider;

    private Vector2 direction;
    private Vector2 playerVelocity;
    private Vector2 startPosition;

    private bool isMoving => direction != Vector2.zero;

    private float maxTravelDistance = 30.0f;
    private float speed = 10.0f;

    private ulong previousOwner;
    
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
        if (!pickableItem.isBeingHeld && isMoving)
        {
            if (isArrivedAtDestination())
                FallOnTheGround();
            else
                MoveTowardDestination();
        }
    }

    private void OnPickup(ItemHandler player)
    {
        ResetVelocityRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    private void ResetVelocityRpc()
    {
        direction = Vector2.zero;
        startPosition = Vector2.zero;
    }

    private void ResetVelocityLocal()
    {
        direction = Vector2.zero;
        startPosition = Vector2.zero;
    }
    
    [Rpc(SendTo.Everyone)]
    private void SetVelocityRpc(Vector2 position, Vector2 throwDirection, Vector2 playerCurrentVelocity, ulong player)
    {
        startPosition = position;
        direction = throwDirection.normalized;
        playerVelocity = playerCurrentVelocity;
        previousOwner = player;
    }

    public void ThrowItem(ulong player, Vector2 throwDirection, Vector2 playerCurrentVelocity)
    {
        SetVelocityRpc(transform.position, throwDirection, playerCurrentVelocity, player);
    }

    private void MoveTowardDestination()
    {
        Vector2 position = transform.position.ToVector2() + (direction * (speed * Time.deltaTime)) + playerVelocity * Time.deltaTime;
        transform.position = position;
    }

    private bool isArrivedAtDestination()
    {
        if (IsBlockedByWall())
        {
            Debug.Log("Zuzu : Blocked By Wall");
            return true;
        }

        return (startPosition - transform.position.ToVector2()).magnitude >= maxTravelDistance;
    }

    private bool IsBlockedByWall()
    {
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(wallCollider, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return false;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("Wall"))
                return true;
        }

        return false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickableItem.isBeingHeld)
            return;
        
        if (!isMoving)
            return;

        bool isPlayer = other.CompareTag("Player");
            
        if (!isPlayer)
            return;

        ItemHandler player = other.transform.parent.GetComponent<ItemHandler>();

        if (player.NetworkObjectId == previousOwner)
            return;
        
        bool isLocalPlayer = player.IsOwner;
        
        if (!isLocalPlayer)
            return;

        bool isPlayerEmptyHanded = ItemParentingAuthority.Instance.GetItem(player) == null;
        
        if (isPlayerEmptyHanded)
            ItemParentingAuthority.Instance.RequestAuthority(player, pickableItem);
    }

    private void FallOnTheGround()
    {
        ResetVelocityLocal();
        if (IsServer)
            ResetVelocityRpc();
    }
}
