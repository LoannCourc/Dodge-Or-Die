using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button quitButton;
    public Button musicButton;
    public Button pauseButton;

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        musicButton.onClick.AddListener(ToggleMusic);
        pauseButton.onClick.AddListener(Pause);
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