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
    }
    
    public void DisableSliderPanel()
    {
        sliderPanel.SetActive(false);
        gameManager.StartGame();
        Time.timeScale = 1f;
        AudioManager.Instance.StopSound("IntroMusic");
        AudioManager.Instance.PlaySound("MainMusic");
    }

    private void OnSliderValueChanged(float value)
    {
        int gridSize = Mathf.RoundToInt(value);
        gridSizeText.text = gridSize + " x " + gridSize;
       
        // Met à jour la grille et redimensionne les objets en fonction de la taille de la grille
        gridManager.InitializeGrid(gridSize); // Initialise la grille
        ScaleManager.Instance.ScaleObjects(gridSize); // Redimensionne les objets en fonction de la taille de la grille
        // Autres actions à effectuer après avoir initialisé la grille et redimensionné les objets
        gameManager.InitializeGame();
    }
}