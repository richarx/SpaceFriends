using System.Collections;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTeleport : NetworkBehaviour
{
    public static UnityEvent<bool> OnTeleportPlayer = new UnityEvent<bool>();

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
        
        OnTeleportPlayer?.Invoke(fromStaticToMoving);
    }

    private void FromStaticToMoving()
    {
        Vector2 position = transform.position;
        Vector2 relativePosition = position - Airlock.Instance.staticPosition;
        Vector2 newPosition = Airlock.Instance.movingPosition + relativePosition;
        transform.position = newPosition.ToVector3();

        GetComponent<PlayerMovement>().SetInSpaceStatusRpc(true);
        
        CameraManager.OnRequestCameraSwap.Invoke(CameraManager.CameraState.OutsideShip);
    }
    
    private void FromMovingToStatic()
    {
        Vector2 position = transform.position;
        Vector2 relativePosition = position - Airlock.Instance.movingPosition;
        Vector2 newPosition = Airlock.Instance.staticPosition + relativePosition;
        transform.position = newPosition;
        
        GetComponent<PlayerMovement>().SetInSpaceStatusRpc(false);
        
        CameraManager.OnRequestCameraSwap.Invoke(CameraManager.CameraState.InsideShip);
    }

    private IEnumerator SkipInterpolation()
    {
        ClientNetworkTransform networkTransform = GetComponent<ClientNetworkTransform>();
        networkTransform.Interpolate = false;

        yield return new WaitForSeconds(0.2f);
        
        networkTransform.Interpolate = true;
    }
}
