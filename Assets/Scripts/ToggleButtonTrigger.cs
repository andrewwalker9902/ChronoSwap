using UnityEngine;

public class ToggleButtonTrigger : MonoBehaviour
{
    public MonoBehaviour[] linkedObjects;
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidInteractor(other))
        {
            if (animator != null)
                animator.Play("GreenButtonPress");

            foreach (var obj in linkedObjects)
            {
                if (obj is IInteractable interactable)
                {
                    interactable.NotifyPress(other.tag, Time.time); // Toggle door state
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidInteractor(other))
        {
            if (animator != null)
                animator.Play("GreenButtonRelease");
            // No door logic here
        }
    }


    private bool IsValidInteractor(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return true;
        if (other.CompareTag("PushBox"))
            return true;
        if (other.CompareTag("Clone"))
        {
            var controller = other.GetComponent<CloneController>();
            if (controller != null && controller.isPlayingBack)
                return true;
        }
        return false;
    }
}
