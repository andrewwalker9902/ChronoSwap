using UnityEngine;

public class StairPlatform : MonoBehaviour, IInteractable
{
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogWarning("StairPlatform has no Collider2D attached!");
        }
        else
        {
            col.enabled = false; // Make stairs non-collidable at the beginning
        }
    }

    public void NotifyPress(string tag, float timestamp)
    {
        if (col != null)
            col.enabled = true; // Enable collision
    }

    public void NotifyRelease(string tag)
    {
        if (col != null)
            col.enabled = false; // Disable collision
    }
}
