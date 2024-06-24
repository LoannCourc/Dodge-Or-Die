using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject groundTilePrefab; // Prefab des tiles du sol
    public int gridSize = 3; // Taille de la grille (par exemple 3x3)
    public float tileSpacing = 1.0f; // Espacement entre les tiles

    private GameObject[,] groundTiles; // Tableau 2D pour les tiles du sol

    void Start()
    {
        InitializeGrid(gridSize);
    }

    public void InitializeGrid(int size)
    {
        gridSize = size;
        ClearGrid();

        float halfSize = (size - 1) * tileSpacing / 2;
        groundTiles = new GameObject[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector3 tilePosition = new Vector3(-halfSize + x * tileSpacing, -halfSize + y * tileSpacing, 0);
                groundTiles[x, y] = Instantiate(groundTilePrefab, tilePosition, Quaternion.identity, transform);
            }
        }
    }

    public int GetGridSize()
    {
        return gridSize;
    }

    public float GetTileSpacing()
    {
        return tileSpacing;
    }
    
    private void ClearGrid()
    {
        if (groundTiles != null)
        {
            foreach (GameObject tile in groundTiles)
            {
                if (tile != null)
                {
                    Destroy(tile);
                }
            }
        }
    }
}
