using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActivateTable : NetworkBehaviour
{
    [SerializeField] private Collider2D playerCollider;
    
    private bool isUsingTable = false;
    
    private void Update()
    {
        if (!IsOwner)
            return;
        
        if (PlayerInputs.CheckForUseItem() && (isUsingTable || LookForTable()))
            SwapTableStatus();

        if (PlayerInputs.CheckForUseItem())
            CheckForScoopModule();
    }

    private void CheckForScoopModule()
    {
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(playerCollider, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("ScoopModule"))
            {
                result.GetComponent<ActivateScoop>().Activate();
                return;
            }
        }
    }

    private void SwapTableStatus()
    {
        isUsingTable = !isUsingTable;

        if (isUsingTable)
            StartUsingTable();
        else
            StopUsingTable();
    }

    private void StartUsingTable()
    {
        AttachCameraToPlayer.OnRequestCameraFovUpdate?.Invoke(15.0f);
    }

    private void StopUsingTable()
    {
        AttachCameraToPlayer.OnRequestCameraFovUpdate?.Invoke(4.0f);
    }

    private bool LookForTable()
    {
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(playerCollider, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return false;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("Table"))
                return true;
        }

        return false;
    }
}
