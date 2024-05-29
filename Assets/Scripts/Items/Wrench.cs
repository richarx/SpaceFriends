using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Wrench : UsableItem
{
    [SerializeField] private GameObject graphics;

    private bool isUsingWrench;
    
    public override void UseItem(ItemHandler itemHandler)
    {
        if (isUsingWrench)
            return;

        CalibrationModule calibrationModule = LookForCalibrationModule();

        if (calibrationModule != null)
        {
            bool wasActionPerformed = calibrationModule.UseWrench();
            if (wasActionPerformed)
                HideWrenchRpc();
        }
    }
    
    [Rpc(SendTo.Everyone)]
    private void HideWrenchRpc()
    {
        StartCoroutine(HideWrenchForDuration(0.5f));
    }

    private IEnumerator HideWrenchForDuration(float duration)
    {
        isUsingWrench = true;
        graphics.SetActive(false);
        yield return new WaitForSeconds(duration);
        graphics.SetActive(true);
        isUsingWrench = false;
    }
}
