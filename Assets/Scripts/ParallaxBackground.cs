using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect;

    private void Start()
    {
        startPos = transform.position.x;
    }

    private void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
