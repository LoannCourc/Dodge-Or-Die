using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void Restart()
    {
        AudioManager.Instance.PlaySound("ClickSound");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1f;
    }

    public void Menu()
    {
        AudioManager.Instance.PlaySound("ClickSound");
        Application.Quit();
    }
}
