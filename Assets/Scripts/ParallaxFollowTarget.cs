using UnityEngine;

public class ParallaxFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform movingSpaceship;

    private Transform playerTransform;
    
    public bool hasTarget => target != null;
    private Transform target = null;

    private Vector2 offset;

    private void Awake()
    {
        PlayerMovement.OnPlayerSpawn.AddListener((player) => playerTransform = player);
        PlayerTeleport.OnTeleportPlayer.AddListener(SwapTargetOnTeleport);
        
        SetTarget(movingSpaceship);
    }

    private void SwapTargetOnTeleport(bool fromStaticToMoving)
    {
        if (fromStaticToMoving)
            SetTarget(playerTransform);
        else
            SetTarget(movingSpaceship);
    }

    private void SetTarget(Transform newTarget)
    {
        target = newTarget;
        offset = transform.position - newTarget.position;
    }
    
    private void Update()
    {
        if (hasTarget)
            FollowTarget();
    }

    private void FollowTarget()
    {
        transform.position = target.position + offset.ToVector3();
    }
}
