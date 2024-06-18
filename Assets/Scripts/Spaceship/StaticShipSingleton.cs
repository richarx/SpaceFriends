using UnityEngine;

public class StaticShipSingleton : MonoBehaviour
{
    public static StaticShipSingleton Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
}
