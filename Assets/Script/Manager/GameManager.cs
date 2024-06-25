using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager; // Référence au GridManager
    public ArrowManager arrowManager; // Référence au ArrowManager
    public SpawnManager spawnManager; // Référence au SpawnManager
    public WaveManager waveManager; // Référence au SpawnManager
    public GameObject player;
    private int currentGridSize;
    private int currentScore;
    private Vector3 centerPos;
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

    private void Awake()
    {
        AudioManager.Instance.PlaySound("IntroMusic");
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
        player.SetActive(true);
        ChangePlayerPosition();
        NewScore(1);
    }

    void ChangePlayerPosition()
    {
        float tileSpacing = gridManager.GetTileSpacing();
        // Déplacer le joueur au centre d'une tuile si la grille est de taille 4x4
        if (currentGridSize == 4)
        {
            int randomPos = Random.Range(0, 4);
            switch (randomPos)
            {
                case 0 : 
                    centerPos = new Vector3(tileSpacing/2f, tileSpacing/2f, tileSpacing/2f);
                    break;
                case 1 : 
                    centerPos = new Vector3(-tileSpacing/2f, tileSpacing/2f, tileSpacing/2f);
                    break;
                case 2 : 
                    centerPos = new Vector3(-tileSpacing/2f, -tileSpacing/2f, tileSpacing/2f);
                    break;
                case 3 : 
                    centerPos = new Vector3(tileSpacing/2f, -tileSpacing/2f, tileSpacing/2f);
                    break;
            }

            Vector3 centerPosition = centerPos;
            
            // Déplacer le joueur à la position centrale
            player.transform.position = centerPosition;
        }
    }
}