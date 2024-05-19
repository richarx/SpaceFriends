using Cinemachine;
using UnityEngine;

public class AttachCameraToPlayer : MonoBehaviour
{
    void Awake()
    {
        PlayerMovement.OnPlayerSpawn.AddListener(AttachCamera);
    }

    private void AttachCamera(Transform target)
    {
        GetComponent<CinemachineVirtualCamera>().Follow = target;
    }
}
