using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    public int currentLevel = 1; // Set this per level in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UnlockNextLevel(currentLevel);
            GameManager.Instance.LoadLevel(currentLevel + 1);
        }
    }
}
