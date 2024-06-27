using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The pause menu UI.")]
    public GameObject pauseMenuUI;

    [Tooltip("The button to resume the game.")]
    public Button resumeButton;

    [Tooltip("The button to quit the game.")]
    public Button quitButton;

    [Tooltip("The button to toggle the music.")]
    public Button musicButton;

    [Tooltip("The button to pause the game.")]
    public Button pauseButton;

    [Header("Music Icons")]
    [Tooltip("Icon for music on.")]
    public Sprite musicOnIcon;

    [Tooltip("Icon for music off.")]
    public Sprite musicOffIcon;

    public Image musicButtonImage;
    private bool isMusicOn = true;

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        musicButton.onClick.AddListener(ToggleMusic);
        pauseButton.onClick.AddListener(Pause);
        
        if (musicButtonImage != null)
        {
            musicButtonImage.sprite = musicOnIcon;
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
        isMusicOn = !isMusicOn;

        if (musicButtonImage != null)
        {
            musicButtonImage.sprite = isMusicOn ? musicOnIcon : musicOffIcon;
        }
    }

    void Pause()
    {
        if (pauseMenuUI.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuUI.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
