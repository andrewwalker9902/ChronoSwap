using UnityEngine;

public class TrapDoorController : MonoBehaviour, IInteractable
{
    private Collider2D trapdoorCollider;

    private void Awake()
    {
        trapdoorCollider = GetComponent<Collider2D>();
        if (trapdoorCollider == null)
        {
            Debug.LogError("TrapDoorController requires a Collider component.");
        }
    }

    private void Start()
    {
        if (trapdoorCollider != null)
            trapdoorCollider.enabled = true; // Solid by default

        if(trapdoorCollider == null)
            Debug.LogError("TrapDoorController: Collider is not set up correctly.");
    }

    public void NotifyPress(string tag, float timestamp)
    {
        if (trapdoorCollider != null)
        {
            trapdoorCollider.enabled = false; // Let objects fall through
            Debug.Log("Trapdoor opened (collider disabled).");
        }
    }

    public void NotifyRelease(string tag)
    {
        if (trapdoorCollider != null)
        {
            trapdoorCollider.enabled = true; // Block again
            Debug.Log("Trapdoor closed (collider enabled).");
        }
    }

    public void resetTrapdoor()
    {
        if (trapdoorCollider != null)
        {
            trapdoorCollider.enabled = true; // Reset to solid state
            Debug.Log("Trapdoor reset to solid state.");
        }
    }

    public static void resetAllTrapdoors()
    {
        TrapDoorController[] trapdoors = FindObjectsByType<TrapDoorController>(FindObjectsSortMode.None);
        foreach (var trapdoor in trapdoors)
        {
            trapdoor.resetTrapdoor();
        }
        Debug.Log("All trapdoors reset to solid state.");
    }
}
