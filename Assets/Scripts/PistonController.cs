using UnityEngine;
using System.Collections;

public class PistonController : MonoBehaviour, IInteractable
{
    public Transform targetPoint;
    public float resetmoveSpeed = 2f;
    public float smashMoveSpeed = 5f;
    public float delayAtEnds = 0.5f;
    public bool isToggleButton = false;
    public bool startPoweredOn = true; // <-- Add this line

    private Vector3 startPos;
    private Coroutine pistonRoutine;
    private bool poweredOn = true;
    private Rigidbody2D rb;
    private Vector3 moveTarget;
    private float moveSpeed;
    private bool isMoving = false;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        if (startPoweredOn)
            PowerOn();
        else
            PowerOff();
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

            // After moving, check for crush on all overlapping players/clones
            Collider2D pistonCol = GetComponent<Collider2D>();
            if (pistonCol != null)
            {
                Collider2D[] hits = Physics2D.OverlapBoxAll(
                    pistonCol.bounds.center,
                    pistonCol.bounds.size,
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

    IEnumerator MoveTo(Vector3 destination, float speed)
    {
        moveTarget = destination;
        moveSpeed = speed;
        isMoving = true;
        while (isMoving)
            yield return new WaitForFixedUpdate();
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
    }

    public void PlayerOnTop(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }
}
