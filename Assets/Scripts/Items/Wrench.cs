using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Wrench : UsableItem
{
    [SerializeField] private GameObject graphics;
    [SerializeField] private GameObject useAnimation;

    private Animator attachedAnimator;
    private PickableItem pickableItem;
    
    private bool isUsingWrench;

    private void Awake()
    {
        attachedAnimator = useAnimation.GetComponent<Animator>();
        pickableItem = GetComponent<PickableItem>();
    }

    public override void UseItem(ItemHandler itemHandler)
    {
        if (isUsingWrench)
            return;

        CheckForCalibrationModule();
        
        if (isUsingWrench)
            return;

        CheckForBooster();
    }

    private void CheckForCalibrationModule()
    {
        GameObject foundModule = LookForModule("Module");

        if (foundModule == null)
            return;

        CalibrationModule calibrationModule = foundModule.GetComponent<CalibrationModule>();

        if (calibrationModule != null)
        {
            bool wasActionPerformed = calibrationModule.UseWrench();
            if (wasActionPerformed)
                AnimateWrenchRpc(calibrationModule.wrenchUseTarget.position);
        }
    }
    
    private void CheckForBooster()
    {
        GameObject foundModule = LookForModule("Booster");

        if (foundModule == null)
            return;

        Booster booster = foundModule.GetComponent<Booster>();

        if (booster != null)
        {
            bool wasActionPerformed = booster.Repair();
            if (wasActionPerformed)
                AnimateWrenchRpc(booster.wrenchUseTarget.position);
        }
    }
    
    [Rpc(SendTo.Everyone)]
    private void AnimateWrenchRpc(Vector2 position)
    {
        StartCoroutine(AnimateWrenchForDuration(position,1.0f));
    }

    private IEnumerator AnimateWrenchForDuration(Vector2 position, float duration)
    {
        isUsingWrench = true;
        Vector2 previousPosition = transform.position;
        transform.position = position;
        
        graphics.SetActive(false);
        useAnimation.SetActive(true);
        pickableItem.isBeingUsed = true;

        yield return null;
        
        attachedAnimator.Play("Wrench_Use");

        yield return new WaitForSeconds(duration);
        
        graphics.SetActive(true);
        useAnimation.SetActive(false);
        pickableItem.isBeingUsed = false;
        
        transform.position = previousPosition;
        isUsingWrench = false;
    }
}
