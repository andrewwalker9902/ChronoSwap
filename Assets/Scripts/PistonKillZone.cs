using UnityEngine;

public class PistonKillZone : MonoBehaviour
{
    public enum KillZoneType { Top, Bottom }
    public KillZoneType zoneType;
    public PistonController piston;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            piston.OnKillZoneTriggered(zoneType, other.gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (zoneType == KillZoneType.Top && other.CompareTag("Player"))
        {
            piston.OnPlayerOnTop(other.gameObject);
        }
    }
}