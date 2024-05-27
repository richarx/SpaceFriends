using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BlowTorch : UsableItem
{
    [SerializeField] private GameObject torchFlame;
    
    private bool isUsingTorch;

    public override void UseItem(ItemHandler itemHandler)
    {
        if (isUsingTorch)
            return;

        CalibrationModule calibrationModule = LookForCalibrationModule();

        if (calibrationModule != null)
        {
            bool wasActionPerformed = calibrationModule.UseTorch();
            if (wasActionPerformed)
                ActivateFlameRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void ActivateFlameRpc()
    {
        StartCoroutine(ActivateFlameForDuration(1.0f));
    }
    
    private IEnumerator ActivateFlameForDuration(float duration)
    {
        isUsingTorch = true;
        torchFlame.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        isUsingTorch = false;
        yield return new WaitForSeconds(duration - 0.25f);
        torchFlame.SetActive(false);
    }
}