using UnityEngine;

public class ElevatorMover : MonoBehaviour, IInteractable
{
    public Transform targetPoint;
    public float moveSpeed = 2f;
    public HoldButtonTrigger[] linkedButtons;
    public bool requireBoth = false;
    public bool requireTiming = false;

    private float lastPlayerPressTime = -10f;
    private float lastClonePressTime = -10f;
    private float lastPushBoxPressTime = -10f;
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
    }

    // Implements IInteractable
    public void NotifyPress(string tag, float timestamp)
    {
        if (tag == "Player")
            lastPlayerPressTime = timestamp;
        else if (tag == "Clone")
            lastClonePressTime = timestamp;
        else if (tag == "PushBox")
            lastPushBoxPressTime = timestamp;

        bool withinWindow = Mathf.Abs(lastPlayerPressTime - lastClonePressTime) <= pressWindow;
        bool validCombo;
        if (requireBoth && requireTiming)
        {
            validCombo = withinWindow;
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
        else if (requireTiming)
        {
            validCombo = withinWindow;
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
        lastPlayerPressTime = -10f;
        lastClonePressTime = -10f;
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
