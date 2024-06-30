using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public ArrowManager arrowManager;
    public SpawnManager spawnManager;
    public WaveManager waveManager;
    public GameObject player;
    
    public GameObject canvasHeal;
    public int currentGridSize;
    private int currentScore;
    private Vector3 centerPos;
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject gameManagerObject = new GameObject("GameManager");
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
        Application.targetFrameRate = 60;
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
        float tileSpacing = ScaleManager.Instance.CalculateTileSpacing(currentGridSize);
        gridManager.InitializeGrid(currentGridSize);
        arrowManager.InitializeArrows(currentGridSize, tileSpacing);
        spawnManager.InitializeSpawnPoints(currentGridSize, tileSpacing);
    }

    public void RestartGame()
    {
        // Réinitialiser le score ou tout autre état de jeu nécessaire
        currentScore = 0;

        ScoreManager.Instance.ResetScore();
        
        // Réinitialiser et préparer la grille et les autres composants pour un nouveau jeu
        float tileSpacing = ScaleManager.Instance.CalculateTileSpacing(currentGridSize);
        gridManager.InitializeGrid(currentGridSize);
        arrowManager.InitializeArrows(currentGridSize, tileSpacing);
        spawnManager.InitializeSpawnPoints(currentGridSize, tileSpacing);
        waveManager.ResetWaveManager(); // Vous devrez ajouter cette méthode dans WaveManager pour réinitialiser son état

        player.GetComponent<PlayerHealth>().Heal(3);
        
        canvasHeal.SetActive(true);
        
        // Reactiver le joueur et le positionner
        player.SetActive(true);
        ChangePlayerPosition();
       
        // Supprime tous les objets instanciés liés au gameplay
        DestroySpawnedObjects();
        
        waveManager.ResetWaveManager();

        // Redémarrer le jeu
        InitializeGame();
        
        // Démarrer le jeu avec le score de départ si nécessaire
        NewScore(1);

        // Assurez-vous que le jeu n'est pas en pause
        Time.timeScale = 1f;

        // Jouer la musique de jeu principale, si elle était arrêtée ou changée
        AudioManager.Instance.PlaySound("MainMusic");
    }
    
    private void DestroySpawnedObjects()
    {
        // Trouver et détruire tous les objets avec un certain tag ou ceux créés dynamiquement
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        
        // Assurez-vous de nettoyer tous les autres types d'objets que vous avez peut-être instanciés
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

    public int CalculateFinalScore()
    {
        return currentScore;
    }
    
    public void EndGame()
    {
        int finalScore = CalculateFinalScore(); // Remplacez ceci par votre logique de score
        int gridSize = gridManager.GetGridSize();
        HighScoreManager.Instance.SaveHighScore(gridSize, finalScore);
    }
    
    void ChangePlayerPosition()
    {
        float tileSpacing = ScaleManager.Instance.CalculateTileSpacing(currentGridSize);
        float gridSizeHalf = (currentGridSize - 1) / 2.0f;

        // Calculer la position du joueur pour qu'il soit au centre de la grille
        centerPos = new Vector3((gridSizeHalf - Mathf.Floor(gridSizeHalf)) * tileSpacing, 
            (gridSizeHalf - Mathf.Floor(gridSizeHalf)) * tileSpacing, 
            0);
        
        if (currentGridSize == 4)
        {
            int randomPos = Random.Range(0, 4);
            switch (randomPos)
            {
                case 0:
                    centerPos = new Vector3(tileSpacing / 2f, tileSpacing / 2f, 0);
                    break;
                case 1:
                    centerPos = new Vector3(-tileSpacing / 2f, tileSpacing / 2f, 0);
                    break;
                case 2:
                    centerPos = new Vector3(-tileSpacing / 2f, -tileSpacing / 2f, 0);
                    break;
                case 3:
                    centerPos = new Vector3(tileSpacing / 2f, -tileSpacing / 2f, 0);
                    break;
            }
        }
        // Appliquer la position calculée
        player.transform.position = centerPos;
    }
}
