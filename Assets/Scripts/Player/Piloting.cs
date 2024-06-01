using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Piloting : NetworkBehaviour
{
    [SerializeField] private Collider2D playerCollider;

    public static UnityEvent<Piloting> OnUpdatePilotingStatus = new UnityEvent<Piloting>();

    private bool isPiloting = false;
    public bool IsPiloting => isPiloting;
    
    public float speed = 3.0f;
    public float maxSpeed = 6.0f;
    private Vector2 velocity = Vector2.zero;

    private Vector2 inputDirection = Vector2.zero;
    public Vector2 InputDirection => inputDirection;

    private void Update()
    {
        if (!IsOwner)
            return;
        
        if (PlayerInputs.CheckForUseItem() && (isPiloting || LookForPilotingSeat()))
            SwapPilotingStatus();

        if (isPiloting)
            PilotShip();
    }

    private void PilotShip()
    {
        inputDirection = PlayerInputs.ComputeInputDirection();
        velocity += inputDirection.normalized * (speed * Time.deltaTime);
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        SpaceshipSingleton.Instance.SetVelocity(velocity);
    }

    private void SwapPilotingStatus()
    {
        isPiloting = !isPiloting;

        if (isPiloting)
            StartPiloting();
        else
            StopPiloting();
    }
    
    private bool LookForPilotingSeat()
    {
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(playerCollider, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return false;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("PilotSeat"))
                return true;
        }

        return false;
    }
    
    private void StartPiloting()
    {
        AttachCameraToPlayer.OnRequestCameraFovUpdate?.Invoke(15.0f);
        GetComponent<PlayerMovement>().isLocked = true;
        OnUpdatePilotingStatus?.Invoke(this);
    }

    private void StopPiloting()
    {
        AttachCameraToPlayer.OnRequestCameraFovUpdate?.Invoke(4.0f);
        //velocity = Vector2.zero;
        GetComponent<PlayerMovement>().isLocked = false;
        inputDirection = Vector2.zero;
        OnUpdatePilotingStatus?.Invoke(this);
    }
}
