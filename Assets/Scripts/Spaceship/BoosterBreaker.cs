using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class BoosterBreaker : NetworkBehaviour
{
    [SerializeField] private List<Booster> boosters;

    private float timer = 0.0f;
    private float coolDown = 20.0f;

    private bool hasGameStarted = false;
    
    public override void OnNetworkSpawn()
    {
        timer = Time.time - 15.0f;
    }
    
    private void LateUpdate()
    {
        if (!IsServer)
            return;

        if (!hasGameStarted)
        {
            if (PlayerInputs.CheckForStartGame())
                hasGameStarted = true;
            return;
        }
        
        if (Time.time - timer > coolDown)
        {
            BreakRandomModule();
        }
    }

    private void BreakRandomModule()
    {
        List<Booster> normalBoosters = boosters.Where((b) => b.CurrentState == BoosterState.Normal).ToList();

        if (normalBoosters.Count < 1)
        {
            if (boosters.Count(b => b.CurrentState == BoosterState.Broken || b.CurrentState == BoosterState.Burning) > 3)
                Debug.Log("Zuzu : No more boosters to break -> YOU LOOSE !");
            return;
        }
        
        timer = Time.time;
        
        normalBoosters.GetRandomElements().First().SetBoosterStateRpc(
            Tools.RandomBool() ?
            BoosterState.Burning :
            BoosterState.Broken
        );
    }
}
