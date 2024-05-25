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
            calibrationModule.UseWrench();
    }
}
