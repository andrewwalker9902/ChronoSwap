using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    public Transform targetPoint;
    public float moveSpeed = 5f;
    public HoldButtonTrigger[] linkedButtons;
    public bool requireBoth = false;
    public bool requireTiming = false;
    public bool isToggleButton = false;
    
    public float pressWindow = 1f;

    private Vector3 startPos;
    public bool isMoving = false;
    private bool isOpen = false;

    private Collider2D col;

    void Start()
    {
        startPos = transform.position;
        col = GetComponentInChildren<Collider2D>();
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

    public void NotifyPress(string tag, float timestamp)
    {
        bool validCombo;
        if (!isToggleButton)
        {
            if (requireBoth && requireTiming)
            {
                validCombo = AreTwoButtonsPressedWithinWindow();
            }
            else if (requireBoth)
            {
                int pressedButtonCount = 0;
                foreach (var button in linkedButtons)
                {
                    if (button != null && button.IsPressed())
                        pressedButtonCount++;
                }
                validCombo = pressedButtonCount >= 2;
            }
            else
            {
                validCombo = true; // No requirements, always valid
            }
            if (validCombo && !isMoving)
            {
                isMoving = true;
                isOpen = !isOpen;
            }
        }
        if (isToggleButton)
        {
            if (isOpen)
                isOpen = false;
            else
                isOpen = true;
        }

    }



    // problem with NotifyRelease, its setting isMoving to false
    // anytime a button is released, even if the door only needs one
    // button to open it. 


    public void NotifyRelease(string tag)
    {
        if (!isToggleButton && !requireTiming && !requireBoth)
        {
            bool anyPressed = false;
            if (linkedButtons != null)
            {
                foreach (var button in linkedButtons)
                {
                    if (button != null && button.IsPressed())
                    {
                        anyPressed = true;
                        break;
                    }
                }
            }
            if (!anyPressed && isMoving)
            {
                isMoving = false;
                isOpen = false;
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                isOpen = false;
            }
        }

    }

    private bool AreTwoButtonsPressedWithinWindow()
    {
        if (linkedButtons == null || linkedButtons.Length < 2)
            return false;

        float firstTime = -1000f;
        float secondTime = -1000f;
        bool foundFirst = false;

        foreach (var button in linkedButtons)
        {
            if (button != null && button.IsPressed())
            {
                // Get the most recent press time for this button
                float buttonTime = button.GetLastPressTime();
                if (!foundFirst)
                {
                    firstTime = buttonTime;
                    foundFirst = true;
                }
                else
                {
                    secondTime = buttonTime;
                    // Check if within window
                    if (Mathf.Abs(firstTime - secondTime) <= pressWindow)
                        return true;
                }
            }
        }
        return false;
    }

    public void ResetDoor()
    {
        isMoving = false;
        isOpen = false;
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
