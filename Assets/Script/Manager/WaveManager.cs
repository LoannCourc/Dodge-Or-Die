using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WaveManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject crashPrefab;
    public GameObject spawnIndicator;
    public GameObject dashPrefab;
    public GameObject player;

    public GridManager gridManager;
    public ArrowManager arrowManager;
    public SpawnManager spawnManager;
    public CSVReader csvReader;
    private int gridSize;
    private int currentScore = 0;
    private float currentCooldown = 0f;
    private float cooldownDuration = 0f;
    
    void Start()
    {
        if (csvReader == null)
        {
            Debug.LogError("csvReader is not assigned in WaveManager!");
        }
        else
        {
            csvReader.ReadCSV();
        }
    }
    
   public void OnScoreUpdated(int newScore)
   {
       currentScore = newScore;
       gridSize = gridManager.GetGridSize();
       
       SpawnData spawnData = csvReader.GetSpawnData(currentScore, gridSize);
   
       if (spawnData != null)
       {
           cooldownDuration = Random.Range(spawnData.minSpawnTime, spawnData.maxSpawnTime);
           currentCooldown = 0f;
       }
       else
       {
           Debug.LogWarning($"SpawnData is null for score {currentScore}. Check CSVReader.GetSpawnData implementation.");
       }
   }
   void Update()
   {
       if (currentCooldown < cooldownDuration)
       {
           currentCooldown += Time.deltaTime;

           if (currentCooldown >= cooldownDuration)
           {
               SpawnData spawnData = csvReader.GetSpawnData(currentScore, gridSize);
               if (spawnData != null)
               {
                   SpawnObject(spawnData);
                   cooldownDuration = Random.Range(spawnData.minSpawnTime, spawnData.maxSpawnTime);
                   currentCooldown = 0f;
               }
           }
       }
   }

   private void SpawnObject(SpawnData spawnData)
   {
       // Générer un nombre aléatoire pour déterminer le nombre d'objets à spawner
       float randomValue = Random.value;
       int numberOfObjectsToSpawn = 1; // Par défaut, spawner un seul objet

       if (randomValue <= spawnData.multiSpawnChance1)
       {
           numberOfObjectsToSpawn = 1;
       }
       else if (randomValue <= spawnData.multiSpawnChance1 + spawnData.multiSpawnChance2)
       {
           numberOfObjectsToSpawn = 2;
       }
       else if (randomValue <= spawnData.multiSpawnChance1 + spawnData.multiSpawnChance2 + spawnData.multiSpawnChance3)
       {
           numberOfObjectsToSpawn = 3;
       }

       // Appeler la méthode appropriée pour spawner le nombre correct d'objets
       for (int i = 0; i < numberOfObjectsToSpawn; i++)
       {
           SpawnSingleObject(spawnData);
       }
   }

   private void SpawnSingleObject(SpawnData spawnData)
   {
       float randomValue = Random.value;

       if (randomValue <= spawnData.bulletSpawnChance)
       {
           //SpawnBullet();
           StartCoroutine(SpawnCrash());
       }
       else if (randomValue <= spawnData.bulletSpawnChance + spawnData.crashSpawnChance)
       {
           StartCoroutine(SpawnCrash());
       }
       else if (randomValue <= spawnData.bulletSpawnChance + spawnData.crashSpawnChance + spawnData.dashSpawnChance)
       {
           //SpawnDash();
           StartCoroutine(SpawnCrash());
       }
   }

    
    public void SpawnBullet()
    {
        int currentGridSize = gridManager.GetGridSize();
        int randomSpawnIndex = Random.Range(0, currentGridSize * 4);
        GameObject selectedSpawnPoint = spawnManager.GetSpawnPoint(randomSpawnIndex);
        GameObject instantiatedObject = Instantiate(bulletPrefab, selectedSpawnPoint.transform.position, Quaternion.identity);
        arrowManager.ActivateArrow(randomSpawnIndex);
        Vector3 direction = (player.transform.position - instantiatedObject.transform.position).normalized;
        instantiatedObject.GetComponent<BulletController>().SetDirection(direction);
        SpawnData spawnData = csvReader.GetSpawnData(currentScore, gridSize);
        instantiatedObject.GetComponent<BulletController>().SetSpeed(spawnData.bulletSpeed);
    }

    public IEnumerator SpawnCrash()
    {
        SpawnData spawnData = csvReader.GetSpawnData(currentScore, gridSize);
        GameObject indicator = Instantiate(spawnIndicator, player.transform.position, Quaternion.identity);

        // Récupère la position actuelle de l'objet
        Vector3 position = indicator.transform.position;

        // Calcule la position centrée sur une tuile la plus proche
        float tileSpacing = gridManager.GetTileSpacing();
        float offset = (gridSize % 2 == 0) ? tileSpacing / 2f : 0f;  // Ajustement pour les grilles de taille paire
        float centeredX = Mathf.Round((position.x - offset) / tileSpacing) * tileSpacing + offset;
        float centeredY = position.y;  // Assume no vertical adjustment needed
        float centeredZ = Mathf.Round((position.z - offset) / tileSpacing) * tileSpacing + offset;

        // Met à jour la position de l'objet
        indicator.transform.position = new Vector3(centeredX, centeredY, centeredZ);

        // Animation d'agrandissement et de rétrécissement avec DOTween
        float animationDuration = spawnData.crashSpeed / 3f;
        Sequence indicatorSequence = DOTween.Sequence();
        indicatorSequence.Append(indicator.transform.DOScale(indicator.transform.localScale * 1.5f, animationDuration).SetEase(Ease.InOutQuad));
        indicatorSequence.Append(indicator.transform.DOScale(indicator.transform.localScale, animationDuration).SetEase(Ease.InOutQuad));

        yield return indicatorSequence.WaitForCompletion();

        // Instanciation de l'objet de crash
        GameObject crashObject = Instantiate(crashPrefab, indicator.transform.position, Quaternion.identity);

        // Centre le CrashObject sur une tuile
        crashObject.GetComponent<CrashObject>().CenterOnTile(tileSpacing);

        crashObject.GetComponent<CrashObject>().SetSpeed(spawnData.crashSpeed);
        Destroy(indicator);
    }

    public void SpawnDash()
    {
        int currentGridSize = gridManager.GetGridSize();
        int randomSpawnIndex = Random.Range(0, currentGridSize * 4);
        GameObject selectedSpawnPoint = spawnManager.GetSpawnPoint(randomSpawnIndex);
        GameObject instantiatedObject = Instantiate(dashPrefab, selectedSpawnPoint.transform.position, Quaternion.identity);
        Vector3 arrowPosition = arrowManager.GetArrowPosition(randomSpawnIndex);
        if (arrowPosition != Vector3.zero)
        {
            instantiatedObject.GetComponent<DashObject>().SetTargetPosition(arrowPosition);
            arrowManager.ActivateArrow(randomSpawnIndex);
        }
        else
        {
            Debug.LogWarning("Arrow position not found for dash spawn index: " + randomSpawnIndex);
            Destroy(instantiatedObject);
        }
        SpawnData spawnData = csvReader.GetSpawnData(currentScore, gridSize);
        instantiatedObject.GetComponent<DashObject>().SetSpeed(spawnData.dashSpeed);
    }
}
