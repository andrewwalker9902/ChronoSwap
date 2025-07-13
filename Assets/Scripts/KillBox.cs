using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the Player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ResetGame(); // Your existing reset method
            }
        }

        // Check if it's the Clone
        else if (other.CompareTag("Clone"))
        {
            // Destroy clone
            Destroy(other.gameObject);

            // Return camera and control to player
            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.ResetClone(); // Custom method to cleanly handle returning to player
            }
        }
    }
}
