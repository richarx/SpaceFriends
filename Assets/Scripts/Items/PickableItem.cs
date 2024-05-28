using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PickableItem : NetworkBehaviour
{
    public UnityEvent<ItemHandler> OnItemPickup = new UnityEvent<ItemHandler>();
    public UnityEvent OnItemRelease = new UnityEvent();

    
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

    private bool previousHeldStatus = false;
    
    public ItemHandler currentHolder => ItemParentingAuthority.Instance != null ? ItemParentingAuthority.Instance.GetOwner(this) : null;

    private void Start()
    {
        canBeThrown = TryGetComponent(out ThrowableItem _);
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
            }
            
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
                OnItemRelease?.Invoke();
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
    private void SetItemStateRpc(ItemDisplayState state)
    {
        SetSpriteState(state);
    }
    
    public void DropItem()
    {
        transform.position = currentHolder.itemDropPosition;
        UpdatePositionAndDirectionRpc(currentHolder.itemDropPosition, currentHolder.itemHolderDirection);
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
}
