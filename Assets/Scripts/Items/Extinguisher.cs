using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Extinguisher : UsableItem
{
    [SerializeField] private ParticleSystem particles;
    
    public override void UseItem(ItemHandler itemHandler)
    {
        ActivateParticlesRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void ActivateParticlesRpc()
    {
        particles.Play();
    }
}
