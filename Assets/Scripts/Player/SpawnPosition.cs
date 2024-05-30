using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    public static SpawnPosition Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    public Vector2 GetSpawnPosition()
    {
        return transform.position.ToVector2();
    }
}
