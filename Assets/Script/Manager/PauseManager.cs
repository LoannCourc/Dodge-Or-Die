using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
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

    [Tooltip("The button to toggle the SFX.")]
    public Button sfxButton;

    [Tooltip("The button to toggle the SFX.")]
    public Button pauseButton;
    
    [Header("Icons")]
    [Tooltip("Icon for music on.")]
    public Sprite musicOnIcon;

    [Tooltip("Icon for music off.")]
    public Sprite musicOffIcon;

    [Tooltip("Icon for SFX on.")]
    public Sprite sfxOnIcon;

    [Tooltip("Icon for SFX off.")]
    public Sprite sfxOffIcon;

    public Image musicButtonImage;
    public Image sfxButtonImage;

    private bool isMusicOn = true;
    private bool isSFXOn = true;

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);
        pauseButton.onClick.AddListener(PauseGame);

        if (musicButtonImage != null)
        {
            musicButtonImage.sprite = isMusicOn ? musicOnIcon : musicOffIcon;
        }

        if (sfxButtonImage != null)
        {
            sfxButtonImage.sprite = isSFXOn ? sfxOnIcon : sfxOffIcon;
        }
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlaySound("ClickSound");
        pauseButton.gameObject.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySound("ClickSound");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.PlaySound("ClickSound");
        AudioManager.Instance.ToggleMusic();
        isMusicOn = !isMusicOn;

        if (musicButtonImage != null)
        {
            musicButtonImage.sprite = isMusicOn ? musicOnIcon : musicOffIcon;
        }
    }

    void ToggleSFX()
    {
        AudioManager.Instance.PlaySound("ClickSound");
        AudioManager.Instance.ToggleSFX();
        isSFXOn = !isSFXOn;

        if (sfxButtonImage != null)
        {
            sfxButtonImage.sprite = isSFXOn ? sfxOnIcon : sfxOffIcon;
        }
    }

    void PauseGame()
    {
        AudioManager.Instance.PlaySound("ClickSound");
        pauseButton.gameObject.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
