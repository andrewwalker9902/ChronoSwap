using UnityEngine;

public class AcidRainController : MonoBehaviour, IInteractable
{
    public ParticleSystem rainParticles;
    public float fadeDuration = 1.5f;
    public bool isToggleButton = false;

    public HoldButtonTrigger[] linkedButtons;
    public bool requireBoth = false;
    public bool requireTiming = false;
    public float pressWindow = 1f;

    public bool isActive = true;
    private float fadeTimer = 0f;
    private ParticleSystem.EmissionModule emission;
    private Collider2D killZoneTrigger;

    private void Start()
    {
        killZoneTrigger = GetComponent<Collider2D>();

        if (rainParticles != null)
        {
            emission = rainParticles.emission;
            emission.rateOverTime = 100f;
        }

        SetRainActive(true); // Or false if you want it off by default
    }

    public void NotifyPress(string tag, float timestamp)
    {
        if (requireBoth && requireTiming)
        {
            if (AreTwoButtonsPressedWithinWindow())
            {
                SetRainActive(false); // Turn off rain when both buttons pressed within window
                return;
            }
        }
        else if (requireBoth)
        {
            int pressedButtonCount = 0;
            foreach (var button in linkedButtons)
            {
                if (button != null && button.IsPressed())
                    pressedButtonCount++;
            }
            if (pressedButtonCount < 2)
                return; // Not enough buttons pressed
        }

        if (isToggleButton)
        {
            SetRainActive(!isActive); // Toggle on press
        }
        else
        {
            SetRainActive(false); // Hold: turn off while pressed
        }
    }

    public void NotifyRelease(string tag)
    {
        if (requireBoth && requireTiming)
        {
            // Optionally, you could turn rain back on when either button is released
            // SetRainActive(true);
            return;
        }

        if (!isToggleButton)
        {
            SetRainActive(true); // Hold: turn on when released
        }
        // Toggle: do nothing on release
    }

    private bool AreTwoButtonsPressedWithinWindow()
    {
        if (linkedButtons == null || linkedButtons.Length < 2)
            return false;

        float firstTime = -1000f;
        float secondTime = -1000f;
        bool foundFirst = false;

        foreach (var button in linkedButtons)
        {
            if (button != null && button.IsPressed())
            {
                float buttonTime = button.GetLastPressTime();
                if (!foundFirst)
                {
                    firstTime = buttonTime;
                    foundFirst = true;
                }
                else
                {
                    secondTime = buttonTime;
                    if (Mathf.Abs(firstTime - secondTime) <= pressWindow)
                        return true;
                }
            }
        }
        return false;
    }

    public void SetRainActive(bool active)
    {
        isActive = active;

        if (killZoneTrigger != null)
            killZoneTrigger.enabled = active;

        if (!active)
        {
            fadeTimer = fadeDuration;
        }
        else
        {
            if (rainParticles != null)
                emission.rateOverTime = 100f; // Reset if re-enabled
        }
    }

    private void Update()
    {
        if (!isActive && fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime;
            float t = fadeTimer / fadeDuration;
            emission.rateOverTime = Mathf.Lerp(0f, 100f, t);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ResetPlayerAtAnchor();
            }
        }
    }
}
