using System;
using Unity.Netcode;
using UnityEngine;

public class PickableItem : NetworkBehaviour
{
    private enum ItemDisplayState
    {
        Idle,
        Selected,
        PickedUp
    }
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite pickedUpSprite;
    
    [HideInInspector]
    public bool canBeThrown;

    public bool isBeingHeld => currentHolder != null;
    
    private ItemHandler currentHolder => ItemParentingAuthority.Instance != null ? ItemParentingAuthority.Instance.GetOwner(this) : null;

    private void Start()
    {
        canBeThrown = TryGetComponent(out ThrowableItem _);
    }

    private void LateUpdate()
    {
        if (isBeingHeld)
        {
            Vector2 position = currentHolder.itemHolderPosition;
            float direction = currentHolder.itemHolderDirection;
            
            transform.position = position;
            transform.localScale = new Vector3(direction, 1.0f, 1.0f);
            UpdatePositionAndDirectionRpc(position, direction);
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

    public void PickedUpItem(ItemHandler itemHandler)
    {
        SetItemAsPickedUpRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    private void SetItemAsPickedUpRpc()
    {
        SetSpriteState(ItemDisplayState.PickedUp);
    }
    
    public void DropItem()
    {
        transform.position = currentHolder.itemDropPosition;
        UpdatePositionAndDirectionRpc(currentHolder.itemDropPosition, currentHolder.itemHolderDirection);

        SetItemAsDroppedRpc();
    }

    public void DetachItem()
    {
        SetItemAsDroppedRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void SetItemAsDroppedRpc()
    {
        SetSpriteState(ItemDisplayState.Selected);
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
    
    [Rpc(SendTo.Server)]
    public void GiveOwnershipRpc(ulong targetId)
    {
        NetworkObject.ChangeOwnership(targetId);
    }
    
    [Rpc(SendTo.Server)]
    public void RemoveOwnershipRpc()
    {
        NetworkObject.RemoveOwnership();
    }
}
