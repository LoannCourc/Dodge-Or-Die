using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
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
            DisplayEndGameMessage(true, score);
        }
        else
        {
            DisplayEndGameMessage(false, score);
        }
    }

    // Nouvelle fonction pour afficher un message de fin de jeu
    public void DisplayEndGameMessage(bool isNewHighScore, int score)
    {
        if (isNewHighScore)
        {
            scoreText.text = "New Highscore: " + score;
        }
        else
        {
            scoreText.text = "Your Score: " + score + ". Highscore: " + GetHighScore(GameManager.Instance.currentGridSize);
        }
    }
}