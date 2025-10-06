using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all typing logic, statistics, stability effects, and gameplay feedback
/// </summary>
public class Typer : MonoBehaviour
{
    [Header("Core References")]
    public WordBank wordBank;
    public StabilityMeter stabilityMeter;
    public GameObject typedLetterParentObject;
    public TextMeshProUGUI wordOutput;

    [Header("UI Elements")]
    public Image capsLockIcon;
    public Image flashImage;
    public TextMeshProUGUI wpmOutput;
    public TextMeshProUGUI mistakesOutput;
    public TextMeshProUGUI accuracyOutput;
    public TextMeshProUGUI wordsCompletedOutput;
    public TextMeshProUGUI abilityText;
    public GameObject typedLetterPrefab;

    [Header("Audio")]
    public AudioSource doublePointsKeystrokeAudio;
    public AudioSource focusBurstAudio;
    public AudioSource textCompleteAudio;
    public AudioSource correctKeystrokeAudio;
    public AudioSource wrongKeystrokeAudio;

    // Caps lock detection (Windows only)
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [System.Runtime.InteropServices.DllImport("USER32.dll")]
    public static extern short GetKeyState(int nVirtKey);
    bool IsCapsLockOn => (GetKeyState(0x14) & 1) > 0;
#endif

    // Word typing state
    private string remainingWord = string.Empty;
    private string currentWord = string.Empty;
    private Vector3 originalTextPos;

    // Statistics
    private int totalCharactersTyped;
    private int totalMistakes;
    private int correctCharactersTyped;
    private int wordsCompleted;
    private float startTime;
    private const int AverageWordLength = 5;
    private float elapsedTime => Time.time - startTime;

    // Idle stability drain
    private float baseIdleDelay = 0.5f;
    private float minIdleDelay = 0.1f;
    private float baseDrainRate = 0.5f;
    private float minDrainRate = 0.1f;
    private float lastTypeTime;
    private float idleTimer;

    // Image flash and text shake
    private float flashDuration = 0.2f;
    private Coroutine flashCoroutine;
    private Coroutine textShakeCoroutine;

    // Focus burst
    private int focusBurstCombo;
    private bool isInLowStabilityZone;
    private bool focusBurstActive;

    private void Awake()
    {
        originalTextPos = wordOutput.rectTransform.localPosition;
    }

    private void Start()
    {
        InitializeStatistics();
        SetCurrentWord();
    }

    private void Update()
    {
        CheckInput();
        UpdateStatsUI();
        CheckCapsLock();
        UpdateIdleDrain();
        stabilityMeter.ApplyWordTremble(wordOutput, originalTextPos);
    }

    #region Setup & Word Management

    private void InitializeStatistics()
    {
        startTime = Time.time;
        totalCharactersTyped = 0;
        totalMistakes = 0;
        wordsCompleted = 0;
    }

    private void SetCurrentWord()
    {
        bool useEasyWords = stabilityMeter.GetCurrentStability() < 50f;
        currentWord = wordBank.GetWord(useEasyWords);
        SetRemainingWord(currentWord);
    }

    private void SetRemainingWord(string newString)
    {
        remainingWord = newString;
        wordOutput.text = remainingWord;
    }

    #endregion

    #region Typing Logic

    private void CheckInput()
    {
        if (Input.anyKeyDown)
        {
            string keysPressed = Input.inputString;

            if (keysPressed.Length == 1)
            {
                EnterLetter(keysPressed);
            }
        }
    }

    private void EnterLetter(string typedLetter)
    {
        totalCharactersTyped++;
        lastTypeTime = Time.time;

        if (IsCorrectLetter(typedLetter))
        {
            HandleCorrectLetter(typedLetter);
        }
        else
        {
            HandleIncorrectLetter();
        }
    }

    private bool IsCorrectLetter(string letter) => remainingWord.IndexOf(letter) == 0;

    private void HandleCorrectLetter(string typedLetter)
    {
        correctCharactersTyped++;
        float stability = stabilityMeter.GetCurrentStability();

        // Double stability bonus under 10%
        if (stability < 10f)
        {
            stabilityMeter.IncreaseStability(2f);
            TriggerAbilityFeedback("Focus Boost ×2!", new Color(0.2f, 1f, 0.2f), doublePointsKeystrokeAudio);
        }
        else
        {
            stabilityMeter.IncreaseStability(1f);
            correctKeystrokeAudio.Play();
        }

        RemoveLetter();
        HandleFocusBurst(true);

        // Typing animation
        GameObject typedLetterAnim = Instantiate(typedLetterPrefab, typedLetterParentObject.transform);
        typedLetterAnim.GetComponent<TextMeshProUGUI>().text = typedLetter;
        Destroy(typedLetterAnim, 1f);

        // Completed word
        if (IsWordComplete())
        {
            wordsCompleted++;
            stabilityMeter.IncreaseStability(5f);
            SetCurrentWord();
            textCompleteAudio.Play();
        }
    }

    private void HandleIncorrectLetter()
    {
        totalMistakes++;
        stabilityMeter.DecreaseStability(5f);
        wrongKeystrokeAudio.Play();
        TriggerFlash();
        TriggerTextShake();
        HandleFocusBurst(false);
    }

    private void RemoveLetter()
    {
        string newString = remainingWord.Remove(0, 1);
        SetRemainingWord(newString);
    }

    private bool IsWordComplete()
    {
        return remainingWord.Length == 0;
    }

    #endregion

    #region Stats & UI

    private void UpdateStatsUI()
    {
        wpmOutput.text = $"WPM: {GetWordsPerMinute():F0}";
        mistakesOutput.text = $"Mistakes: {totalMistakes}";
        accuracyOutput.text = $"Accuracy: {GetAccuracy():F1}%";
        wordsCompletedOutput.text = $"Completed: {wordsCompleted}";
    }

    public float GetWordsPerMinute()
    {
        if (elapsedTime <= 0) return 0f;
        // WPM = (total characters / 5) / (time in minutes)
        return (correctCharactersTyped / (float)AverageWordLength) / (elapsedTime / 60f);
    }

    private float GetAccuracy()
    {
        if (totalCharactersTyped <= 0) return 100f;
        return (correctCharactersTyped / (float)totalCharactersTyped) * 100f;
    }

    private void CheckCapsLock()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (capsLockIcon != null)
        {
            capsLockIcon.enabled = IsCapsLockOn;
        }
#endif
    }

    #endregion

    #region Idle Drain

    // Decreases stability gradually if the player stays idle, with faster drain and shorter delay at high stability levels
    private void UpdateIdleDrain()
    {
        float stability = stabilityMeter.GetCurrentStability();
        float t = stability / 100f;

        float currentIdleDelay = Mathf.Lerp(baseIdleDelay, minIdleDelay, t);
        float currentDrainRate = Mathf.Lerp(baseDrainRate, minDrainRate, t);

        if (Time.time - lastTypeTime > currentIdleDelay)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= currentDrainRate)
            {
                stabilityMeter.DecreaseStability(2f);
                idleTimer = 0f;
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }

    #endregion

    #region Visual Feedback

    private void TriggerFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flashImage.color = new Color(1f, 0f, 0f, 0.5f);
        float t = 0f;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0f, t / flashDuration);
            flashImage.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }

        flashImage.color = new Color(1f, 0f, 0f, 0f);
    }

    private void TriggerTextShake()
    {
        if (textShakeCoroutine != null)
        {
            StopCoroutine(textShakeCoroutine);
        }

        textShakeCoroutine = StartCoroutine(ShakeText(wordOutput));
    }

    private IEnumerator ShakeText(TextMeshProUGUI text)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        float strength = 10f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Mathf.Sin(elapsed * 40f) * strength;
            text.rectTransform.localPosition = originalTextPos + new Vector3(offsetX, 0f, 0f);
            yield return null;
        }

        text.rectTransform.localPosition = originalTextPos;
    }

    #endregion

    #region Focus Burst System

    private void HandleFocusBurst(bool correct)
    {
        float currentStability = stabilityMeter.GetCurrentStability();
        isInLowStabilityZone = currentStability <= 20f;

        if (!isInLowStabilityZone)
        {
            focusBurstCombo = 0;
            return;
        }

        if (correct)
        {
            focusBurstCombo++;

            if (focusBurstCombo >= 3 && !focusBurstActive)
            {
                focusBurstActive = true;
                StartCoroutine(FocusBurstRoutine());
            }
        }
        else
        {
            focusBurstCombo = 0;
        }
    }

    private IEnumerator FocusBurstRoutine()
    {
        stabilityMeter.IncreaseStability(10f);
        TriggerAbilityFeedback("FOCUS BURST!", new Color(0.4f, 0.9f, 1f), focusBurstAudio);
        focusBurstCombo = 0;

        flashImage.color = new Color(0f, 1f, 0f, 0.4f);
        yield return new WaitForSeconds(0.1f);
        flashImage.color = new Color(0f, 1f, 0f, 0f);

        yield return new WaitForSeconds(0.2f);
        focusBurstActive = false;
    }

    private void TriggerAbilityFeedback(string message, Color flashColor, AudioSource sound)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(AbilityFlashRoutine(flashColor));
        sound.Play();

        if (abilityText != null)
        {
            abilityText.text = message;
            abilityText.alpha = 1f;
            StartCoroutine(FadeAbilityText());
        }
    }

    private IEnumerator AbilityFlashRoutine(Color flashColor)
    {
        flashImage.color = flashColor;
        float duration = 0.25f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0.6f, 0f, t / duration);
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }

        flashImage.color = new Color(0, 0, 0, 0);
    }

    private IEnumerator FadeAbilityText()
    {
        float duration = 1.2f;
        float t = 0f;
        Color startColor = abilityText.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            abilityText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        abilityText.text = string.Empty;
    }

    #endregion
}