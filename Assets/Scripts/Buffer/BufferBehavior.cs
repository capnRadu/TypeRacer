using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BufferBehavior : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI messageText;
    public Slider progressBar;
    public Image backgroundImage;

    // Progress settings
    private float fillSpeed = 0.2f;
    private float decaySpeed = 0.15f;

    private float flashSpeed = 2f;

    private string requiredInput;
    private float progress = 0f;
    private bool active = true;
    private Typer typer;

    private readonly string[] inputs = { "E", "Space", "Right Mouse Button", "Left Mouse Button" };

    private void Update()
    {
        if (!active) return;

        HandleInput();
        UpdateProgressBar();
    }

    public void Setup(Typer typerRef)
    {
        typer = typerRef;
        typer.CanType = false;

        requiredInput = inputs[Random.Range(0, inputs.Length)];
        messageText.text = $"{GetDeviceMessage()} Please press [{requiredInput}] to fix...";

        StartCoroutine(FlashBackground());
    }

    private void HandleInput()
    {
        bool pressed = false;

        if (requiredInput == "E" && Input.GetKeyDown(KeyCode.E)) pressed = true;
        else if (requiredInput == "Space" && Input.GetKeyDown(KeyCode.Space)) pressed = true;
        else if (requiredInput == "Right Mouse Button" && Input.GetMouseButtonDown(0)) pressed = true;
        else if (requiredInput == "Left Mouse Button" && Input.GetMouseButtonDown(1)) pressed = true;

        if (pressed)
        {
            progress += fillSpeed;
        }
        else
        {
            progress -= decaySpeed * Time.deltaTime;
        }

        progress = Mathf.Clamp01(progress);

        if (progress >= 1f)
        {
            StartCoroutine(ResolveErrorRoutine());
        }
    }

    private void UpdateProgressBar()
    {
        progressBar.value = Mathf.Lerp(progressBar.value, progress, 8f * Time.deltaTime);
    }

    private IEnumerator FlashBackground()
    {
        float t = 0f;
        Color colorA = new Color(1f, 1f, 1f, 1f);
        Color colorB = new Color(1f, 0.2f, 0.2f, 1f);

        while (active)
        {
            t += Time.deltaTime * flashSpeed;
            backgroundImage.color = Color.Lerp(colorA, colorB, (Mathf.Sin(t) + 1f) / 2f);
            yield return null;
        }
    }

    private IEnumerator ResolveErrorRoutine()
    {
        active = false;
        messageText.text = "Reconnecting...";
        progressBar.gameObject.SetActive(false);
        backgroundImage.color = new Color(0.47f, 0.47f, 0.47f, 1f);

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));

        messageText.text = "Device reconnected successfully.";
        backgroundImage.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.8f);

        typer.CanType = true;
        Destroy(gameObject);
    }

    private string GetDeviceMessage()
    {
        string[] messages =
        {
            "Keyboard not detected.",
            "Mouse disconnected.",
            "Peripheral driver failure.",
            "Input device timeout.",
            "Signal lost from primary device."
        };

        return messages[Random.Range(0, messages.Length)];
    }
}