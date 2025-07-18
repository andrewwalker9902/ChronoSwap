using UnityEngine;
using System.Collections;

public class PistonController : MonoBehaviour, IInteractable
{
    public Transform targetPoint;
    public float resetmoveSpeed = 2f;
    public float smashMoveSpeed = 5f;
    public float delayAtEnds = 0.5f; // Delay in seconds at each end
    public HoldButtonTrigger[] linkedButtons;
    public bool requireBoth = false;
    public bool requireTiming = false;
    public bool isToggleButton = false;
    public bool smashesUpward = false; // Set in Inspector for each piston
    private bool atEndPoint = false;

    private Vector3 startPos;
    private Coroutine pistonRoutine;
    public bool poweredOn = true;
    private Rigidbody2D rb;
    private Vector3 moveTarget;
    private float moveSpeed;
    private bool isMoving = false;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        PowerOn();
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, moveTarget, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if ((rb.position - (Vector2)moveTarget).sqrMagnitude < 0.001f)
            {
                rb.MovePosition(moveTarget);
                isMoving = false;
            }
        }
    }

    IEnumerator PistonLoop()
    {
        while (true)
        {
            // Smash to target
            yield return StartCoroutine(MoveTo(targetPoint.position, smashMoveSpeed));
            yield return new WaitForSeconds(delayAtEnds);

            // Reset to start
            yield return StartCoroutine(MoveTo(startPos, resetmoveSpeed));
            yield return new WaitForSeconds(delayAtEnds);
        }
    }

    IEnumerator ResetToStart()
    {
        yield return StartCoroutine(MoveTo(startPos, resetmoveSpeed));
        pistonRoutine = null;
    }

    // Example method to start a move
    IEnumerator MoveTo(Vector3 destination, float speed)
    {
        moveTarget = destination;
        moveSpeed = speed;
        isMoving = true;
        while (isMoving)
            yield return new WaitForFixedUpdate();
        if (smashesUpward && destination == targetPoint.position)
            atEndPoint = true;
        else
            atEndPoint = false;
    }

    public void OnKillZoneTriggered(PistonKillZone.KillZoneType zoneType, GameObject player)
    {
        if (smashesUpward)
        {
            // Only kill if at endpoint and hit top
            if (zoneType == PistonKillZone.KillZoneType.Top && atEndPoint)
            {
                KillPlayer(player);
            }
        }
        else
        {
            // Downward piston: kill if hit bottom at any time
            if (zoneType == PistonKillZone.KillZoneType.Bottom)
            {
                KillPlayer(player);
            }
        }
    }

    private void KillPlayer(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            PlayerController player = obj.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ResetPlayerAtAnchor();
            }
        }
        else if (obj.CompareTag("Clone"))
        {
            Destroy(obj);

            // Return camera and control to the player
            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.ResetClone();
            }
        }
    }

    private void PowerOn()
    {
        poweredOn = true;
        if (pistonRoutine != null)
        {
            StopCoroutine(pistonRoutine);
            pistonRoutine = null;
        }
        pistonRoutine = StartCoroutine(PistonLoop());
    }

    private void PowerOff()
    {
        poweredOn = false;
        if (pistonRoutine != null)
        {
            StopCoroutine(pistonRoutine);
            pistonRoutine = null;
        }
        pistonRoutine = StartCoroutine(ResetToStart());
    }

    public void NotifyPress(string tag, float timestamp)
    {
        if (isToggleButton)
        {
            if (poweredOn)
                PowerOff();
            else
                PowerOn();
        }
        else
        {
            if (poweredOn)
                PowerOff();
        }
    }

    public void NotifyRelease(string tag)
    {
        if (!isToggleButton)
        {
            if (!poweredOn)
                PowerOn();
        }
        // Toggle button: do nothing on release
    }

    public void OnPlayerOnTop(GameObject player)
    {
        if (!smashesUpward)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }
        }
    }
}
