using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int maxLevel = 5; // Set this to however many levels your game has

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate managers
        }
    }

    public void UnlockNextLevel(int currentLevel)
    {
        int savedLevel = PlayerPrefs.GetInt("LastLevel", 1);
        if (currentLevel >= savedLevel && currentLevel < maxLevel)
        {
            PlayerPrefs.SetInt("LastLevel", currentLevel + 1);
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.SetInt("LastLevel", 1);
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
