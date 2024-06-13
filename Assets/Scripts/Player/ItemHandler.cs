using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class ItemHandler : NetworkBehaviour
{
    [SerializeField] private Transform itemHolder;
    [SerializeField] private Transform itemDropper;
    [SerializeField] private Collider2D playerCollider;

    private PlayerMovement playerMovement;
    private Rigidbody2D attachedRigidbody;
    
    public Vector2 itemHolderPosition => itemHolder.position;
    public float itemHolderDirection => itemHolder.localScale.x;
    public Vector2 itemDropPosition => itemDropper.position;

    public bool IsHoldingItem => currentItem != null;
    public bool IsItemThrowable => IsHoldingItem && currentItem!.canBeThrown;
    public string itemName => IsHoldingItem ? currentItem!.name : "none";
    [CanBeNull] private PickableItem currentItem => ItemParentingAuthority.Instance != null ? ItemParentingAuthority.Instance.GetItem(this) : null;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        attachedRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        
        if (PlayerInputs.CheckForPickupInput())
        {
            if (IsHoldingItem)
                DropItem();
            else
                TryToPickupItem();
        }
        
        if (IsHoldingItem && PlayerInputs.CheckForUseItem())
        {
            Debug.Log("Zuzu : CheckForUseItem");
            UseItem();
        }
    }
    
    public void ThrowItem(Vector2 direction)
    {
        if (!IsHoldingItem)
            return;
        
        ThrowableItem throwableItem = currentItem!.GetComponent<ThrowableItem>();
        if (throwableItem != null)
        {
            ItemParentingAuthority.Instance.ReleaseAuthority(this, currentItem);

            Vector2 playerVelocity = playerMovement.IsInSpace ? attachedRigidbody.velocity : Vector2.zero;
            throwableItem.ThrowItem(NetworkObjectId, direction, playerVelocity);
            playerMovement.ApplyKnockBack(direction * -1.0f, 3.0f);
        }
        else
            Debug.Log("Zuzu : ItemHandler item not throwable");
    }

    private void UseItem()
    {
        if (!IsHoldingItem)
            return;
        
        UsableItem usableItem = currentItem!.GetComponent<UsableItem>();
        if (usableItem != null)
            usableItem.UseItem(this);
        else
            Debug.Log("Zuzu : ItemHandler item not usable");
    }

    private void DropItem()
    {
        Debug.Log($"Zuzu : Request Dropping item : {currentItem}");

        if (IsHoldingItem)
            currentItem.SetFloatingVelocityRpc(attachedRigidbody.velocity * 0.5f);

        ItemParentingAuthority.Instance.ReleaseAuthority(this, currentItem);
    }

    private void TryToPickupItem()
    {
        PickableItem targetItem = LookForPickupItem();

        if (targetItem != null)
            PickupItem(targetItem);
    }

    private void PickupItem(PickableItem targetItem)
    {
        Debug.Log($"Zuzu : Request PickupItem : {targetItem}");

        ItemParentingAuthority.Instance.RequestAuthority(this, targetItem);
    }

    private PickableItem LookForPickupItem()
    {
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(playerCollider, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return null;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("Item"))
                return result.GetComponent<PickableItem>();
        }

        return null;
    }
}
