using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PingHandler : NetworkBehaviour
{
    public static UnityEvent<Vector2> PingBeaconPosition = new UnityEvent<Vector2>();
    public static UnityEvent<Vector2> PingSpaceshipPosition = new UnityEvent<Vector2>();

    private DisplayPing localPlayer;
    
    private void Awake()
    {
        PlayerMovement.OnPlayerSpawn.AddListener((t) => localPlayer = t.GetComponent<DisplayPing>());
        PingBeaconPosition.AddListener((p) => SpreadPingToAllPlayersRpc(p, true));
    }

    [Rpc(SendTo.NotMe)]
    private void SpreadPingToAllPlayersRpc(Vector2 position, bool isBeacon)
    {
        localPlayer.ComputeAndDisplayPing(position, isBeacon);
    }
}
