using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class PlayerTeleport : NetworkBehaviour
{
    [Rpc(SendTo.Everyone)]
    public void TeleportPlayerRpc(bool fromStaticToMoving)
    {
        if (!IsOwner)
            return;

        StartCoroutine(SkipInterpolation());

        if (fromStaticToMoving)
            FromStaticToMoving();
        else
            FromMovingToStatic();
    }

    private void FromStaticToMoving()
    {
        Vector2 position = transform.position;
        Vector2 relativePosition = position - Airlock.Instance.staticPosition;
        Vector2 newPosition = Airlock.Instance.movingPosition + relativePosition;
        transform.position = newPosition.ToVector3();

        GetComponent<PlayerMovement>().isInSpace = true;
        
        AttachCameraToPlayer.OnTeleportPlayer?.Invoke(newPosition - position);
    }
    
    private void FromMovingToStatic()
    {
        Vector2 position = transform.position;
        Vector2 relativePosition = position - Airlock.Instance.movingPosition;
        Vector2 newPosition = Airlock.Instance.staticPosition + relativePosition;
        transform.position = newPosition;
        
        GetComponent<PlayerMovement>().isInSpace = false;
        
        AttachCameraToPlayer.OnTeleportPlayer?.Invoke(newPosition - position);
    }

    private IEnumerator SkipInterpolation()
    {
        ClientNetworkTransform networkTransform = GetComponent<ClientNetworkTransform>();
        networkTransform.Interpolate = false;

        yield return new WaitForSeconds(0.2f);
        
        networkTransform.Interpolate = true;
    }
}
