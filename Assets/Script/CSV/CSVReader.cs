using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class CSVReader : MonoBehaviour
{
    public TextAsset[] csvFiles; // Array de TextAsset pour contenir les différents CSV
    private List<List<SpawnData>> spawnDataLists; // Liste de listes de SpawnData pour chaque CSV

    void Awake()
    {
        LoadCSVs();
    }

    public void ReadCSV()
    {
        if (csvFiles == null || csvFiles.Length == 0)
        {
            Debug.LogError("CSV files are not assigned in CSVReader!");
            return;
        }

        // Appel à LoadCSVs() ou autre méthode pour charger les données des fichiers CSV
        LoadCSVs();
    }
    
    // Méthode pour charger tous les CSV au démarrage
    private void LoadCSVs()
    {
    spawnDataLists = new List<List<SpawnData>>();

    foreach (TextAsset csvFile in csvFiles)
    {
        List<SpawnData> spawnDataList = new List<SpawnData>();
        using (StringReader reader = new StringReader(csvFile.text))
        {
            string line;
            bool isFirstLine = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // Ignorer la première ligne d'en-têtes
                }

                string[] data = line.Split(';');
                if (data.Length < 12) // Assurez-vous que le nombre correspond aux colonnes attendues
                {
                    Debug.LogWarning("Skipping invalid data entry.");
                    continue;
                }

                // Conversion des valeurs du CSV en types appropriés
                if (!int.TryParse(data[0].Trim(), out int scoreThreshold))
                {
                    Debug.LogWarning($"Skipping invalid scoreThreshold: {data[0].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[1].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float bulletSpawnChance))
                {
                    Debug.LogWarning($"Skipping invalid bulletSpawnChance: {data[1].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[2].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float crashSpawnChance))
                {
                    Debug.LogWarning($"Skipping invalid crashSpawnChance: {data[2].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[3].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float dashSpawnChance))
                {
                    Debug.LogWarning($"Skipping invalid dashSpawnChance: {data[3].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[4].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float bulletSpeed))
                {
                    Debug.LogWarning($"Skipping invalid bulletSpeed: {data[4].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[5].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float crashSpeed))
                {
                    Debug.LogWarning($"Skipping invalid crashSpeed: {data[5].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[6].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float dashSpeed))
                {
                    Debug.LogWarning($"Skipping invalid dashSpeed: {data[6].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[7].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float multiSpawnChance1))
                {
                    Debug.LogWarning($"Skipping invalid multiSpawnChance1: {data[7].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[8].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float multiSpawnChance2))
                {
                    Debug.LogWarning($"Skipping invalid multiSpawnChance2: {data[8].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[9].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float multiSpawnChance3))
                {
                    Debug.LogWarning($"Skipping invalid multiSpawnChance3: {data[9].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[10].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float minTimeSpawn))
                {
                    Debug.LogWarning($"Skipping invalid minTimeSpawn: {data[10].Trim()}");
                    continue;
                }

                if (!float.TryParse(data[11].Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float maxTimeSpawn))
                {
                    Debug.LogWarning($"Skipping invalid maxTimeSpawn: {data[11].Trim()}");
                    continue;
                }

                // Création de l'objet SpawnData et ajout à la liste
                SpawnData spawnData = new SpawnData(
                    scoreThreshold, bulletSpawnChance, crashSpawnChance, dashSpawnChance,
                    bulletSpeed, crashSpeed, dashSpeed,
                    multiSpawnChance1, multiSpawnChance2, multiSpawnChance3,
                    minTimeSpawn, maxTimeSpawn
                );

                spawnDataList.Add(spawnData);
            }
        }

        spawnDataLists.Add(spawnDataList);
    }
}


    // Méthode pour obtenir les données de spawn en fonction du score actuel et de la grille
    public SpawnData GetSpawnData(int currentScore, int gridSize)
    {
        // Sélectionner le bon CSV en fonction de la grille choisie
        int csvIndex = gridSize - 3; // Supposant que gridSize 3 correspond à csvFiles[0], 4 à csvFiles[1], etc.

        // Vérifier si le score actuel est inférieur au score minimum dans le CSV
        foreach (SpawnData spawnData in spawnDataLists[csvIndex])
        {
            if (currentScore >= spawnData.scoreThreshold)
            {
                return spawnData;
            }
        }

        return null;
    }
}


[System.Serializable]
public class SpawnData
{
    public int scoreThreshold;
    public float bulletSpawnChance;
    public float crashSpawnChance;
    public float dashSpawnChance;
    public float bulletSpeed;
    public float crashSpeed;
    public float dashSpeed;
    public float multiSpawnChance1;
    public float multiSpawnChance2;
    public float multiSpawnChance3;
    public float minSpawnTime;
    public float maxSpawnTime;

    public SpawnData(int scoreThreshold, float bulletSpawnChance, float crashSpawnChance, float dashSpawnChance,
        float bulletSpeed, float crashSpeed, float dashSpeed,
        float multiSpawnChance1, float multiSpawnChance2, float multiSpawnChance3,
        float minSpawnTime, float maxSpawnTime)
    {
        this.scoreThreshold = scoreThreshold;
        this.bulletSpawnChance = bulletSpawnChance;
        this.crashSpawnChance = crashSpawnChance;
        this.dashSpawnChance = dashSpawnChance;
        this.bulletSpeed = bulletSpeed;
        this.crashSpeed = crashSpeed;
        this.dashSpeed = dashSpeed;
        this.multiSpawnChance1 = multiSpawnChance1;
        this.multiSpawnChance2 = multiSpawnChance2;
        this.multiSpawnChance3 = multiSpawnChance3;
        this.minSpawnTime = minSpawnTime;
        this.maxSpawnTime = maxSpawnTime;
    }
}
