using UnityEngine;

public class PushBoxController : MonoBehaviour, IInteractable
{
    private Vector3 startPos;
    private Quaternion startRot;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        startRot = Quaternion.identity; // Store the original rotation (0,0,0)
        rb = GetComponent<Rigidbody2D>();
    }

    public void NotifyPress(string tag, float timestamp)
    {
        ResetBoxPosition();
    }

    public void NotifyRelease(string tag)
    {
        // No action needed on release for push boxes
    }

    public void ResetBoxPosition()
    {
        Debug.Log($"[PushBox] Resetting to {startPos}");
        transform.position = startPos;
        transform.rotation = startRot; // Reset rotation to 0,0,0
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public static void ResetAllBoxPosition()
    {
        PushBoxController[] boxes = FindObjectsByType<PushBoxController>(FindObjectsSortMode.None);
        foreach (var box in boxes)
        {
            box.ResetBoxPosition();
        }
    }
}
