using System;
using UnityEngine;

[Serializable]
public class ColorToPrefab
{
    public Color32 color;
    public GameObject prefab;
}

public class MapMaker : MonoBehaviour
{
    [SerializeField] private Texture2D mapData;
    [SerializeField] private Texture2D itemData;
    [SerializeField] private ColorToPrefab[] prefabs;
    
    void Start()
    {
        LoadMap();
    }

    private void ClearMap()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void LoadMap()
    {
        ClearMap();

        LoadData(mapData);
        LoadData(itemData, false);
    }

    private void LoadData(Texture2D data, bool reParent = true)
    {
        Color32[] allPixels = data.GetPixels32();
        int width = data.width;
        int height = data.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnTileAt(allPixels[x + (y * width)], x, y, reParent);
            }
        }
    }

    private void SpawnTileAt(Color32 color, int x, int y, bool reParent = true)
    {
        if (color.a <= 155)
            return;

        foreach (ColorToPrefab colorToPrefab in prefabs)
        {
            if (colorToPrefab.color.r == color.r && colorToPrefab.color.g == color.g && colorToPrefab.color.b == color.b)
            {
                GameObject tile = Instantiate(colorToPrefab.prefab, new Vector3(x, y, 0), Quaternion.identity);
                if (reParent)
                    tile.transform.parent = transform;
            }           
        }
    }
}
