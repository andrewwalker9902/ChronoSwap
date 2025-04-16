using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Match your gameplay scene name
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game!"); // Won't show outside of the editor

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode if in Unity Editor
        #endif
    }

    public void OpenSettings()
    {
        SceneContextManager.CameFrom = SceneContextManager.SourceScene.MainMenu;
        SceneManager.LoadScene("SettingsScene");
    }
}
