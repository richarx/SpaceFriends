using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BlowTorch : UsableItem
{
    [SerializeField] private GameObject torchFlame;

    private bool isUsingTorch;

    private Coroutine flameRoutine = null;

    public override void UseItem(ItemHandler itemHandler)
    {
        if (isUsingTorch)
            return;

        GameObject foundModule = LookForModule("Module");

        if (foundModule == null)
            return;

        CalibrationModule calibrationModule = foundModule.GetComponent<CalibrationModule>();

        bool wasActionPerformed = false;

        if (calibrationModule != null)
            wasActionPerformed = calibrationModule.UseTorch();

        ActivateFlameRpc(wasActionPerformed);
    }

    [Rpc(SendTo.Everyone)]
    private void ActivateFlameRpc(bool wasActionPerformed)
    {
        if (flameRoutine != null)
            StopAllCoroutines();

        flameRoutine = StartCoroutine(ActivateFlameForDuration(1.0f, wasActionPerformed));
    }
    
    private IEnumerator ActivateFlameForDuration(float duration, bool lockTorch)
    {
        torchFlame.SetActive(true);
        if (lockTorch)
        {
            isUsingTorch = true;
            yield return new WaitForSeconds(0.25f);
            isUsingTorch = false;
        }
        yield return new WaitForSeconds(duration - 0.25f);
        torchFlame.SetActive(false);
    }
}