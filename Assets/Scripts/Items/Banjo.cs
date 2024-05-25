using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banjo : UsableItem
{
    public override void UseItem(ItemHandler itemHandler)
    {
        Debug.Log("Zuzu : Using Banjo !");
        
        itemHandler.gameObject.GetComponentInChildren<PlayerAnimation>().PlayBanjo(this);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
