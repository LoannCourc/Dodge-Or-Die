using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance; // Instance statique du ScoreManager

    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Rechercher une instance existante dans la scène
                instance = FindObjectOfType<ScoreManager>();

                // Si aucune instance n'existe, créer un nouvel objet ScoreManager dans la scène
                if (instance == null)
                {
                    GameObject scoreManagerObject = new GameObject("ScoreManager");
                    instance = scoreManagerObject.AddComponent<ScoreManager>();
                }
            }
            return instance;
        }
    }

    private int score = 0; // Variable pour stocker le score actuel
    public TextMeshProUGUI scoreText; // Référence au TextMeshPro pour afficher le score

    // Méthode pour incrémenter le score
    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    // Méthode pour réinitialiser le score
    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    // Méthode pour obtenir le score actuel
    public int GetScore()
    {
        return score;
    }

    // Méthode pour mettre à jour le TextMeshPro avec le score actuel
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
            GameManager.Instance.NewScore(score);
        }
    }

    private void Awake()
    {
        // Assurez-vous qu'il n'y a qu'une seule instance du ScoreManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Assurez-vous que l'objet ScoreManager persiste entre les scènes
        }
        else
        {
            Destroy(gameObject); // Détruire les instances supplémentaires
        }
    }
}