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
    [SerializeField] private DummyPlayer dummyPlayer;
    
    public static UnityEvent<CameraState> OnRequestCameraSwap = new UnityEvent<CameraState>();

    private CameraState currentState = CameraState.InsideShip;

    private Transform playerTransform;
    
    private void Awake()
    {
        OnRequestCameraSwap.AddListener(ActivateCamera);
        PlayerMovement.OnPlayerSpawn.AddListener(SetupCameraTargets);
    }

    private void SetupCameraTargets(Transform target)
    {
        //playerCamera.Follow = target;
        playerTransform = target;
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
        Vector2 playerPosition = playerCamera.Follow.position.ToVector2();
        Vector2 previousPlayerPosition = jetPackCamera.transform.position.ToVector2();
        
        jetPackCamera.OnTargetObjectWarped(jetPackCamera.Follow, playerPosition - previousPlayerPosition);
        jetPackCamera.Follow = null;
        
        overlayCamera.SetActive(true);
        playerCamera.Priority = 10;
        jetPackCamera.Priority = 9;
        spaceshipCamera.Priority = 9;
        
        dummyPlayer.isFlying = false;
    }

    private void ActivateOutsideCamera()
    {
        dummyPlayer.isFlying = true;
        jetPackCamera.Follow = playerTransform;
        
        Vector2 playerPosition = playerCamera.Follow.position.ToVector2();
        Vector2 previousPlayerPosition = jetPackCamera.transform.position.ToVector2();
        
        jetPackCamera.OnTargetObjectWarped(jetPackCamera.Follow,  previousPlayerPosition - playerPosition);

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
