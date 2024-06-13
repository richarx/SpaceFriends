using System;
using Unity.Netcode;
using UnityEngine;

public enum BoosterState
{
    Clean,
    Normal,
    Broken,
    Burning
}

public class Booster : NetworkBehaviour
{
    [SerializeField] public Transform wrenchUseTarget;
    
    private Animator attachedAnimator;
    
    public BoosterState CurrentState => boosterState;
    private BoosterState boosterState;
    
    private float cleanTimer;
    private float cleanCooldown = 10.0f;

    private void Awake()
    {
        boosterState = BoosterState.Normal;
        attachedAnimator = GetComponent<Animator>();
        cleanTimer = Time.time;
    }

    private void Update()
    {
        if (!IsServer)
            return;
        
        if (boosterState == BoosterState.Clean && Time.time - cleanTimer > cleanCooldown)
            SetBoosterStateRpc(BoosterState.Normal);
    }

    [Rpc(SendTo.Everyone)]
    public void SetBoosterStateRpc(BoosterState newState)
    {
        if (boosterState == newState)
            return;
        
        switch (newState)
        {
            case BoosterState.Clean:
                cleanTimer = Time.time;
                RepairBooster();
                break;
            case BoosterState.Normal:
                RepairBooster();
                break;
            case BoosterState.Broken:
                BreakBooster();
                break;
            case BoosterState.Burning:
                BurnBooster();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        boosterState = newState;
    }

    private void RepairBooster()
    {
        attachedAnimator.Play("Booster_Idle");
    }
    
    private void BreakBooster()
    {
        attachedAnimator.Play("Booster_Broken_3");
    }
    
    private void BurnBooster()
    {
        attachedAnimator.Play("Booster_Burning");
    }

    public void Extinguish()
    {
        if (boosterState != BoosterState.Burning)
            return;
        
        SetBoosterStateRpc(BoosterState.Broken);
    }
    
    public bool Repair()
    {
        if (boosterState != BoosterState.Broken)
            return false;
        
        SetBoosterStateRpc(BoosterState.Clean);
        return true;
    }
}
