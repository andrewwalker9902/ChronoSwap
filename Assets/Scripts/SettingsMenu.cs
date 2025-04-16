using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public void GoBack()
    {
        switch (SceneContextManager.CameFrom)
        {
            case SceneContextManager.SourceScene.MainMenu:
                SceneManager.LoadScene("MainMenuScene");
                break;
            case SceneContextManager.SourceScene.Gameplay:
                SceneManager.LoadScene("SampleScene"); // your actual game scene
                break;
            default:
                Debug.LogWarning("Unknown source scene. Defaulting to main menu.");
                SceneManager.LoadScene("MainMenuScene");
                break;
        }
    }
}
