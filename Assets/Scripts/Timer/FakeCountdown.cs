using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FakeCountdown : MonoBehaviour
{
    [Header("UI Reference")]
    public Image panelBackgroundImage;
    public TextMeshProUGUI countdownText;

    [Header("Visual Style")]
    private Color normalColor = Color.white;
    private Color warningColor = Color.red;
    public AudioSource beepAudio;
    public AudioSource alarmAudio;

    // Timer settings
    private float minDuration = 5f;
    private float maxDuration = 10f;
    private float minDelay = 10f;
    private float maxDelay = 30f;

    private Coroutine timerCoroutine;

    private void Start()
    {
        countdownText.text = string.Empty;
        panelBackgroundImage.enabled = false;
        StartCoroutine(FakeTimerLoop());
    }

    private IEnumerator FakeTimerLoop()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            float duration = Random.Range(minDuration, maxDuration);
            StartFakeCountdown(duration);

            yield return new WaitForSeconds(duration + 1f);
        }
    }

    private void StartFakeCountdown(float duration)
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        timerCoroutine = StartCoroutine(FakeCountdownRoutine(duration));
        panelBackgroundImage.enabled = true;
    }

    private IEnumerator FakeCountdownRoutine(float duration)
    {
        float timeLeft = duration;
        countdownText.color = normalColor;
        countdownText.alpha = 1f;

        while (timeLeft > 0)
        {
            countdownText.text = $"{timeLeft:F0}";

            if (timeLeft <= 3f)
            {
                countdownText.color = warningColor;
            }

            if (beepAudio && timeLeft <= 3f)
            {
                beepAudio.Play();
            }

            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        countdownText.text = "0";
        countdownText.color = warningColor;
        alarmAudio.Play();
        yield return new WaitForSeconds(2.5f);

        countdownText.text = string.Empty;
        panelBackgroundImage.enabled = false;
    }
}