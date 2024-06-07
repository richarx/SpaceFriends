using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : UsableModule
{
    private bool isUsingTable;
    
    public override void UseModule(ModuleHandler moduleHandler)
    {
        isUsingTable = !isUsingTable;

        if (isUsingTable)
            StartUsingTable(moduleHandler);
        else
            StopUsingTable(moduleHandler);
    }
    
    private void StartUsingTable(ModuleHandler moduleHandler)
    {
        moduleHandler.GetComponent<PlayerMovement>().isLocked = true;
        CameraManager.OnRequestCameraSwap.Invoke(CameraManager.CameraState.PilotingShip);
    }

    private void StopUsingTable(ModuleHandler moduleHandler)
    {
        moduleHandler.GetComponent<PlayerMovement>().isLocked = false;
        CameraManager.OnRequestCameraSwap.Invoke(CameraManager.CameraState.InsideShip);
    }
}
