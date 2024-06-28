using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WaveManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public GameObject crashPrefab;
    public GameObject spawnIndicator;
    public GameObject dashPrefab;
    public GameObject player;
    public ParticleSystem crashParticles;
    [Header("Managers")]
    public GridManager gridManager;
    public ArrowManager arrowManager;
    public SpawnManager spawnManager;
    public CSVReader csvReader;
    [Header("Internal Variables")]
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
        
        UpdateCooldownDuration();
    }

    void UpdateCooldownDuration()
    {
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
                    UpdateCooldownDuration();
                }
            }
        }
    }

    private void SpawnObject(SpawnData spawnData)
    {
        float randomValue = Random.value;
        int numberOfObjectsToSpawn = 1;

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

        // Activer la flèche correspondante
        arrowManager.ActivateArrow(randomSpawnIndex);

        // Récupérer la rotation de la flèche activée et appliquer cette rotation au sprite du bullet
        float arrowRotation = arrowManager.GetArrowRotation(randomSpawnIndex);
        instantiatedObject.transform.rotation = Quaternion.Euler(0, 0, arrowRotation);

        // Définir la direction initiale du bullet
        Vector3 direction = (player.transform.position - instantiatedObject.transform.position).normalized;
        instantiatedObject.GetComponent<BulletController>().SetDirection(direction);

        // Récupérer les données de vitesse du bullet depuis le CSV
        SpawnData spawnData = csvReader.GetSpawnData(currentScore, gridSize);
        instantiatedObject.GetComponent<BulletController>().SetSpeed(spawnData.bulletSpeed);
    
        // Appliquer l'échelle calculée pour l'objet
        ScaleManager.Instance.ScaleSpawnedObjects(instantiatedObject, ScaleManager.Instance.CalculateScaleObject(currentGridSize));

        // Jouer le son associé au lancement du bullet
        AudioManager.Instance.PlaySound("BulletLauncher");
    }

  public IEnumerator SpawnCrash()
{
    SpawnData spawnData = csvReader.GetSpawnData(currentScore, gridSize);
    Vector3 spawnPosition = player.transform.position;

    GameObject indicator = Instantiate(spawnIndicator, spawnPosition, Quaternion.identity);
    indicator.transform.localPosition = new Vector3(indicator.transform.localPosition.x, indicator.transform.localPosition.y, 0); // Forcer Z à 0

    float tileSpacing = ScaleManager.Instance.GetTileSpacing();
    float offset = (gridSize % 2 == 0) ? tileSpacing / 2f : 0f;
    float centeredX = Mathf.Round((indicator.transform.position.x - offset) / tileSpacing) * tileSpacing + offset;
    float centeredY = indicator.transform.position.y;

    indicator.transform.position = new Vector3(centeredX, centeredY, 0); // Réinitialiser Z à 0 après ajustement

    float animationDuration = spawnData.crashSpeed / 3f;
    Sequence indicatorSequence = DOTween.Sequence();
    indicatorSequence.Append(indicator.transform.DOScale(indicator.transform.localScale * 1.5f, animationDuration).SetEase(Ease.InOutQuad));
    indicatorSequence.Append(indicator.transform.DOScale(indicator.transform.localScale, animationDuration).SetEase(Ease.InOutQuad));

    yield return indicatorSequence.WaitForCompletion();

    float randomRotationZ = Random.Range(0f, 360f);
    GameObject crashObject = Instantiate(crashPrefab, new Vector3(centeredX, centeredY, 0), Quaternion.Euler(0, 0, randomRotationZ));

    AudioManager.Instance.PlaySound("CrashSound");
    crashObject.GetComponent<CrashObject>().CenterOnTile(tileSpacing);
    crashObject.GetComponent<CrashObject>().SetSpeed(spawnData.crashSpeed);
    Destroy(indicator);

    crashObject.transform.DOShakePosition(0.4f, new Vector3(0.1f, 0.1f, 0), 50, 90, false, true);

    if (crashParticles != null)
    {
        ParticleSystem particles = Instantiate(crashParticles, new Vector3(centeredX, centeredY, 0), Quaternion.identity);
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
            instantiatedObject.GetComponent<DashObject>().SetRotation(arrowManager.GetArrowRotation(randomSpawnIndex)); // Nouvelle méthode pour définir la rotation
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
