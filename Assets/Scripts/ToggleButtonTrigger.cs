using UnityEngine;
using System.Collections.Generic;

public class ToggleButtonTrigger : MonoBehaviour
{
    public MonoBehaviour[] linkedObjects;

    private HashSet<string> activeTags = new(); // Tracks who is pressing
    private bool toggleState = false;
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
                    animator.Play("GreenButtonPress");

                toggleState = !toggleState;

                foreach (var obj in linkedObjects)
                {
                    IInteractable interactable = obj as IInteractable;
                    if (interactable != null)
                    {
                        if (toggleState)
                            interactable.NotifyPress(other.tag, Time.time);  // Turn ON
                        else
                            interactable.NotifyRelease(other.tag);           // Turn OFF
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
                    animator.Play("GreenButtonRelease");
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
