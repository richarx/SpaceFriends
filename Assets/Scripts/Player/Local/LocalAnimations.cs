using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAnimations : MonoBehaviour
{
    [SerializeField] private LocalMovement playerMovement;
    [SerializeField] private Animator animator;
    
    private string previousAnimation;
    
    private void LateUpdate()
    {
        Vector2 direction = playerMovement.MovementDirection;

        string targetAnimation = ComputeAnimation(direction, previousAnimation);

        bool isFirstAnimationEver = string.IsNullOrEmpty(previousAnimation);
        bool isANewAnimation = !isFirstAnimationEver && !targetAnimation.Equals(previousAnimation);
        
        if (isFirstAnimationEver || isANewAnimation)
            animator.Play(targetAnimation);

        previousAnimation = targetAnimation;
    }
    
    private static string ComputeAnimation(Vector2 direction, string previousAnimation)
    {
        if (direction.magnitude < 0.5f)
            return ComputeIdleAnimation(previousAnimation);

        return ComputeWalkAnimation_4(direction);
    }
    
    private static string ComputeWalkAnimation_4(Vector2 direction)
    {
        if (direction.x > 0.5f && direction.y > 0.5f)
            return "Walk_B";
        if (direction.x > 0.5f && direction.y < -0.5f)
            return "Walk_F";
        if (direction.x < -0.5f && direction.y > 0.5f)
            return "Walk_B";
        if (direction.x < -0.5f && direction.y < -0.5f)
            return "Walk_F";
        if (direction.x > 0.5f && direction.y < 0.5f && direction.y > -0.5f)
            return "Walk_R";
        if (direction.x < -0.5f && direction.y < 0.5f && direction.y > -0.5f)
            return "Walk_L";
        if (direction.x < 0.5f && direction.x > -0.5f && direction.y > 0.5f)
            return "Walk_B";
        if (direction.x < 0.5f && direction.x > -0.5f && direction.y < -0.5f)
            return "Walk_F";

        return "Idle_F";
    }
    
    private static string ComputeIdleAnimation(string previousAnimation)
    {
        if (string.IsNullOrEmpty(previousAnimation))
            return "Idle_F";
        
        if (previousAnimation == "Walk_BR")
            return "Idle_BR";
        if (previousAnimation == "Walk_FR")
            return "Idle_FR";
        if (previousAnimation == "Walk_R")
            return "Idle_R";
        if (previousAnimation == "Walk_BL")
            return "Idle_BL";
        if (previousAnimation == "Walk_FL")
            return "Idle_FL";
        if (previousAnimation == "Walk_L")
            return "Idle_L";
        if (previousAnimation == "Walk_F")
            return "Idle_F";
        if (previousAnimation == "Walk_B")
            return "Idle_B";

        return previousAnimation;
    }
}
