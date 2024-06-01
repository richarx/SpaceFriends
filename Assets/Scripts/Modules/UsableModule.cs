using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UsableModule : NetworkBehaviour
{
    public virtual void UseModule(ModuleHandler moduleHandler)
    {
    }
}
