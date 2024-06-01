using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ModuleHandler : NetworkBehaviour
{
    [SerializeField] private Collider2D playerCollider;

    private void Update()
    {
        if (!IsOwner)
            return;
        
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
            if (result.CompareTag("UsableModule"))
            {
                result.GetComponent<UsableModule>().UseModule(this);
            }
        }
    }
}
