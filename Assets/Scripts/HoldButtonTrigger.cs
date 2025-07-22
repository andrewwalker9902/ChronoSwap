using UnityEngine;
using System.Collections.Generic;

public class HoldButtonTrigger : MonoBehaviour
{
    public MonoBehaviour[] linkedObjects;

    private HashSet<string> activeTags = new(); // Tracks who is pressing
    private Animator animator;
    private float lastPressTime = -1000f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidInteractor(other))
        {
            if (activeTags.Add(other.tag))
            {
                lastPressTime = Time.time;
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
            if (activeTags.Remove(other.tag))
            {
                if (activeTags.Count == 0 && animator != null)
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
        return lastPressTime;
    }

    private bool IsValidInteractor(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Always allow the player, even if frozen
            return true;
        }
        if (other.CompareTag("PushBox"))
            return true;
        else if (other.CompareTag("Clone"))
        {
            var controller = other.GetComponent<CloneController>();
            if (controller != null && controller.isPlayingBack)
            {
                // Only allow clone interaction during playback
                return true;
            }
        }

        return false;
    }

    public bool IsPressed()
    {
        return activeTags.Count > 0;
    }

    public HashSet<string> GetActiveTags()
    {
        return new HashSet<string>(activeTags);
    }

    public bool HasPlayer() => activeTags.Contains("Player");
    public bool HasClone() => activeTags.Contains("Clone");
    public bool HasPushBox() => activeTags.Contains("PushBox");
}
