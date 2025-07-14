using UnityEngine;
using System.Collections.Generic;

public class ButtonTrigger : MonoBehaviour
{
    public ElevatorMover[] linkedElevators;

    private HashSet<string> activeTags = new(); // Tracks who is pressing
    private Animator animator;

    private void Start()
    {
        // Get Animator from child (like Red part of button)
        animator = GetComponentInChildren<Animator>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            if (activeTags.Add(other.tag)) // Only act on new tag
            {
                // Play ButtonPress animation
                if (animator != null)
                    animator.Play("ButtonPress");

                foreach (var elevator in linkedElevators)
                    elevator.NotifyPress(other.tag, Time.time);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            if (activeTags.Remove(other.tag))
            {
                // Only play release if no one is left on the button
                if (activeTags.Count == 0 && animator != null)
                    animator.Play("ButtonRelease");

            }
        }
    }

    public bool HasPlayer() => activeTags.Contains("Player");
    public bool HasClone() => activeTags.Contains("Clone");
}
