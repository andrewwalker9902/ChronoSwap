using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CloneRecorder : MonoBehaviour
{
    private Vector3 startPosition;
    private Rigidbody2D rb;
    public float recordRate = 0.02f;
    private float recordTimer;

    private List<Vector3> recordedPositions = new List<Vector3>();
    private int playbackIndex = 0;
    private bool isRecording = false;
    private bool isPlayingBack = false;

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
    }

    void FixedUpdate()
    {
        if (isPlayingBack && playbackIndex < recordedPositions.Count)
        {
            rb.MovePosition(recordedPositions[playbackIndex]);
            playbackIndex++;

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
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public void StartPlayback()
    {
        if (recordedPositions.Count == 0) return;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        isRecording = false;
        isPlayingBack = true;
        playbackIndex = 0;
    }

    public void Freeze()
    {
        isRecording = false;
        isPlayingBack = false;

        transform.position = startPosition;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Updated to use linearVelocity instead of velocity  
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }

        var cloneController = GetComponent<CloneController>();
        if (cloneController != null)
        {
            cloneController.isControllable = false;
        }
    }
}
