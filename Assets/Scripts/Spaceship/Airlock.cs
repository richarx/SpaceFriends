using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Airlock : NetworkBehaviour
{
    [SerializeField] private GameObject floor;
    [SerializeField] private Animator staticInsideDoor;
    [SerializeField] private Animator movingInsideDoor;
    [SerializeField] private Animator movingOutsideDoor;

    [SerializeField] private Collider2D staticTeleportZone;
    [SerializeField] private Collider2D movingTeleportZone;

    public static Airlock Instance;

    public Vector2 staticPosition => staticTeleportZone.transform.position.ToVector2();
    public Vector2 movingPosition => movingTeleportZone.transform.position.ToVector2();

    private bool isOpen = false;

    public void Awake()
    {
        Instance = this;
    }

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
        
        movingOutsideDoor.Play("Door_Close");

        if (IsServer)
            TeleportEverythingRpc(false);
    }
    
    [Rpc(SendTo.Everyone)]
    private void CloseAirlockRpc()
    {
        floor.gameObject.SetActive(false);
        
        staticInsideDoor.Play("Door_Close");
        movingInsideDoor.Play("Door_Close");
        
        movingOutsideDoor.Play("Door_Open");

        if (IsServer)
            TeleportEverythingRpc(true);
    }

    [Rpc(SendTo.Server)]
    private void TeleportEverythingRpc(bool fromStaticToMoving)
    {
        List<Collider2D> results = new List<Collider2D>();

        Collider2D teleportZone = fromStaticToMoving ? staticTeleportZone : movingTeleportZone;
        
        int contactCount = Physics2D.OverlapCollider(teleportZone, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("Teleportable"))
            {
                Transform parentTransform = result.transform.parent;
                if (parentTransform.CompareTag("Player"))
                {
                    TeleportPlayer(parentTransform, fromStaticToMoving);
                }
                else
                {
                    TeleportItem(parentTransform, fromStaticToMoving);
                }
            }
        }
    }

    private void TeleportPlayer(Transform parentTransform, bool fromStaticToMoving)
    {
        parentTransform.GetComponent<PlayerTeleport>().TeleportPlayerRpc(fromStaticToMoving);
    }

    private void TeleportItem(Transform parentTransform, bool fromStaticToMoving)
    {
        if (fromStaticToMoving)
        {
            Vector2 position = parentTransform.position;
            Vector3 relativePosition = position - staticTeleportZone.transform.position.ToVector2();
            Vector2 newPosition = movingTeleportZone.transform.position + relativePosition;
            parentTransform.position = newPosition;
        }
        else
        {
            Vector2 position = parentTransform.position;
            Vector2 relativePosition = position - movingTeleportZone.transform.position.ToVector2();
            Vector2 newPosition = staticTeleportZone.transform.position.ToVector2() + relativePosition;
            parentTransform.position = newPosition;
        }
        
        PickableItem pickableItem = parentTransform.GetComponent<PickableItem>();
        if (pickableItem != null)
            pickableItem.SetFloatingVelocityRpc(Vector2.zero);
    }
}
