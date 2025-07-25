using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        PlayerPrefs.DeleteKey("LastLevel");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level1");
    }

    public void OpenSettings()
    {
        SceneContextManager.CameFrom = SceneContextManager.SourceScene.MainMenu;
        SceneManager.LoadScene("SettingsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game!");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
