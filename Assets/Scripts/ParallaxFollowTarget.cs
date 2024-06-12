using UnityEngine;

public class ParallaxFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform movingSpaceship;
    private Transform playerTransform;

    public Rigidbody2D shipRigidbody;
    public Rigidbody2D playerRigidbody;
    
    private bool isShipTarget = true;
    public Vector2 CurrentVelocity => isShipTarget ? shipRigidbody.velocity : playerRigidbody.velocity;
    
    public bool hasTarget => target != null;
    private Transform target = null;

    private Vector2 offset;

    private void Awake()
    {
        PlayerMovement.OnPlayerSpawn.AddListener((player) =>
        {
            playerTransform = player;
            playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();
        });
        PlayerTeleport.OnTeleportPlayer.AddListener(SwapTargetOnTeleport);

        shipRigidbody = movingSpaceship.GetComponent<Rigidbody2D>();
        
        SetShipTarget();
    }

    private void SwapTargetOnTeleport(bool fromStaticToMoving)
    {
        if (fromStaticToMoving)
            SetPlayerTarget();
        else
            SetShipTarget();
    }

    private void SetPlayerTarget()
    {
        isShipTarget = false;
        target = playerTransform;
        offset = transform.position - playerTransform.position;
    }
    
    private void SetShipTarget()
    {
        isShipTarget = true;
        target = movingSpaceship;
        offset = transform.position - movingSpaceship.position;
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
