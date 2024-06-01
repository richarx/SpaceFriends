using UnityEngine;

public class AirlockModule : UsableModule
{
    [SerializeField] private Airlock airlock;
    [SerializeField] private bool isInside;

    public override void UseModule(ModuleHandler moduleHandler)
    {
        airlock.UseModule(isInside);
    }
}
