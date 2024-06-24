using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider gridSizeSlider;
    public TextMeshProUGUI gridSizeText;
    public GameObject sliderPanel; // Référence vers le panel contenant le slider
    
    public GridManager gridManager;
    public GameManager gameManager;
   
    private void Start()
    {
        gridSizeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(gridSizeSlider.value);
        Time.timeScale = 0f;
    }
    
    public void DisableSliderPanel()
    {
        sliderPanel.SetActive(false);
        gameManager.StartGame();
        Time.timeScale = 1f;
    }

    private void OnSliderValueChanged(float value)
    {
        int gridSize = Mathf.RoundToInt(value);
        gridSizeText.text = gridSize + " x " + gridSize;
        // Initialise la grille et les flèches en fonction de la taille de la grille
        gridManager.InitializeGrid(gridSize);
        gameManager.InitializeGame();
    }
}