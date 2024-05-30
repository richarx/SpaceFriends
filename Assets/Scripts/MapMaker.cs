using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class ColorToPrefab
{
    public Color32 color;
    public GameObject prefab;
}

public class MapMaker : NetworkBehaviour
{
    [SerializeField] private Texture2D mapData;
    [SerializeField] private Texture2D itemData;
    [SerializeField] private ColorToPrefab[] prefabs;
    [SerializeField] private GameObject[] tools;

    public override void OnNetworkSpawn()
    {
        LoadMap(mapData);

        if (IsServer)
            LoadItems(itemData);
    }

    private void ClearMap()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void LoadMap(Texture2D data)
    {
        Color32[] allPixels = data.GetPixels32();
        int width = data.width;
        int height = data.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnTileAt(allPixels[x + (y * width)], x, y);
            }
        }
    }

    private void LoadItems(Texture2D data)
    {
        Color32[] allPixels = data.GetPixels32();
        int width = data.width;
        int height = data.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnItemAt(allPixels[x + (y * width)], x, y);
            }
        }
    }

    private void SpawnTileAt(Color32 color, int x, int y)
    {
        if (color.a <= 155)
            return;

        foreach (ColorToPrefab colorToPrefab in prefabs)
        {
            if (colorToPrefab.color.r == color.r && colorToPrefab.color.g == color.g && colorToPrefab.color.b == color.b)
            {
                GameObject tile = Instantiate(colorToPrefab.prefab, new Vector3(x, y, 0), Quaternion.identity);
                tile.transform.parent = transform;
            }           
        }
    }
    
    private void SpawnItemAt(Color32 color, int x, int y)
    {
        if (color.a <= 155)
            return;

        if (color.r == 0 && color.g == 0 && color.b == 0)
        {
            SpawnToolAt(x, y);
            return;
        }
        
        foreach (ColorToPrefab colorToPrefab in prefabs)
        {
            if (colorToPrefab.color.r == color.r && colorToPrefab.color.g == color.g && colorToPrefab.color.b == color.b)
            {
                GameObject tile = Instantiate(colorToPrefab.prefab, new Vector3(x, y, 0), Quaternion.identity);
                tile.GetComponent<NetworkObject>()?.Spawn(true);
            }           
        }
    }

    private int toolSpawnedCount = 0;
    private void SpawnToolAt(int x, int y)
    {
        GameObject tool = Instantiate(tools[toolSpawnedCount], new Vector3(x, y, 0), Quaternion.identity);
        tool.GetComponent<NetworkObject>().Spawn(true);
        toolSpawnedCount += 1;
    }
}
