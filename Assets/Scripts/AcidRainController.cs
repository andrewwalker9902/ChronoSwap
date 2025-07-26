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
        bool validCombo;
        if (!isToggleButton)
        {
            if (requireBoth && requireTiming)
            {
                validCombo = AreTwoButtonsPressedWithinWindow();
            }
            else if (requireBoth)
            {
                int pressedButtonCount = 0;
                foreach (var button in linkedButtons)
                {
                    if (button != null && button.IsPressed())
                        pressedButtonCount++;
                }
                validCombo = pressedButtonCount >= 2;
            }
            else
            {
                validCombo = true; // No requirements, always valid
            }
            if (validCombo)
            {
                SetRainActive(false); // Deactivate acid rain
            }
        }
        if (isToggleButton)
        {
            SetRainActive(!isActive);
        }
    }

    public void NotifyRelease(string tag)
    {
        if (!isToggleButton && !requireTiming && !requireBoth)
        {
            bool anyPressed = false;
            if (linkedButtons != null)
            {
                foreach (var button in linkedButtons)
                {
                    if (button != null && button.IsPressed())
                    {
                        anyPressed = true;
                        break;
                    }
                }
            }
            if (!anyPressed)
            {
                SetRainActive(true); // Only turn on if all buttons are released
            }
        }
        else
        {
            SetRainActive(true); // Turn on immediately for other modes
        }
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
                // Get the most recent press time for this button
                float buttonTime = button.GetLastPressTime();
                if (!foundFirst)
                {
                    firstTime = buttonTime;
                    foundFirst = true;
                }
                else
                {
                    secondTime = buttonTime;
                    // Check if within window
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

    public void resetAcidRain()
    {
       SetRainActive(true);
       fadeTimer = 0f;
        if (rainParticles != null)
            emission.rateOverTime = 100f; // Reset emission rate
    }

    public static void resetAllAcidRains()
    {
        AcidRainController[] acidRains = FindObjectsByType<AcidRainController>(FindObjectsSortMode.None);
        foreach (var acidRain in acidRains)
        {
            if (acidRain != null)
            {
                acidRain.resetAcidRain();
            }
        }
    }
}
