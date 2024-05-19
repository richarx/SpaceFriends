using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MapSwapper : NetworkBehaviour
{
    [SerializeField] private List<GameObject> maps;
    
    private NetworkVariable<int> mapIndex = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    public override void OnNetworkSpawn()
    {
        if (IsClient)
            mapIndex.OnValueChanged += (value, newValue) =>
            {
                Debug.Log($"Map Update : {value} / {newValue}");
                UpdateMap(newValue);
            };
    }

    private void Update()
    {
        if (IsServer && PlayerInputs.CheckForSwapMap())
        {
            Debug.Log("Zuzu : SWAP MAP");
            
            if (mapIndex.Value < maps.Count - 1)
            {
                mapIndex.Value += 1;
                //UpdateMap(mapIndex.Value);
            }
        }
    }

    private void UpdateMap(int index)
    {
        for (int i = 0; i < maps.Count; i++)
        {
            maps[i].SetActive(i == index);
        }
    }
}
