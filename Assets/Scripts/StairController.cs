using UnityEngine;

public class StairPlatform : MonoBehaviour, IInteractable
{
    private Collider2D col;

    public Sprite defaultSprite;
    public Sprite activatedSprite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogWarning("StairPlatform has no Collider2D attached!");
        }
        else
        {
            col.enabled = false; // Make stairs non-collidable at the beginning
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;

    }

    public void NotifyPress(string tag, float timestamp)
    {
        if (col != null)
            col.enabled = true; // Enable collision

        if (activatedSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = activatedSprite;
    }

    public void NotifyRelease(string tag)
    {
        if (col != null)
            col.enabled = false; // Disable collision

        if (defaultSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = defaultSprite;
    }
}
