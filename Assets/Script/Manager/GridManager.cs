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

    public GameObject[] GetGroundTiles()
    {
        if (groundTiles == null)
        {
            Debug.LogError("Grid has not been initialized.");
            return new GameObject[0];
        }

        // Convertir le tableau 2D en un tableau 1D pour les tiles du sol
        GameObject[] tiles = new GameObject[gridSize * gridSize];
        int index = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                tiles[index] = groundTiles[x, y];
                index++;
            }
        }

        return tiles;
    }

    public Vector3 GetRandomFreeTilePosition()
    {
        List<Vector3> freeTilePositions = GetFreeTilePositions();

        if (freeTilePositions == null || freeTilePositions.Count == 0)
        {
            Debug.LogWarning("No free tiles available.");
            return Vector3.zero; // Ou une autre position par défaut
        }

        int randomIndex = Random.Range(0, freeTilePositions.Count);
        return freeTilePositions[randomIndex];
    }

    List<Vector3> GetFreeTilePositions()
    {
        List<Vector3> freePositions = new List<Vector3>();

        // Parcourir les positions potentielles dans la grille
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = groundTiles[x, y].transform.position;

                // Vérifier si la position n'est pas déjà occupée
                if (!IsPositionOccupied(position))
                {
                    freePositions.Add(position);
                }
            }
        }

        return freePositions;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        // Vérifie si la position est occupée par un objet ou non
        // Vous devez implémenter cette méthode selon la logique de votre jeu
        // Cela pourrait impliquer de vérifier les collisions avec d'autres objets, etc.
        return false; // Exemple simple : toujours considérer comme non occupée
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
