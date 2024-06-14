using System;
using Unity.Netcode;
using UnityEngine;

public enum BoosterState
{
    Clean,
    Normal,
    Broken,
    Burning,
    BurningSmall
}

public class Booster : NetworkBehaviour
{
    [SerializeField] public Transform wrenchUseTarget;
    
    private Animator attachedAnimator;
    
    public BoosterState CurrentState => boosterState;
    private BoosterState boosterState;
    
    private float cleanTimer;
    private float cleanCooldown = 10.0f;

    private float totalFire = 5.0f;
    private float fireReduction = 2.5f;

    private float currentFireLevel;

    private int currentBreakLevel = -1;

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

    [Rpc(SendTo.Server)]
    public void SetBoosterStateRpc(BoosterState newState)
    {
        if (boosterState == newState && newState != BoosterState.Broken)
            return;
        
        switch (newState)
        {
            case BoosterState.Clean:
                cleanTimer = Time.time;
                RepairBoosterRpc(newState);
                break;
            case BoosterState.Normal:
                RepairBoosterRpc(newState);
                break;
            case BoosterState.Broken:
                BreakBoosterRpc(currentBreakLevel);
                break;
            case BoosterState.Burning:
                BurnBoosterRpc(false);
                break;
            case BoosterState.BurningSmall:
                BurnBoosterRpc(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        boosterState = newState;
    }

    [Rpc(SendTo.Everyone)]
    private void RepairBoosterRpc(BoosterState newState)
    {
        currentBreakLevel = -1;
        attachedAnimator.Play("Booster_Idle");
        boosterState = newState;
    }
    
    [Rpc(SendTo.Everyone)]
    private void BreakBoosterRpc(int currentBreaking)
    {
        Debug.Log($"Zuzu : BreakBoosterRpc : {currentBreaking}");
        currentBreaking = Math.Clamp(currentBreaking, 0, 3);
        currentBreakLevel = currentBreaking;
        attachedAnimator.Play($"Booster_Broken_{currentBreaking}");
        boosterState = BoosterState.Broken;
    }
    
    [Rpc(SendTo.Everyone)]
    private void BurnBoosterRpc(bool small)
    {
        currentFireLevel = totalFire;
        attachedAnimator.Play(small ? "Booster_Burning_Small" : "Booster_Burning");
        boosterState = small ? BoosterState.BurningSmall : BoosterState.Burning;
    }

    [Rpc(SendTo.Server)]
    public void ExtinguishRpc()
    {
        Debug.Log("Zuzu ExtinguishRpc");
        
        if (boosterState != BoosterState.Burning && boosterState != BoosterState.BurningSmall)
            return;

        currentFireLevel -= fireReduction * Time.deltaTime;
        
        Debug.Log($"Zuzu fire level : {currentFireLevel}");

        if (currentFireLevel <= 0.0f)
        {
            BreakRpc();
            return;
        }
        
        if (boosterState != BoosterState.BurningSmall && currentFireLevel < totalFire / 2.0f)
            SetBoosterStateRpc(BoosterState.BurningSmall);
    }

    public bool Repair()
    {
        if (boosterState != BoosterState.Broken)
            return false;
        
        bool isRepaired = currentBreakLevel >= 0;
        
        RepairRpc();

        return isRepaired;
    }
    
    [Rpc(SendTo.Server)]
    private void RepairRpc()
    {
        currentBreakLevel -= 1;
        
        if (currentBreakLevel < 0)
            SetBoosterStateRpc(BoosterState.Clean);
        else
            SetBoosterStateRpc(BoosterState.Broken);
    }
    
    [Rpc(SendTo.Server)]
    public void BreakRpc()
    {
        if (boosterState == BoosterState.Broken)
            return;

        currentBreakLevel = 3;
        
        SetBoosterStateRpc(BoosterState.Broken);
    }
}
