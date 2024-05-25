using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemHandler : NetworkBehaviour
{
    [SerializeField] private Transform itemHolder;
    [SerializeField] private Transform itemDropPosition;
    [SerializeField] private Collider2D playerCollider;

    public bool IsHoldingItem => currentItem != null;
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

        if (IsHoldingItem)
        {
            currentItem.transform.position = itemHolder.position;
        }
    }

    private void DropItem()
    {
        Debug.Log($"Zuzu : Dropping item : {currentItem}");
        currentItem.SetItemAsDropped();
        currentItem.transform.position = itemDropPosition.transform.position;
        if (!IsServer)
            currentItem.RemoveOwnershipRpc();
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

        targetItem.SetItemAsPickedUp();
        if (!targetItem.NetworkObject.IsOwner)
            targetItem.GiveOwnershipRpc(OwnerClientId);
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
