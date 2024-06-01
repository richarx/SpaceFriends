using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ActivateScoop : UsableModule
{
    [SerializeField] private GameObject scoopObject;

    private bool isDeployed = false;
    
    public override void UseModule(ModuleHandler moduleHandler)
    {
        if (isDeployed)
            return;

        ActivateRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void ActivateRpc()
    {
        if (isDeployed)
            return;
        
        isDeployed = true;
        StartCoroutine(DeployScoop());
    }

    private IEnumerator DeployScoop()
    {
        while (scoopObject.transform.localPosition.y < 0.5f)
        {
            Vector3 newPosition = scoopObject.transform.localPosition;
            newPosition.y += 0.1f * Time.deltaTime;
            scoopObject.transform.localPosition = newPosition;
            yield return null;
        }
    }
}
