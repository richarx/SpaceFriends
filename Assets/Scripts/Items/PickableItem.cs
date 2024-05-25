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

    private bool isPickedUp = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPickedUp)
            return;
        
        if (other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerMovement>().IsOwner)
            SetSpriteState(ItemDisplayState.Selected);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isPickedUp)
            return;
        
        if (other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerMovement>().IsOwner)
            SetSpriteState(ItemDisplayState.Idle);
    }

    public void SetItemAsPickedUp()
    {
        isPickedUp = true;
        SetSpriteState(ItemDisplayState.PickedUp);
    }
    
    public void SetItemAsDropped()
    {
        isPickedUp = false;
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
