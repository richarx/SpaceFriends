using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Fucking H count :
// |||

public class ItemParentingAuthority : NetworkBehaviour
{
    public static ItemParentingAuthority Instance;

    private Dictionary<ItemHandler, PickableItem> authority;

    private void Awake()
    {
        Instance = this;
    }

    public void RequestAuthority(ItemHandler player, PickableItem item)
    {
        RequestAuthorityRpc(player.NetworkObjectId, item.NetworkObjectId);
    }

    [Rpc(SendTo.Server)]
    private void RequestAuthorityRpc(ulong player, ulong item)
    {
        ItemHandler itemHandler =
            NetworkManager.Singleton.ConnectedClients[player].PlayerObject.GetComponent<ItemHandler>();

        PickableItem pickableItem =
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[item].GetComponent<PickableItem>();

        if (authority.ContainsKey(itemHandler) || authority.ContainsValue(pickableItem))
            return;
        
        authority.Add(itemHandler, pickableItem);
        SpreadAuthorityRpc(player, item, true);
    }

    [Rpc(SendTo.NotMe)]
    private void SpreadAuthorityRpc(ulong player, ulong item, bool ownership)
    {
        ItemHandler itemHandler =
            NetworkManager.Singleton.ConnectedClients[player].PlayerObject.GetComponent<ItemHandler>();

        PickableItem pickableItem =
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[item].GetComponent<PickableItem>();

        if (ownership)
            authority.Add(itemHandler, pickableItem);
        else
            authority.Remove(itemHandler);
    }

    public void ReleaseAuthority(ItemHandler player, PickableItem item)
    {
        RequestReleaseAuthorityRpc(player.NetworkObjectId, item.NetworkObjectId);
    }

    [Rpc(SendTo.Server)]
    private void RequestReleaseAuthorityRpc(ulong player, ulong item)
    {
        ItemHandler itemHandler =
            NetworkManager.Singleton.ConnectedClients[player].PlayerObject.GetComponent<ItemHandler>();

        PickableItem pickableItem =
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[item].GetComponent<PickableItem>();

        bool playerHasNoItem = !authority.ContainsKey(itemHandler);
        bool playerHasItem = !playerHasNoItem && authority[itemHandler] == pickableItem;

        if (playerHasNoItem)
        {
            Debug.LogError($"Zuzu : Error : Player has no item to loose ownership over : player : {player} / item : {item} - {pickableItem.name}");
            return;
        }

        if (!playerHasItem)
        {
            Debug.LogError($"Zuzu : Error : Player does not own this item : player : {player} / {item} - {pickableItem.name}");
            return;
        }

        authority.Remove(itemHandler);
        SpreadAuthorityRpc(player, item, false);
    }
}
