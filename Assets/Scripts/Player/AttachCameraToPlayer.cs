using System;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class AttachCameraToPlayer : NetworkBehaviour
{
    public static UnityEvent<float> OnRequestCameraFovUpdate = new UnityEvent<float>();
    public static UnityEvent<Vector2> OnTeleportPlayer = new UnityEvent<Vector2>();
    public static UnityEvent<Transform> OnRequestAttachToPlayer = new UnityEvent<Transform>();
    public static UnityEvent OnRequestAttachToMovingShip = new UnityEvent();

    private CinemachineVirtualCamera virtualCamera;
    
    private NetworkVariable<float> zoom = new NetworkVariable<float>(
        7,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    private void Awake()
    {
        PlayerMovement.OnPlayerSpawn.AddListener(AttachCamera);
        OnRequestAttachToPlayer.AddListener(SwapToPlayerCamera);
        OnRequestAttachToMovingShip.AddListener(AttachToMovingShip);
        OnTeleportPlayer.AddListener(MoveCamera);
        OnRequestCameraFovUpdate.AddListener(UpdateCameraFov);
    }

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsClient)
            zoom.OnValueChanged += (value, newValue) =>
            {
                Debug.Log($"Zooooom : {value} / {newValue}");
                virtualCamera.m_Lens.OrthographicSize = newValue;
            };
    }

    private void Update()
    {
        if (IsServer && PlayerInputs.CheckForZoomIncrease())
        {
            zoom.Value -= 0.5f;
            zoom.Value = Mathf.Max(zoom.Value, 0.5f);
            virtualCamera.m_Lens.OrthographicSize = zoom.Value;
        }

        if (IsServer && PlayerInputs.CheckForZoomDecrease())
        {
            zoom.Value += 0.5f;
            virtualCamera.m_Lens.OrthographicSize = zoom.Value;
        }
    }

    private void MoveCamera(Vector2 direction)
    {
        Debug.Log($"Zuzu : MoveCamera : direction : {direction} / finalPosition : {transform.position + direction.ToVector3()}");
        virtualCamera.OnTargetObjectWarped(virtualCamera.Follow, direction);
    }
    
    private void UpdateCameraFov(float size)
    {
        virtualCamera.m_Lens.OrthographicSize = size;
    }
    
    private void AttachToMovingShip()
    {
        virtualCamera.Priority = 8;
    }

    private void SwapToPlayerCamera(Transform target)
    {
        virtualCamera.Priority = 10;
    }

    private void AttachCamera(Transform target)
    {
        virtualCamera.Follow = target;
    }
}
