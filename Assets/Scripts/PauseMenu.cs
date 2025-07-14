using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;
    public PlayerController playerController;

    private bool isPaused = false;

    void Start()
    {
        Debug.Log("PauseMenuUI.Start() ran");
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false); // Hide panel at game start
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        playerController.ResetGame(); // Call your existing method

        ElevatorMover.ResetAllElevators();

        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode if in Unity Editor
        #endif
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        pauseMenuUI.SetActive(false); // hide pause menu
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        pauseMenuUI.SetActive(true); // return to pause menu
    }

}
