using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 15f;
    public float maxJumpTime = 0.5f;   // how long they can hold jump
    public float jumpHoldForce = 3f;   // extra upward force while holding

    private bool isJumping = false;
    private float jumpTimeCounter = 0f;


    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Clone Settings")]
    public GameObject clonePrefab;
    private GameObject activeClone;
    private CloneRecorder cloneRecorder;
    private bool isControllingClone = false;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 startingPosition;
    private bool isFrozen = false;
    private Vector3 respawnPoint;

    private CameraFollow camFollow;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        respawnPoint = startingPosition;
        camFollow = Camera.main.GetComponent<CameraFollow>();
        camFollow.SetTarget(transform); // Start following player
    }

    public void SetRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }

    void Update()
    {
        HandleInputs();

        if (!isControllingClone && !isFrozen)
        {
            HandleMovement();
        }
    }

    void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isControllingClone)
            {
                EnterCloneMode();
            }
            else
            {
                ExitCloneMode();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StartClonePlayback();
        }
    }


    void EnterCloneMode()
    {
        if (activeClone != null)
        {
            Destroy(activeClone);
            activeClone = null;
            cloneRecorder = null;
        }

        activeClone = Instantiate(clonePrefab, transform.position, Quaternion.identity);
        cloneRecorder = activeClone.GetComponent<CloneRecorder>();
        cloneRecorder.StartRecording();
        camFollow.SetTarget(activeClone.transform);

        CloneController cloneController = activeClone.GetComponent<CloneController>();
        if (cloneController != null)
        {
            cloneController.isControllable = true;
        }

        Rigidbody2D cloneRB = activeClone.GetComponent<Rigidbody2D>();
        if (cloneRB != null)
        {
            cloneRB.bodyType = RigidbodyType2D.Dynamic;
            cloneRB.linearVelocity = Vector2.zero;
        }

        isControllingClone = true;
        FreezePlayer();
    }

    void ExitCloneMode()
    {
        if (cloneRecorder != null)
        {
            cloneRecorder.StopRecording();
            cloneRecorder.Freeze();
        }

        isControllingClone = false;
        UnfreezePlayer();
        camFollow.SetTarget(transform);
    }

    void StartClonePlayback()
    {
        if (cloneRecorder != null)
        {
            cloneRecorder.StartPlayback();
        }
    }

    void FreezePlayer()
    {
        isFrozen = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    void UnfreezePlayer()
    {
        isFrozen = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        camFollow.SetTarget(transform);
        rb.gravityScale = 1f; // Or use `originalGravity` if you stored it
    }


    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHoldForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }


    }

    public void ResetPlayerAtAnchor()
    {
        // Reset velocity to prevent momentum carry-over
        rb.linearVelocity = Vector2.zero;

        // Destroy clone if it exists
        if (activeClone != null)
        {
            Destroy(activeClone);
            activeClone = null;
            cloneRecorder = null;
        }

        ElevatorMover.ResetAllElevators();

        // Reset player position
        transform.position = respawnPoint;

        // Unfreeze player
        UnfreezePlayer();

        // Reset state
        isControllingClone = false;
        camFollow.SetTarget(transform);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void ResetClone()
    {
        isControllingClone = false;

        // Make sure player is unfrozen
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        camFollow.SetTarget(transform);

        // Clear clone reference
        activeClone = null;
        cloneRecorder = null;

        UnfreezePlayer();
    }

    public void CheckCrush()
    {
        // Get all colliders overlapping the player
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(
            GetComponent<Collider2D>().bounds.center,
            GetComponent<Collider2D>().bounds.size * 0.95f, // slightly smaller to avoid false positives
            0f
        );

        bool touchingGround = false;
        bool touchingMoving = false;

        foreach (var col in overlaps)
        {
            if (col == null || col == GetComponent<Collider2D>())
                continue;
            if (col.CompareTag("Ground"))
                touchingGround = true;
            // Tag all moving objects (pistons, doors, etc.) as "Moving"
            if (col.CompareTag("Moving"))
                touchingMoving = true;
        }

        if (touchingGround && touchingMoving)
        {
            ResetPlayerAtAnchor();
        }
    }

}
