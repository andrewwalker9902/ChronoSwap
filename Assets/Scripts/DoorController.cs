using UnityEngine;
using System.Collections.Generic;

public class DoorController : MonoBehaviour, IInteractable
{
    public Transform targetPoint;
    public float moveSpeed = 5f;
    public HoldButtonTrigger[] linkedButtons;
    public bool requireBoth = false;
    public bool requireTiming = false;
    public bool isToggleButton = false;

    private Vector3 startPos;
    private bool isMoving = false;
    private bool isOpen = false;

    private Collider2D col;
    private Animator animator;

    void Start()
    {
        startPos = transform.position;
        col = GetComponentInChildren<Collider2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Vector3 destination = isOpen ? targetPoint.position : startPos;

        if (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }

        // After moving, check for crush on all overlapping players/clones
        if (col != null)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                col.bounds.center,
                col.bounds.size,
                0f
            );

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    PlayerController pc = hit.GetComponent<PlayerController>();
                    if (pc != null)
                        pc.CheckCrush();
                }
                else if (hit.CompareTag("Clone"))
                {
                    // If your clone has a similar crush check, call it here
                    // CloneController cc = hit.GetComponent<CloneController>();
                    // if (cc != null) cc.CheckCrush();
                }
            }
        }
    }

    private bool IsValidCombo()
    {
        if (!requireBoth || linkedButtons == null || linkedButtons.Length < 2)
            return true;

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

        if (pressedTags.Count != 2)
            return false;

        HashSet<string> allTags = new();
        foreach (var tags in pressedTags)
            foreach (var tag in tags)
                allTags.Add(tag);

        return allTags.Count == 2;
    }

    public void NotifyPress(string tag, float timestamp)
    {
        if (isToggleButton)
        {
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
        isOpen = true;
        if (animator != null)
            animator.SetBool("Open", true);
        if (col != null)
            col.enabled = false;
    }

    private void CloseDoor()
    {
        isOpen = false;
        if (animator != null)
            animator.SetBool("Open", false);
        if (col != null)
            col.enabled = true;
    }

    public void ResetDoor()
    {
        isOpen = false;
        if (animator != null)
        {
            animator.SetBool("Open", false);
            animator.Play("DoorIdle", 0, 0f);
        }
        if (col != null)
            col.enabled = true;
        transform.position = startPos;
    }

    public static void ResetAllDoors()
    {
        DoorController[] doors = FindObjectsByType<DoorController>(FindObjectsSortMode.None);
        foreach (var door in doors)
        {
            door.ResetDoor();
        }
    }
}
