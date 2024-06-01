using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Airlock : NetworkBehaviour
{
    public void UseModule(bool isInside)
    {
        UseModuleRpc(isInside);
    }
    
    [Rpc(SendTo.Server)]
    private void UseModuleRpc(bool isInside)
    {
        
    }

    private void FromStaticToMoving()
    {
        Debug.Log("Zuzu From Static To Moving");
    }

    private void FromMovingToStatic()
    {
        Debug.Log("Zuzu From Moving To Static");
    }
}
