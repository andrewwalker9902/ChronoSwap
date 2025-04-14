using UnityEngine;

public class PlatformEditor : MonoBehaviour
{
    public GameObject platformPrefab; // Assign a prefab with BoxCollider2D + SpriteRenderer
    public LayerMask drawingLayer;    // Optional: layer to ignore when drawing

    private bool editorMode = false;
    private Vector2 startPos;
    private GameObject currentPlatform;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            editorMode = !editorMode;
            Debug.Log("Editor Mode: " + (editorMode ? "ON" : "OFF"));
        }

        if (!editorMode) return;

        if (Input.GetMouseButtonDown(0))
        {
            startPos = GetMouseWorldPosition();
            currentPlatform = Instantiate(platformPrefab, startPos, Quaternion.identity);
        }

        if (Input.GetMouseButton(0) && currentPlatform != null)
        {
            Vector2 currentPos = GetMouseWorldPosition();
            Vector2 center = (startPos + currentPos) / 2f;
            Vector2 size = new Vector2(Mathf.Abs(currentPos.x - startPos.x), Mathf.Abs(currentPos.y - startPos.y));

            currentPlatform.transform.position = center;
            currentPlatform.transform.localScale = size;
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentPlatform = null;
        }

        // Right-click to delete a platform
        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            Vector2 mousePos = GetMouseWorldPosition();
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("DrawnPlatform"))
            {
                Destroy(hit.collider.gameObject);
            }
        }


    }

    Vector2 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = 10f; // distance from camera
        return Camera.main.ScreenToWorldPoint(mouseScreen);
    }
}
