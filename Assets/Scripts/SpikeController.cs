using UnityEngine;

public class SpikeController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the Player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ResetGame(); // Reuse your existing reset logic
            }
        }

        // Check if it's the Clone
        else if (other.CompareTag("Clone"))
        {
            // Destroy the clone
            Destroy(other.gameObject);

            // Return camera and control to the player
            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.ResetClone(); // Cleanly return to player control
            }
        }
    }
}
