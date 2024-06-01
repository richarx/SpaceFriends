using Unity.Netcode;
using UnityEngine;

public class ActivateScoop : NetworkBehaviour
{
    [SerializeField] private GameObject scoopObject;

    public void Activate()
    {
        ActivateRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void ActivateRpc()
    {
        Vector3 pos = scoopObject.transform.localPosition;
        scoopObject.transform.localPosition = new Vector3(pos.x, 0.5f, 0.0f);
    }
}
