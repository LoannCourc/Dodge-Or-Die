using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager; // Référence au GridManager
    public ArrowManager arrowManager; // Référence au ArrowManager
    public SpawnManager spawnManager; // Référence au SpawnManager
    public WaveManager waveManager; // Référence au SpawnManager
    public GameObject player;
    private int currentGridSize;
    private int currentScore;
    
    public bool isGameStarted = false;

    private static GameManager instance; // Instance statique du ScoreManager

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Rechercher une instance existante dans la scène
                instance = FindObjectOfType<GameManager>();

                // Si aucune instance n'existe, créer un nouvel objet ScoreManager dans la scène
                if (instance == null)
                {
                    GameObject gameManagerObject = new GameObject("ScoreManager");
                    instance = gameManagerObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    
    void Start()
    {
        if (gridManager == null || arrowManager == null || spawnManager == null)
        {
            Debug.LogError("GridManager, ArrowManager ou SpawnManager non trouvé dans la scène.");
        }
        else
        {
            InitializeGame();
        }
    }

    public void InitializeGame()
    {
        currentGridSize = gridManager.GetGridSize();
        float tileSpacing = gridManager.GetTileSpacing();
        arrowManager.InitializeArrows(currentGridSize, tileSpacing);
        spawnManager.InitializeSpawnPoints(currentGridSize, tileSpacing);
    }

    public void NewScore(int score)
    {
        currentScore = score;
        waveManager.OnScoreUpdated(currentScore);
    }
    
    public void StartGame()
    {
        isGameStarted = true;
        player.SetActive(true);
        NewScore(1);
        if (waveManager != null)
        {
            //InvokeRepeating("SpawnBullet",1f,5f);
        }
    }

    void SpawnBullet()
    {
        waveManager.SpawnDash();
    }
}