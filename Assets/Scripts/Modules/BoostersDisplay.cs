using UnityEngine;

public class BoostersDisplay : MonoBehaviour
{
    [SerializeField] private LocalPiloting piloting;

    [SerializeField] private Animator topBooster;
    [SerializeField] private Animator botBooster;
    [SerializeField] private Animator leftBooster;
    [SerializeField] private Animator rightBooster;

    private Vector2 previousDirection;
    
    private void LateUpdate()
    {
        Vector2 direction = piloting.MoveDirection;

        if (direction == previousDirection)
            return;
        
        DisplayBoosterFromDirection(direction);

        previousDirection = direction;
    }

    private void DisplayBoosterFromDirection(Vector2 direction)
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
