using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UsableItem : NetworkBehaviour
{
    private Collider2D itemTriggerZone; 
    
    public override void OnNetworkSpawn()
    {
        itemTriggerZone = GetComponent<Collider2D>();
    }
    
    public virtual void UseItem(ItemHandler itemHandler)
    {
        Debug.Log("Zuzu : Calling UseItem on parent class");
    }
    
    protected GameObject LookForModule(string targetTag, Collider2D trigger = null)
    {
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(trigger != null ? trigger : itemTriggerZone, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return null;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag(targetTag))
            {
                return result.gameObject;
            }
        }

        return null;
    }
}
