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
    
    protected CalibrationModule LookForCalibrationModule()
    {
        List<Collider2D> results = new List<Collider2D>();

        int contactCount = Physics2D.OverlapCollider(itemTriggerZone, new ContactFilter2D().NoFilter(), results);

        if (contactCount < 1)
            return null;

        foreach (Collider2D result in results)
        {
            if (result.CompareTag("Module"))
            {
                CalibrationModule item = result.GetComponent<CalibrationModule>();
                if (item != null)
                    return item;
            }
        }

        return null;
    }
}
