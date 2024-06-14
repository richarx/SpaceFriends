using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public enum BulbState
{
    Green,
    Orange,
    Red
}

public class BoosterDisplay : NetworkBehaviour
{
    [SerializeField] private BoosterBreaker boosterBreaker;
    [SerializeField] private List<SpriteRenderer> bulbsRenderers;
    [SerializeField] private Sprite green;
    [SerializeField] private Sprite orange;
    [SerializeField] private Sprite red;
    
    private List<BulbState> bulbsStates = new List<BulbState>();

    private bool isSetup = false;
    
    public override void OnNetworkSpawn()
    {
        for (int i = 0; i < 4; i++)
        {
            bulbsStates.Add(BulbState.Green);
        }

        isSetup = true;
    }
    
    private void Update()
    {
        if (!IsServer || !isSetup)
            return;
        
        if (!CheckCurrentList())
            UpdateBulbsRenderersRpc(bulbsStates[0], bulbsStates[1], bulbsStates[2], bulbsStates[3]);
    }

    private bool CheckCurrentList()
    {
        bool isUpToDate = true;

        for (int i = 0; i < boosterBreaker.BoosterList.Count; i++)
        {
            Booster booster = boosterBreaker.BoosterList[i];
            BulbState currentColor = GetColorFromBooster(booster.CurrentState);
            if (bulbsStates[i] != currentColor)
            {
                isUpToDate = false;
                bulbsStates[i] = currentColor;
            }
        }

        return isUpToDate;
    }
    
    [Rpc(SendTo.Everyone)]
    private void UpdateBulbsRenderersRpc(BulbState stateOne, BulbState stateTwo, BulbState stateThree, BulbState stateFour)
    {
        bulbsRenderers[0].sprite = GetSpriteFromColor(stateOne);
        bulbsRenderers[1].sprite = GetSpriteFromColor(stateTwo);
        bulbsRenderers[2].sprite = GetSpriteFromColor(stateThree);
        bulbsRenderers[3].sprite = GetSpriteFromColor(stateFour);
    }
    
    private BulbState GetColorFromBooster(BoosterState booster)
    {
        switch (booster)
        {
            case BoosterState.Clean:
                return BulbState.Green;
            case BoosterState.Normal:
                return BulbState.Green;
            case BoosterState.Broken:
                return BulbState.Orange;
            case BoosterState.Burning:
                return BulbState.Red;
            case BoosterState.BurningSmall:
                return BulbState.Red;
            default:
                throw new ArgumentOutOfRangeException(nameof(booster), booster, null);
        }
    }

    private Sprite GetSpriteFromColor(BulbState color)
    {
        switch (color)
        {
            case BulbState.Green:
                return green;
            case BulbState.Orange:
                return orange;
            case BulbState.Red:
                return red;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }
}
