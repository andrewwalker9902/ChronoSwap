using UnityEngine;

public class PistonKillZone : MonoBehaviour
{
    public PistonController piston;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.CheckCrush();

            // Prevent upward launch if standing on top of piston
            if (piston != null)
                piston.PlayerOnTop(other.gameObject);
        }
        // If you want to support clones, add similar logic for them here
    }
}