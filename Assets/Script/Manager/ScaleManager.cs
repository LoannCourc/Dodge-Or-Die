using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    public GameObject player;
    public static ScaleManager Instance;

    private float tileSpacing;
    private float spacing;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    public float GetTileSpacing()
    {
        return tileSpacing;
    }

    public void ScaleObjects(int gridSize)
    {
        tileSpacing = CalculateTileSpacing(gridSize);

        float scaleFactor = CalculateScaleFactor(gridSize);
        float scalePlayer = CalculateScalePlayer(gridSize);
        ScaleGridTiles(scaleFactor);
        ScaleArrows(scaleFactor);
        ScalePlayer(scalePlayer);
    }

    public float CalculateScaleObject(int gridSize)
    {
        float scaleFactor = 1f;
        switch (gridSize)
        {
            case 3: scaleFactor = 0.7f; break;
            case 4: scaleFactor = 0.6f; break;
            case 5: scaleFactor = 0.5f; break;
        }
        return scaleFactor;
    }
    public float CalculateTileSpacing(int gridSize)
    {
        switch (gridSize)
        {
            case 3: spacing = 1f; break;
            case 4: spacing = 0.9f; break;
            case 5: spacing = 0.75f; break;
        }
        tileSpacing = spacing;
        return spacing;
    }
    private float CalculateScaleFactor(int gridSize)
    {
        float scaleFactor = 1f;
        switch (gridSize)
        {
            case 3: scaleFactor = 0.8f; break;
            case 4: scaleFactor = 0.7f; break;
            case 5: scaleFactor = 0.6f; break;
        }
        return scaleFactor;
    }
    public float CalculateScalePlayer(int gridSize)
    {
        float scaleFactor = 1f;
        switch (gridSize)
        {
            case 3: scaleFactor = 0.6f; break;
            case 4: scaleFactor = 0.5f; break;
            case 5: scaleFactor = 0.4f; break;
        }
        return scaleFactor;
    }
    private void ScaleGridTiles(float scaleFactor)
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("GridTile");

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }

    private void ScaleArrows(float scaleFactor)
    {
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("Arrow");
        foreach (GameObject arrow in arrows)
        {
            arrow.transform.localScale = Vector3.one * scaleFactor;
        }
    }

    private void ScalePlayer(float scaleFactor)
    {
        player.transform.localScale = Vector3.one * scaleFactor;
    }
    
    public void ScaleSpawnedObjects(GameObject spawnedObject, float scaleFactor)
    {
        if (spawnedObject != null)
        {
            spawnedObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
        else
        {
            Debug.LogWarning("Tentative de redimensionnement d'un objet null.");
        }
    }

}
