using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroyItemOnClick : NetworkBehaviour
{ 
    private void Update()
    {
        if (IsServer && PlayerInputs.CheckForSwapMap())
            Destroy(gameObject);
    }
}
