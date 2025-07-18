using UnityEngine;

public class PushBoxController : MonoBehaviour, IInteractable
{
    private Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        transform.position = startPos;
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
