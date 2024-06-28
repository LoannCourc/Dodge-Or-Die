using UnityEngine;
using UnityEngine.Serialization;

public class ScaleManager : MonoBehaviour
{
    public GameObject player;
    
    [Header("Scale Factors")]
    [Tooltip("Scale factors for different grid sizes.")]
    public float scale3x3 = 0.7f;
    public float scale4x4 = 0.6f;
    public float scale5x5 = 0.5f;

    [FormerlySerializedAs("spacing3x3")]
    [Header("Tile Spacing")]
    [Tooltip("Tile spacing for different grid sizes.")]
    public float tileSpacing3x3 = 1f;
    public float tileSpacing4x4 = 0.9f;
    public float tileSpacing5x5 = 0.75f;

    [Header("General Scale Factors")]
    [Tooltip("General scale factors for different grid sizes.")]
    public float generalScale3x3 = 0.8f;
    public float generalScale4x4 = 0.7f;
    public float generalScale5x5 = 0.6f;

    [Header("Player Scale Factors")]
    [Tooltip("Scale factors for the player at different grid sizes.")]
    public float playerScale3x3 = 0.6f;
    public float playerScale4x4 = 0.5f;
    public float playerScale5x5 = 0.4f;
    
    
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
        switch (gridSize)
        {
            case 3: return scale3x3;
            case 4: return scale4x4;
            case 5: return scale5x5;
            default: return 1f;  // Default scale factor
        }
    }

    public float CalculateTileSpacing(int gridSize)
    {
        switch (gridSize)
        {
            case 3: return tileSpacing3x3;
            case 4: return tileSpacing4x4;
            case 5: return tileSpacing5x5;
            default: return 1f;  // Default spacing
        }
    }

    private float CalculateScaleFactor(int gridSize)
    {
        switch (gridSize)
        {
            case 3: return generalScale3x3;
            case 4: return generalScale4x4;
            case 5: return generalScale5x5;
            default: return 1f;  // Default general scale factor
        }
    }

    public float CalculateScalePlayer(int gridSize)
    {
        switch (gridSize)
        {
            case 3: return playerScale3x3;
            case 4: return playerScale4x4;
            case 5: return playerScale5x5;
            default: return 1f;  // Default player scale factor
        }
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
