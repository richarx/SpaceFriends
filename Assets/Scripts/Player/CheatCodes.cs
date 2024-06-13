using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CheatCodes : NetworkBehaviour
{
    [HideInInspector] public UnityEvent<float> OnPrimaryCheat = new UnityEvent<float>();
    [HideInInspector] public UnityEvent<float> OnSecondaryCheat = new UnityEvent<float>();
    [HideInInspector] public UnityEvent OnTertiaryCheat = new UnityEvent();
    
    private void Update()
    {
        if (!IsServer)
            return;

        if (PlayerInputs.CheckForTertiaryCheat())
            TriggerTertiaryCheatRpc();

        if (PlayerInputs.CheckForPrimaryCheatUp())
            TriggerPrimaryCheatUpRpc();

        if (PlayerInputs.CheckForPrimaryCheatDown())
            TriggerPrimaryCheatDownRpc();

        if (PlayerInputs.CheckForSecondaryCheatUp())
            TriggerSecondaryCheatUpRpc();

        if (PlayerInputs.CheckForSecondaryCheatDown())
            TriggerSecondaryCheatDownRpc();
    }

    private void TriggerPrimaryCheatUpRpc()
    {
        OnPrimaryCheat?.Invoke(1.0f);
    }
    
    private void TriggerPrimaryCheatDownRpc()
    {
        OnPrimaryCheat?.Invoke(-1.0f);
    }
    
    private void TriggerSecondaryCheatUpRpc()
    {
        OnSecondaryCheat?.Invoke(1.0f);
    }
    
    private void TriggerSecondaryCheatDownRpc()
    {
        OnSecondaryCheat?.Invoke(-1.0f);
    }
    
    private void TriggerTertiaryCheatRpc()
    {
        OnTertiaryCheat?.Invoke();
    }
}
