using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider gridSizeSlider;
    public TextMeshProUGUI gridSizeText;
    public TextMeshProUGUI highScoreText; // Ajout pour afficher le high score
    public GameObject sliderPanel;
    public GameObject panelPause;
    
    public GridManager gridManager;
    public GameManager gameManager;

    private bool _firstTime;

    private void Start()
    {
        gridSizeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(gridSizeSlider.value);
    }

    public void DisableSliderPanel()
    {
        sliderPanel.SetActive(false);
        panelPause.SetActive(true);
        gameManager.StartGame();
        Time.timeScale = 1f;
        AudioManager.Instance.StopSound("IntroMusic");
        AudioManager.Instance.PlaySound("MainMusic");
    }

    private void OnSliderValueChanged(float value)
    {
        int gridSize = Mathf.RoundToInt(value);
        gridSizeText.text = gridSize + " x " + gridSize;

        UpdateHighScoreDisplay(gridSize); // Mettre à jour l'affichage du high score

        if (_firstTime)
        {
            AudioManager.Instance.PlaySound("SliderSound");
        }
        gridManager.InitializeGrid(gridSize);
        ScaleManager.Instance.ScaleObjects(gridSize);
        gameManager.InitializeGame();
        _firstTime = true;
    }

    private void UpdateHighScoreDisplay(int gridSize)
    {
        int highScore = HighScoreManager.Instance.GetHighScore(gridSize);
        highScoreText.text = "High Score: " + highScore;
    }
}