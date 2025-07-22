using UnityEngine;

public class ElevatorMover : MonoBehaviour, IInteractable
{
    public Transform targetPoint;
    public float moveSpeed = 2f;
    public HoldButtonTrigger[] linkedButtons;
    public bool requireBoth = false;
    public bool requireTiming = false;

    public float pressWindow = 1f;

    private Vector3 startPos;
    private bool isMoving = false;
    private bool goingUp = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 destination = goingUp ? targetPoint.position : startPos;

        if (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }

        // After moving, check for crush on all overlapping players/clones
        Collider2D elevatorCol = GetComponent<Collider2D>();
        if (elevatorCol != null)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                elevatorCol.bounds.center,
                elevatorCol.bounds.size,
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

    // Implements IInteractable
    public void NotifyPress(string tag, float timestamp)
    {
        bool validCombo;
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
            goingUp = !goingUp;
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

    // Implements IInteractable
    public void NotifyRelease(string tag)
    {

        if (isMoving)
        {
            isMoving = false;
            goingUp = false;
        }
    }

    public void ResetElevator()
    {
        isMoving = false;
        goingUp = false;
        transform.position = startPos;
    }

    public static void ResetAllElevators()
    {
        ElevatorMover[] elevators = FindObjectsByType<ElevatorMover>(FindObjectsSortMode.None);
        foreach (ElevatorMover elevator in elevators)
        {
            elevator.ResetElevator();
        }
    }
}
