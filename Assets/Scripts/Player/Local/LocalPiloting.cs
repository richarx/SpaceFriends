using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LocalPiloting : MonoBehaviour
{
    [SerializeField] private Rigidbody2D movingSpaceship;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Collider2D playerCollider;
    
    private bool isPiloting = false;
    public bool IsPiloting => isPiloting;
    
    public float speed = 3.0f;
    public float maxSpeed = 6.0f;
    private Vector2 velocity = Vector2.zero;

    private Vector2 inputDirection = Vector2.zero;
    public Vector2 InputDirection => inputDirection;
    
    private void Update()
    {
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
    }

    private void FixedUpdate()
    {
        movingSpaceship.MovePosition(movingSpaceship.position + (velocity * Time.fixedDeltaTime));
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
        virtualCamera.m_Lens.OrthographicSize = 15.0f;
    }

    private void StopPiloting()
    {
        virtualCamera.m_Lens.OrthographicSize = 4.0f;
        velocity = Vector2.zero;
        inputDirection = Vector2.zero;
    }
}
