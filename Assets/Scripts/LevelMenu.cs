using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{

    public Button[] levelButtons; // Array of buttons for each level

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("LastLevel", 1); // Get the last unlocked level
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            levelButtons[i].interactable = true; // Enable buttons for unlocked levels
        }
    }

    public void OpenLevel(int levelID)
    {
        string levelName = "Level" + levelID; 
        SceneManager.LoadScene(levelName);
    }
}
