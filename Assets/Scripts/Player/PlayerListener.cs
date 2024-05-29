using Unity.Netcode;
using UnityEngine;

public class PlayerListener : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            gameObject.AddComponent<AudioListener>();
    }
}
