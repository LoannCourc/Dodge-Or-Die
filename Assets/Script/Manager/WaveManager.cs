using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WaveManager : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab for the bullet object.")]
    public GameObject bulletPrefab;
    [Tooltip("Prefab for the crash object.")]
    public GameObject crashPrefab;
    [Tooltip("Prefab for the spawn indicator.")]
    public GameObject spawnIndicator;
    [Tooltip("Prefab for the dash object.")]
    public GameObject dashPrefab;
    [Tooltip("Reference to the player object.")]
    public GameObject player;
    [Tooltip("Prefab for the crash particles.")]
    public ParticleSystem crashParticles;
    [Header("Managers")]
    [Tooltip("Reference to the GridManager script.")]
    public GridManager gridManager;
    [Tooltip("Reference to the ArrowManager script.")]
    public ArrowManager arrowManager;
    [Tooltip("Reference to the SpawnManager script.")]
    public SpawnManager spawnManager;
    [Tooltip("Reference to the CSVReader script.")]
    public CSVReader csvReader;
    [Header("Internal Variables")]
    [Tooltip("The size of the grid.")]
    private int gridSize;
    [Tooltip("The current score.")]
    private int currentScore = 0;
    [Tooltip("The current cooldown time.")]
    private float currentCooldown = 0f;
    [Tooltip("The duration of the cooldown period.")]
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
           SpawnBullet();
       }
       else if (randomValue <= spawnData.bulletSpawnChance + spawnData.crashSpawnChance)
       {
           StartCoroutine(SpawnCrash());
       }
       else if (randomValue <= spawnData.bulletSpawnChance + spawnData.crashSpawnChance + spawnData.dashSpawnChance)
       {
           SpawnDash();
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
        
        // Appel pour redimensionner l'objet instancié
        ScaleManager.Instance.ScaleSpawnedObjects(instantiatedObject, ScaleManager.Instance.CalculateScaleObject(currentGridSize));
    }

    public IEnumerator SpawnCrash()
{
    SpawnData spawnData = csvReader.GetSpawnData(currentScore, gridSize);
    GameObject indicator = Instantiate(spawnIndicator, player.transform.position, Quaternion.identity);

    // Récupère la position actuelle de l'objet
    Vector3 position = indicator.transform.position;

    // Calcule la position centrée sur une tuile la plus proche
    float tileSpacing = ScaleManager.Instance.GetTileSpacing();
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

    // Ajouter l'effet de tremblement à l'objet crash
    crashObject.transform.DOShakePosition(0.4f, new Vector3(0.1f, 0.1f, 0.1f), 50, 90, false, true);

    // Lancer les particules de crash
    if (crashParticles != null)
    {
        ParticleSystem particles = Instantiate(crashParticles, crashObject.transform.position, Quaternion.identity);
        particles.Play();
        Destroy(particles.gameObject, particles.main.duration);
    }
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
