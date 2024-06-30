#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HighScoreEditor
{
    static HighScoreEditor()
    {
        EditorApplication.quitting += ResetHighScoresOnQuit;
    }

    private static void ResetHighScoresOnQuit()
    {
        Debug.Log("Editor is quitting. Resetting high scores.");
        // Votre code pour réinitialiser les scores ici, par exemple :
        PlayerPrefs.DeleteAll(); // Attention : cela supprimera toutes les PlayerPrefs, pas seulement les highscores.
        PlayerPrefs.Save();
    }
}
#endif