using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DummyCameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private Transform staticShip;
    [SerializeField] private Transform movingShip;
    
    private void LateUpdate()
    {
        Vector2 offset = playerCamera.transform.position - movingShip.position;

        transform.position = staticShip.position + offset.ToVector3(-80.0f);
    }
}
