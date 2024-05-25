using System;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class AttachCameraToPlayer : NetworkBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    
    private NetworkVariable<float> zoom = new NetworkVariable<float>(
        7,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    private void Awake()
    {
        PlayerMovement.OnPlayerSpawn.AddListener(AttachCamera);
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

    private void AttachCamera(Transform target)
    {
        virtualCamera.Follow = target;
    }
}
