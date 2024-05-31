using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LocalPiloting : MonoBehaviour
{
    [SerializeField] private Transform movingSpaceship;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Collider2D playerCollider;
    
    private bool isPiloting = false;
    public bool IsPiloting => isPiloting;
    
    private float speed = 10.0f;
    private Vector2 velocity = Vector2.zero;

    public Vector2 MoveDirection => velocity;
    
    private void Update()
    {
        if (PlayerInputs.CheckForUseItem() && (isPiloting || LookForPilotingSeat()))
            SwapPilotingStatus();

        if (isPiloting)
            PilotShip();
    }

    private void PilotShip()
    {
        Vector2 inputDirection = PlayerInputs.ComputeInputDirection();
        velocity = inputDirection.normalized;
        movingSpaceship.position += (velocity * (speed * Time.deltaTime)).ToVector3();
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
    }
}
