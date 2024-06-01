using System;
using Unity.Netcode;
using UnityEngine;

public class SpaceshipSingleton : NetworkBehaviour
{
    public static SpaceshipSingleton Instance;

    private Rigidbody2D attachedRigidbody2D;

    private Vector2 velocity;
    
    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            attachedRigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        SetVelocityRpc(newVelocity);
    }

    [Rpc(SendTo.Server)]
    private void SetVelocityRpc(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }

    private void FixedUpdate()
    {
        if (IsServer)
            attachedRigidbody2D.MovePosition(attachedRigidbody2D.position + (velocity * Time.fixedDeltaTime));
    }
}
