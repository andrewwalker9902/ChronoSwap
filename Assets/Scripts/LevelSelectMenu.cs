using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    public Button[] levelButtons;

    void Start()
    {
        int unlocked = PlayerPrefs.GetInt("LastLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int level = i + 1;
            levelButtons[i].interactable = level <= unlocked;

            int captured = level;
            levelButtons[i].onClick.AddListener(() => GameManager.Instance.LoadLevel(captured));
        }
    }
}
