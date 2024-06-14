using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PickableItem : NetworkBehaviour
{
    [HideInInspector]
    public UnityEvent<ItemHandler> OnItemPickup = new UnityEvent<ItemHandler>();
    [HideInInspector]
    public UnityEvent OnItemRelease = new UnityEvent();

    
    private enum ItemDisplayState
    {
        Idle,
        Selected,
        PickedUp
    }
    
    [SerializeField] private Collider2D wallCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite pickedUpSprite;
    
    [HideInInspector]
    public bool canBeThrown;

    private Collider2D trigger;
    
    private Vector2 floatDirection;
    private bool isInSpace => floatDirection != Vector2.zero;

    public bool isBeingUsed = false;
    public bool isBeingHeld => currentHolder != null;

    private bool previousHeldStatus = false;
    
    public ItemHandler currentHolder => ItemParentingAuthority.Instance != null ? ItemParentingAuthority.Instance.GetOwner(this) : null;

    private void Start()
    {
        canBeThrown = TryGetComponent(out ThrowableItem _);
        trigger = GetComponent<Collider2D>();
    }

    private void LateUpdate()
    {
        if (isBeingHeld)
        {
            if (!previousHeldStatus && IsServer)
            {
                SetItemStateRpc(ItemDisplayState.PickedUp);
                previousHeldStatus = true;
                OnItemPickup?.Invoke(currentHolder);
                floatDirection = Vector2.zero;
            }

            if (isBeingUsed)
                return;

            Vector2 position = currentHolder.itemHolderPosition;
            float direction = currentHolder.itemHolderDirection;
            
            transform.position = position;
            transform.localScale = new Vector3(direction, 1.0f, 1.0f);
            UpdatePositionAndDirectionRpc(position, direction);
        }
        else
        {
            if (previousHeldStatus && IsServer)
            {
                SetItemStateRpc(ItemDisplayState.Idle);
                previousHeldStatus = false;
                CheckForPlayerAfterReleaseRpc();
                OnItemRelease?.Invoke();
            }

            if (isInSpace && IsServer)
            {
                if (IsBlockedByWall())
                {
                    SetFloatingVelocityRpc(Vector2.zero);
                    Debug.Log("Zuzu : Blocked By Wall");
                    return;
                }
                
                Vector2 position = transform.position + floatDirection.ToVector3() * Time.deltaTime;
                transform.position = position;
                UpdatePositionAndDirectionRpc(position, transform.localScale.x);
            }
        }
    }

    [Rpc(SendTo.NotMe)]
    private void UpdatePositionAndDirectionRpc(Vector2 position, float direction)
    {
        transform.position = position;
        transform.localScale = new Vector3(direction, 1.0f, 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBeingHeld)
            return;
        
        if (other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerMovement>().IsOwner)
            SetSpriteState(ItemDisplayState.Selected);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isBeingHeld)
            return;
        
        if (other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerMovement>().IsOwner)
            SetSpriteState(ItemDisplayState.Idle);
    }

    [Rpc(SendTo.Everyone)]
    private void CheckForPlayerAfterReleaseRpc()
    {
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(trigger, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("Player") && result.transform.parent.GetComponent<PlayerMovement>().IsOwner)
            {
                SetSpriteState(ItemDisplayState.Selected);
                return;
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetItemStateRpc(ItemDisplayState state)
    {
        SetSpriteState(state);
    }
    
    public void DropItem()
    {
        transform.position = currentHolder.itemDropPosition;
        UpdatePositionAndDirectionRpc(currentHolder.itemDropPosition, currentHolder.itemHolderDirection);
    }
    
    [Rpc(SendTo.Server)]
    public void SetFloatingVelocityRpc(Vector2 velocity)
    {
        floatDirection = velocity;
    }
    
    private void SetSpriteState(ItemDisplayState state)
    {
        switch (state)
        {
            case ItemDisplayState.Idle:
                spriteRenderer.sprite = idleSprite;
                spriteRenderer.sortingOrder = 0;
                break;
            case ItemDisplayState.Selected:
                spriteRenderer.sprite = selectedSprite;
                spriteRenderer.sortingOrder = 0;
                break;
            case ItemDisplayState.PickedUp:
                spriteRenderer.sprite = pickedUpSprite;
                spriteRenderer.sortingOrder = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
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
}
