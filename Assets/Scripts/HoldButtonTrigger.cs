using UnityEngine;
using System.Collections.Generic;

public class HoldButtonTrigger : MonoBehaviour
{
    public MonoBehaviour[] linkedObjects;

    private HashSet<string> activeTags = new(); // Tracks who is pressing
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsValidInteractor(other))
        {
            if (activeTags.Add(other.tag))
            {
                if (animator != null)
                    animator.Play("ButtonPress");

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
                    animator.Play("ButtonRelease");

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

    private bool IsValidInteractor(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Always allow the player, even if frozen
            return true;
        }
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

    public bool HasPlayer() => activeTags.Contains("Player");
    public bool HasClone() => activeTags.Contains("Clone");
}
