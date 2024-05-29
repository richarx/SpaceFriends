using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

// Fucking H count :
// |||

public class ItemParentingAuthority : NetworkBehaviour
{
    public static ItemParentingAuthority Instance;

    private Dictionary<ItemHandler, PickableItem> authority = new Dictionary<ItemHandler, PickableItem>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            RequestAuthoritySpreadRpc();
    }

    [CanBeNull]
    public PickableItem GetItem(ItemHandler player)
    {
        if (authority.TryGetValue(player, out var item))
            return item;
        else
            return null;
    }

    [CanBeNull]
    public ItemHandler GetOwner(PickableItem item)
    {
        if (authority.ContainsValue(item))
            return authority.First((pair) => pair.Value == item).Key;
        else
            return null;
    }

    public void RequestAuthority(ItemHandler player, PickableItem item)
    {
        RequestAuthorityRpc(player.NetworkObjectId, item.NetworkObjectId);
    }

    [Rpc(SendTo.Server)]
    private void RequestAuthorityRpc(ulong player, ulong item)
    {
        ItemHandler itemHandler = GetPlayerFromId(player);
        PickableItem pickableItem = GetItemFromId(item);

        if (authority.ContainsKey(itemHandler) || authority.ContainsValue(pickableItem))
            return;

        authority.Add(itemHandler, pickableItem);
        SpreadAuthority();
    }

    public void ReleaseAuthority(ItemHandler player, PickableItem item)
    {
        RequestReleaseAuthorityRpc(player.NetworkObjectId, item.NetworkObjectId);
    }

    [Rpc(SendTo.Server)]
    private void RequestReleaseAuthorityRpc(ulong player, ulong item)
    {
        ItemHandler itemHandler = GetPlayerFromId(player);
        PickableItem pickableItem = GetItemFromId(item);

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
        SpreadAuthority();
    }

    [Rpc(SendTo.Server)]
    private void RequestAuthoritySpreadRpc()
    {
        SpreadAuthority();
    }
    
    private void SpreadAuthority()
    {
        int count = authority.Count;

        ulong[] players = new ulong[count];
        ulong[] items = new ulong[count];

        int i = 0;
        foreach (KeyValuePair<ItemHandler,PickableItem> pair in authority)
        {
            players[i] = pair.Key.NetworkObjectId;
            items[i] = pair.Value.NetworkObjectId;
            i += 1;
        }
        
        SpreadAuthorityRpc(players, items);
    }
    
    [Rpc(SendTo.NotMe)]
    private void SpreadAuthorityRpc(ulong[] players, ulong[] items)
    {
        authority = new Dictionary<ItemHandler, PickableItem>();

        for (int i = 0; i < players.Length; i++)
            authority.Add(GetPlayerFromId(players[i]), GetItemFromId(items[i]));
    }

    private ItemHandler GetPlayerFromId(ulong player)
    {
        return NetworkManager.Singleton.SpawnManager.SpawnedObjects[player].GetComponent<ItemHandler>();
    }
    
    private PickableItem GetItemFromId(ulong item)
    {
        return NetworkManager.Singleton.SpawnManager.SpawnedObjects[item].GetComponent<PickableItem>();
    }
}
