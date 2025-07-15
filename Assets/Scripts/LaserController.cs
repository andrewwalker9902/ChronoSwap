using UnityEngine;

public class LaserController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the Player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ResetPlayerAtAnchor(); // Your existing reset method
            }
        }
    }

}
