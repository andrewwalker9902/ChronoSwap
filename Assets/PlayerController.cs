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

    private CameraFollow camFollow;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        camFollow = Camera.main.GetComponent<CameraFollow>();
        camFollow.SetTarget(transform); // Start following player
    }

    void Update()
    {
        HandleInputs();

        if (!isControllingClone)
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

                if (activeClone != null)
                {
                    Destroy(activeClone);
                    activeClone = null;
                    cloneRecorder = null;
                }
                // Enter clone mode
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

                // Freeze player
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                // Exit clone mode
                if (cloneRecorder != null)
                {
                    cloneRecorder.StopRecording();
                    cloneRecorder.Freeze();
                }

                isControllingClone = false;

                // Unfreeze player
                rb.bodyType = RigidbodyType2D.Dynamic;
                camFollow.SetTarget(transform);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cloneRecorder != null)
            {
                cloneRecorder.StartPlayback();
            }
        }
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

    public void ResetGame()
    {
        // Destroy clone if it exists
        if (activeClone != null)
        {
            Destroy(activeClone);
            activeClone = null;
            cloneRecorder = null;
        }

        // Reset player position
        transform.position = startingPosition;

        // Unfreeze player
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Reset state
        isControllingClone = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
