using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Extinguisher : UsableItem
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Collider2D foamCollider;
    
    public override void UseItem(ItemHandler itemHandler)
    {
        ActivateParticlesRpc();
        LookForFireRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void ActivateParticlesRpc()
    {
        particles.Play();
    }

    private Coroutine lookForFireCoroutine = null;
    
    [Rpc(SendTo.Server)]
    private void LookForFireRpc()
    {
        if (lookForFireCoroutine != null)
            StopAllCoroutines();

        lookForFireCoroutine = StartCoroutine(LookForFireRoutine());
    }

    private IEnumerator LookForFireRoutine()
    {
        float timer = Time.time;

        while (Time.time - timer < 1.0f)
        {
            GameObject foundModule = LookForModule("Booster", foamCollider);

            if (foundModule != null)
            {
                Booster booster = foundModule.GetComponent<Booster>();
                booster.Extinguish();
                Debug.Log($"Zuzu : Found a Booster, state : {booster.CurrentState}");
                yield break;
            }

            yield return null;
        }
    }
}
