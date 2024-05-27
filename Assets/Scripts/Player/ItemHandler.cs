using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemHandler : NetworkBehaviour
{
    [SerializeField] private Transform itemHolder;
    [SerializeField] private Transform itemDropper;
    [SerializeField] private Collider2D playerCollider;

    public Vector2 itemHolderPosition => itemHolder.position;
    public float itemHolderDirection => itemHolder.localScale.x;
    public Vector2 itemDropPosition => itemDropper.position;

    public bool IsHoldingItem => currentItem != null;
    public bool IsItemThrowable => IsHoldingItem && currentItem.canBeThrown;
    public string itemName => currentItem.name;
    private PickableItem currentItem = null;

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
        ThrowableItem throwableItem = currentItem.GetComponent<ThrowableItem>();
        if (throwableItem != null)
        {
            throwableItem.ThrowItem(direction);
            currentItem = null;
        }
        else
            Debug.Log("Zuzu : ItemHandler item not throwable");
    }

    private void UseItem()
    {
        UsableItem usableItem = currentItem.GetComponent<UsableItem>();
        if (usableItem != null)
            usableItem.UseItem(this);
        else
            Debug.Log("Zuzu : ItemHandler item not usable");
    }

    private void DropItem()
    {
        Debug.Log($"Zuzu : Dropping item : {currentItem}");
        currentItem.DropItem();
        currentItem = null;
    }

    private void TryToPickupItem()
    {
        PickableItem targetItem = LookForPickupItem();

        if (targetItem != null)
            PickupItem(targetItem);
    }

    private void PickupItem(PickableItem targetItem)
    {
        Debug.Log($"Zuzu : PickupItem : {targetItem}");

        targetItem.PickedUpItem(this);
        currentItem = targetItem;
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
