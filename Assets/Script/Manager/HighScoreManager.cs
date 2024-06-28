using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetHighScore(int gridSize)
    {
        return PlayerPrefs.GetInt("HighScore_" + gridSize, 0);
    }

    public void SaveHighScore(int gridSize, int score)
    {
        int currentHighScore = GetHighScore(gridSize);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore_" + gridSize, score);
            PlayerPrefs.Save();
        }
    }
}