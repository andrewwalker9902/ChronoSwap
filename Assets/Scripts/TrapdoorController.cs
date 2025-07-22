using UnityEngine;

public class TrapDoorController : MonoBehaviour, IInteractable
{
    public float openDuration = 0.25f; // How quickly it swings open
    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private float lerpTime = 0f;

    private void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0f, 0f, -90f); // 90° swing on Z axis
    }

    public void NotifyPress(string tag, float timestamp)
    {
        if (!isOpen)
            StartCoroutine(SwingOpen());
    }

    public void NotifyRelease(string tag)
    {
        if (isOpen)
            StartCoroutine(SwingClose());
    }

    private System.Collections.IEnumerator SwingOpen()
    {
        isOpen = true;
        lerpTime = 0f;

        while (lerpTime < openDuration)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / openDuration;
            transform.localRotation = Quaternion.Lerp(closedRotation, openRotation, t);
            yield return null;
        }

        transform.localRotation = openRotation;
    }

    private System.Collections.IEnumerator SwingClose()
    {
        lerpTime = 0f;
        while (lerpTime < openDuration)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / openDuration;
            transform.localRotation = Quaternion.Lerp(openRotation, closedRotation, t);
            yield return null;
        }
        transform.localRotation = closedRotation;
        isOpen = false;
    }
}
