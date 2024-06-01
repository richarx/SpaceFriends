using Unity.Netcode;
using UnityEngine;

public class BoostersDisplay : NetworkBehaviour
{
    [SerializeField] private Animator topBooster;
    [SerializeField] private Animator botBooster;
    [SerializeField] private Animator leftBooster;
    [SerializeField] private Animator rightBooster;

    private Piloting piloting = null;
    
    private Vector2 previousDirection;

    public override void OnNetworkSpawn()
    {
       Piloting.OnUpdatePilotingStatus.AddListener((p) => { piloting = p; });
    }

    private void LateUpdate()
    {
        if (piloting == null || !piloting.IsPiloting)
           return;
       
        Vector2 direction = piloting.InputDirection;

        if (direction == previousDirection)
            return;
        
        DisplayBoosterFromDirectionRpc(direction);

        previousDirection = direction;
    }

    [Rpc(SendTo.Everyone)]
    private void DisplayBoosterFromDirectionRpc(Vector2 direction)
    {
        if (direction.x > 0.0f)
           leftBooster.Play("Booster_Selected");
        else
           leftBooster.Play("Booster_Idle");
       
        if (direction.x < 0.0f)
           rightBooster.Play("Booster_Selected");
        else
           rightBooster.Play("Booster_Idle");
       
        if (direction.y > 0.0f)
           botBooster.Play("Booster_Selected");
        else
           botBooster.Play("Booster_Idle");
       
        if (direction.y < 0.0f)
           topBooster.Play("Booster_Selected");
        else
           topBooster.Play("Booster_Idle");
    }
}
