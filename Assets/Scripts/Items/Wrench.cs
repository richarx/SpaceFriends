using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrench : UsableItem
{
    public override void UseItem(ItemHandler itemHandler)
    {
        Debug.Log("Zuzu : Using Wrench !");
        
        CalibrationModule calibrationModule = LookForCalibrationModule();

        if (calibrationModule != null)
        {
            bool wasActionPerformed = calibrationModule.UseWrench();
            if (wasActionPerformed)
                StartCoroutine(HideWrenchForDuration(0.5f));
        }
    }

    private IEnumerator HideWrenchForDuration(float duration)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(duration);
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
