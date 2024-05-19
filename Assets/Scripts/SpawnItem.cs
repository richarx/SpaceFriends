using Unity.Netcode;
using UnityEngine;

public class SpawnItem : NetworkBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    void Start()
    {
        if (IsClient)
            return;
        
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            GameObject spawnedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            spawnedItem.GetComponent<NetworkObject>().Spawn(true);
        };
    }
}
