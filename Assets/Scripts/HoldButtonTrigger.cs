using UnityEngine;
using System.Collections.Generic;

public class HoldButtonTrigger : MonoBehaviour
{
    public MonoBehaviour[] linkedObjects;

    private Dictionary<string, float> activeTagTimes = new(); // Tracks who is pressing and when
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidInteractor(other))
        {
            if (!activeTagTimes.ContainsKey(other.tag))
            {
                activeTagTimes[other.tag] = Time.time;
                if (animator != null)
                    animator.Play("RedButtonPress");

                foreach (var obj in linkedObjects)
                {
                    IInteractable interactable = obj as IInteractable;
                    if (interactable != null)
                    {
                        interactable.NotifyPress(other.tag, Time.time);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidInteractor(other))
        {
            if (activeTagTimes.Remove(other.tag))
            {
                if (activeTagTimes.Count == 0 && animator != null)
                {
                    animator.Play("RedButtonRelease");

                    foreach (var obj in linkedObjects)
                    {
                        IInteractable interactable = obj as IInteractable;
                        if (interactable != null)
                        {
                            interactable.NotifyRelease(other.tag);
                        }
                    }
                }
            }
        }
    }

    public float GetLastPressTime()
    {
        // Return the most recent press time among all active tags
        float latest = -1000f;
        foreach (var t in activeTagTimes.Values)
        {
            if (t > latest)
                latest = t;
        }
        return latest;
    }

    public List<float> GetAllPressTimes()
    {
        return new List<float>(activeTagTimes.Values);
    }

    private bool IsValidInteractor(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return true;
        if (other.CompareTag("PushBox"))
            return true;
        if (other.CompareTag("Clone"))
        {
            var controller = other.GetComponent<CloneController>();
            if (controller != null && controller.isPlayingBack)
                return true;
        }
        return false;
    }

    public bool IsPressed()
    {
        return activeTagTimes.Count > 0;
    }

    public HashSet<string> GetActiveTags()
    {
        return new HashSet<string>(activeTagTimes.Keys);
    }

    public bool HasPlayer() => activeTagTimes.ContainsKey("Player");
    public bool HasClone() => activeTagTimes.ContainsKey("Clone");
    public bool HasPushBox() => activeTagTimes.ContainsKey("PushBox");
}
