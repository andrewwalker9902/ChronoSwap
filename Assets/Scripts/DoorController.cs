using UnityEngine;
using System.Collections.Generic;

public class DoorController : MonoBehaviour, IInteractable
{
    private Collider2D col;
    private Animator animator;
    public bool isOpen = false;

    public bool isToggleButton = false;

    public HoldButtonTrigger[] linkedButtons;
    public bool requireBoth = false;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private bool IsValidCombo()
    {
        if (!requireBoth || linkedButtons == null || linkedButtons.Length < 2)
            return true;

        // Gather which tags are pressing each button
        List<HashSet<string>> pressedTags = new();
        foreach (var button in linkedButtons)
        {
            if (button != null && button.IsPressed())
            {
                var tags = button.GetActiveTags();
                if (tags.Count > 0)
                    pressedTags.Add(tags);
            }
        }

        // Must have exactly two pressed buttons
        if (pressedTags.Count != 2)
            return false;

        // Flatten tags and check for two unique entities
        HashSet<string> allTags = new();
        foreach (var tags in pressedTags)
            foreach (var tag in tags)
                allTags.Add(tag);

        return allTags.Count == 2;
    }

    public void NotifyPress(string tag, float timestamp)
    {
        Debug.Log($"Door received press from {tag} at {timestamp}, currently open: {isOpen}");

        if (isToggleButton)
        {
            // Toggle door on every press
            if (isOpen)
                CloseDoor();
            else
                OpenDoor();
            return;
        }

        bool validCombo = IsValidCombo();

        if (validCombo && !isOpen)
        {
            OpenDoor();
        }
        else if (!validCombo && isOpen)
        {
            CloseDoor();
        }
    }

    public void NotifyRelease(string tag)
    {
        bool validCombo = IsValidCombo();

        if (!validCombo && isOpen)
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        if (animator != null)
            animator.SetBool("Open", true);
        if (col != null)
            col.enabled = false;
        isOpen = true;
    }

    private void CloseDoor()
    {
        if (animator != null)
            animator.SetBool("Open", false);
        if (col != null)
            col.enabled = true;
        isOpen = false;
    }
}
