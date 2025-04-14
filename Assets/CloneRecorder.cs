using System.Collections.Generic;
using UnityEngine;

public class CloneRecorder : MonoBehaviour
{

    private Vector3 startPosition;
    private Rigidbody2D rb;
    public float recordRate = 0.02f;
    private float playbackTimer = 0f;


    private List<Vector3> recordedPositions = new List<Vector3>();
    private int playbackIndex = 0;
    private bool isRecording = false;
    private bool isPlayingBack = false;

    private float recordTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isRecording)
        {
            recordTimer += Time.deltaTime;
            if (recordTimer >= recordRate)
            {
                recordedPositions.Add(transform.position);
                recordTimer = 0f;
            }
        }
        else if (isPlayingBack && recordedPositions.Count > 0)
        {
            playbackTimer += Time.deltaTime;

            if (playbackTimer >= recordRate)
            {
                transform.position = recordedPositions[playbackIndex];
                playbackIndex++;
                playbackTimer = 0f;
            }

            if (playbackIndex >= recordedPositions.Count)
            {
                isPlayingBack = false;
            }
        }

    }

    public void StartRecording()
    {
        startPosition = transform.position;
        recordedPositions.Clear();
        isRecording = true;
        isPlayingBack = false;
        playbackIndex = 0;

        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public void StartPlayback()
    {
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;

        isPlayingBack = true;
        playbackIndex = 0;
        playbackTimer = 0f;
    }

    public void Freeze()
    {
        isRecording = false;
        isPlayingBack = false;

        // Reset position to starting point
        transform.position = startPosition;

        // Fully stop motion
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Make sure controller is disabled
        CloneController cloneController = GetComponent<CloneController>();
        if (cloneController != null)
        {
            cloneController.isControllable = false;
        }
    }


}
