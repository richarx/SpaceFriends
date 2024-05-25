using System;
using Unity.Netcode;
using UnityEngine;

public class SpawnItem : NetworkBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            SpawnNewItem();
    }

    private void SpawnNewItem()
    {
        GameObject spawnedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        spawnedItem.GetComponent<NetworkObject>().Spawn(true);
    }
}
