using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Airlock : NetworkBehaviour
{
    [SerializeField] private GameObject floor;
    [SerializeField] private Animator staticInsideDoor;
    [SerializeField] private Animator staticOutsideDoor;
    [SerializeField] private Animator movingInsideDoor;
    [SerializeField] private Animator movingOutsideDoor;

    [SerializeField] private Collider2D staticTeleportZone;
    [SerializeField] private Collider2D movingTeleportZone;


    private bool isOpen = false;

    public override void OnNetworkSpawn()
    {
        CloseAirlockRpc();
    }

    public void UseModule(bool isInside)
    {
        UseModuleRpc();
    }
    
    [Rpc(SendTo.Server)]
    private void UseModuleRpc()
    {
        isOpen = !isOpen;
        
        if (isOpen)
            OpenAirlockRpc();
        else
            CloseAirlockRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void OpenAirlockRpc()
    {
        floor.gameObject.SetActive(true);
        
        staticInsideDoor.Play("Door_Open");
        movingInsideDoor.Play("Door_Open");
        
        staticOutsideDoor.Play("Door_Close");
        movingOutsideDoor.Play("Door_Close");

        FromMovingToStaticRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    private void CloseAirlockRpc()
    {
        floor.gameObject.SetActive(false);
        
        staticInsideDoor.Play("Door_Close");
        movingInsideDoor.Play("Door_Close");
        
        staticOutsideDoor.Play("Door_Open");
        movingOutsideDoor.Play("Door_Open");

        FromStaticToMovingRpc();
    }

    [Rpc(SendTo.Server)]
    private void FromStaticToMovingRpc()
    {
        Debug.Log("Zuzu From Static To Moving");
        
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(staticTeleportZone, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("Teleportable"))
            {
                Transform parentTransform = result.transform.parent;
                Vector2 position = parentTransform.position;
                Vector3 relativePosition = position - staticTeleportZone.transform.position.ToVector2();
                Vector2 newPosition = movingTeleportZone.transform.position + relativePosition;
                result.transform.parent.position = newPosition;
                
                if (parentTransform.CompareTag("Player") && parentTransform.GetComponent<NetworkObject>().IsOwner)
                {
                    AttachCameraToPlayer.OnTeleportPlayer?.Invoke(newPosition - position);
                }
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void FromMovingToStaticRpc()
    {
        Debug.Log("Zuzu From Moving To Static");
        
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(movingTeleportZone, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("Teleportable"))
            {
                Transform parentTransform = result.transform.parent;
                Vector2 position = parentTransform.position;
                Vector2 relativePosition = position - movingTeleportZone.transform.position.ToVector2();
                Vector2 newPosition = staticTeleportZone.transform.position.ToVector2() + relativePosition;
                parentTransform.position = newPosition;
                
                if (parentTransform.CompareTag("Player") && parentTransform.GetComponent<NetworkObject>().IsOwner)
                {
                    AttachCameraToPlayer.OnTeleportPlayer?.Invoke(newPosition - position);
                }
            }
        }
    }
}
