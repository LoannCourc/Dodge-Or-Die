using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public ArrowManager arrowManager;
    public SpawnManager spawnManager;
    public WaveManager waveManager;
    public GameObject player;
    private int currentGridSize;
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
        float tileSpacing = ScaleManager.Instance.CalculateTileSpacing(currentGridSize);
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
            player.transform.position = centerPos;
        }
    }
}
