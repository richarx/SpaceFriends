using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    public enum CameraState
    {
        InsideShip,
        OutsideShip,
        PilotingShip
    }
    
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera jetPackCamera;
    [SerializeField] private CinemachineVirtualCamera spaceshipCamera;
    [SerializeField] private GameObject overlayCamera;
    
    public static UnityEvent<CameraState> OnRequestCameraSwap = new UnityEvent<CameraState>();

    private CameraState currentState = CameraState.InsideShip;
    
    private void Awake()
    {
        OnRequestCameraSwap.AddListener(ActivateCamera);
        PlayerMovement.OnPlayerSpawn.AddListener(SetupCameraTargets);
    }

    private void SetupCameraTargets(Transform target)
    {
        //playerCamera.Follow = target;
        jetPackCamera.Follow = target;
    }

    private void ActivateCamera(CameraState state)
    {
        switch (state)
        {
            case CameraState.InsideShip:
                ActivateInsideCamera();
                break;
            case CameraState.OutsideShip:
                ActivateOutsideCamera();
                break;
            case CameraState.PilotingShip:
                ActivatePilotingCamera();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        
        currentState = state;
    }

    private void ActivateInsideCamera()
    {
        if (currentState == CameraState.OutsideShip)
        {
            Vector2 playerPosition = playerCamera.Follow.position.ToVector2();
            Vector2 previousPlayerPosition = jetPackCamera.transform.position.ToVector2();
        
            playerCamera.OnTargetObjectWarped(playerCamera.Follow, playerPosition - previousPlayerPosition);
        }

        overlayCamera.SetActive(true);
        playerCamera.Priority = 10;
        jetPackCamera.Priority = 9;
        spaceshipCamera.Priority = 9;
    }

    private void ActivateOutsideCamera()
    {
        Vector2 playerPosition = playerCamera.Follow.position.ToVector2();
        Vector2 previousPlayerPosition = jetPackCamera.transform.position.ToVector2();
        
        playerCamera.OnTargetObjectWarped(playerCamera.Follow, playerPosition - previousPlayerPosition);
        
        overlayCamera.SetActive(false);
        playerCamera.Priority = 9;
        jetPackCamera.Priority = 10;
        spaceshipCamera.Priority = 9;
    }

    private void ActivatePilotingCamera()
    {
        overlayCamera.SetActive(false);
        playerCamera.Priority = 9;
        jetPackCamera.Priority = 9;
        spaceshipCamera.Priority = 10;
    }
}
