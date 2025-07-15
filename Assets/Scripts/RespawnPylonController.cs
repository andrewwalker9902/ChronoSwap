using UnityEngine;

public class RespawnPylonController : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite activatedSprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetRespawnPoint(transform.position);

                if (activatedSprite != null && spriteRenderer != null)
                    spriteRenderer.sprite = activatedSprite;
            }
        }
    }
}
