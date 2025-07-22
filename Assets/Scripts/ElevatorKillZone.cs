using UnityEngine;

public class ElevatorKillZone : MonoBehaviour
{
    public ElevatorMover elevator;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.CheckCrush();
        }
        // If you want to support clones, add similar logic for them here
    }
}