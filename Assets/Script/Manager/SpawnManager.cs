using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject spawnPointPrefab;
    public float spawnPointDistance = 1.0f; // Distance entre chaque spawn point

    public GameObject[] spawnPoints;

    public void InitializeSpawnPoints(int gridSize, float tileSpacing)
    {
        ClearSpawnPoints();

        float halfSize = (gridSize - 1) * tileSpacing / 2;

        // Calculer le nombre total de spawn points en fonction de la taille de la grille
        int totalSpawnPoints = gridSize * 4;
        spawnPoints = new GameObject[totalSpawnPoints];

        for (int i = 0; i < gridSize; i++)
        {
            // Haut
            spawnPoints[i] = Instantiate(spawnPointPrefab, new Vector3(-halfSize + i * tileSpacing, halfSize + spawnPointDistance, 0), Quaternion.identity, transform);
            // Bas
            spawnPoints[i + gridSize] = Instantiate(spawnPointPrefab, new Vector3(-halfSize + i * tileSpacing, -halfSize - spawnPointDistance, 0), Quaternion.identity, transform);
            // Gauche
            spawnPoints[i + gridSize * 2] = Instantiate(spawnPointPrefab, new Vector3(-halfSize - spawnPointDistance, halfSize - i * tileSpacing, 0), Quaternion.identity, transform);
            // Droite
            spawnPoints[i + gridSize * 3] = Instantiate(spawnPointPrefab, new Vector3(halfSize + spawnPointDistance, halfSize - i * tileSpacing, 0), Quaternion.identity, transform);
        }
    }

    public GameObject GetSpawnPoint(int index)
    {
        if (index >= 0 && index < spawnPoints.Length)
        {
            return spawnPoints[index];
        }
        else
        {
            Debug.LogError("Invalid spawn point index: " + index);
            return null;
        }
    }
    public Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points not initialized or empty.");
            return Vector3.zero; // Ou une autre position par défaut
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].transform.position;
    }
    private void ClearSpawnPoints()
    {
        if (spawnPoints != null)
        {
            foreach (GameObject spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Destroy(spawnPoint);
                }
            }
        }
    }
}
