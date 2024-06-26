using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridSize;
    public GameObject tilePrefab;
    private GameObject[] tiles;

    public void InitializeGrid(int size)
    {
        gridSize = size;
        CreateGrid();
    }

    private void CreateGrid()
    {
        float tileSpacing = ScaleManager.Instance.GetTileSpacing();
        float halfSize = (gridSize - 1) * tileSpacing / 2;

        if (tiles != null)
        {
            foreach (GameObject tile in tiles)
            {
                Destroy(tile);
            }
        }

        tiles = new GameObject[gridSize * gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(-halfSize + x * tileSpacing, -halfSize + y * tileSpacing, 0);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tiles[x * gridSize + y] = tile;
            }
        }
        ScaleManager.Instance.ScaleObjects(gridSize);
    }

    public int GetGridSize()
    {
        return gridSize;
    }
}