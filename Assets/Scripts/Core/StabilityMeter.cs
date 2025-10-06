using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Handles player stability — affects color grading, post-processing, UI color transitions, word trembling, background audio
/// </summary>
public class StabilityMeter : MonoBehaviour
{
    private float stability = 100f;
    public float GetCurrentStability() => stability;

    [Header("UI Elements")]
    public TextMeshProUGUI stabilityText;
    public Image backgroundImage;

    [Header("Post-Processing")]
    public Volume postProcessingVolume;
    private Vignette vignette;
    private FilmGrain filmGrain;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;

    [Header("Audio")]
    public AudioSource lofiMusicAudio;
    public AudioSource heartbeatAudio;

    // Colors for different stability levels
    private Color lowStabilityColor;
    private Color mediumStabilityColor;
    private Color highStabilityColor;
    private float colorChangeSpeed = 3f;

    // PostFX animation state
    private float displayedT = 0f;
    private float timeOffset;

    // Audio volume ranges
    private const float LofiMaxVolume = 0.34f;
    private const float LofiMinVolume = 0.022f;
    private const float HeartbeatMinVolume = 0f;
    private const float HeartbeatMaxVolume = 1f;

    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#330000", out lowStabilityColor);
        ColorUtility.TryParseHtmlString("#333333", out mediumStabilityColor);
        ColorUtility.TryParseHtmlString("#96BCDE", out highStabilityColor);

        if (postProcessingVolume.profile.TryGet(out vignette) &&
            postProcessingVolume.profile.TryGet(out filmGrain) &&
            postProcessingVolume.profile.TryGet(out chromaticAberration) &&
            postProcessingVolume.profile.TryGet(out lensDistortion))
        {
            timeOffset = Random.Range(0f, 100f);
        }
        else
        {
            Debug.LogWarning("Post processing effects not found in volume");
        }
    }

    private void Update()
    {
        UpdateBackgroundColor();
        UpdateStabilityText();
        UpdatePostFX();
        UpdateAudio();
    }

    // Updates background color based on current stability
    private void UpdateBackgroundColor()
    {
        Color targetColor = GetTargetColor();
        backgroundImage.color = Color.Lerp(backgroundImage.color, targetColor, Time.deltaTime * colorChangeSpeed);
    }

    // Determines the interpolated background color
    private Color GetTargetColor()
    {
        float t = stability / 100f;

        if (t > 0.5f)
        {
            return Color.Lerp(mediumStabilityColor, highStabilityColor, (t - 0.5f) / 0.5f);
        }
        else
        {
            return Color.Lerp(lowStabilityColor, mediumStabilityColor, t / 0.5f);
        }
    }

    private void UpdateStabilityText()
    {
        stabilityText.text = $"Stability: {stability:F0}%";
    }

    // Drives all post-processing effects dynamically
    private void UpdatePostFX()
    {
        float targetT = 1f - (stability / 100f);
        displayedT = Mathf.Lerp(displayedT, targetT, Time.deltaTime * 2f);

        vignette.intensity.value = Mathf.Lerp(0f, 0.376f, displayedT);
        filmGrain.intensity.value = Mathf.Lerp(0f, 1f, displayedT);
        chromaticAberration.intensity.value = Mathf.Lerp(0f, 0.628f, displayedT);

        lensDistortion.intensity.value = Mathf.Lerp(0f, -0.66f, displayedT);

        float pulse = (Mathf.Sin((Time.time + timeOffset) * 2f) * 0.5f) + 0.5f;
        lensDistortion.xMultiplier.value = pulse;
        lensDistortion.yMultiplier.value = pulse;
    }

    // Interpolates between lofi and heartbeat volumes
    private void UpdateAudio()
    {
        float t = 1f - (stability / 100f);

        if (lofiMusicAudio)
        {
            lofiMusicAudio.volume = Mathf.Lerp(LofiMaxVolume, LofiMinVolume, t);
        }

        if (heartbeatAudio)
        {
            heartbeatAudio.volume = Mathf.Lerp(HeartbeatMinVolume, HeartbeatMaxVolume, t);
        }
    }

    public void IncreaseStability(float amount)
    {
        stability += amount;

        if (stability > 100f)
        {
            stability = 100f;
        }

        Debug.Log($"Stability increased to: {stability}");
    }

    public void DecreaseStability(float amount)
    {
        stability -= amount;

        if (stability < 0f)
        {
            stability = 0f;
        }

        Debug.Log($"Stability decreased to: {stability}");
    }

    // Applies trembling to text based on stability level
    public void ApplyWordTremble(TextMeshProUGUI text, Vector3 originalPosition)
    {
        if (text == null) return;

        float t = 1f - (stability / 100f);

        float shakeStrength = Mathf.Lerp(0f, 10f, Mathf.Pow(t, 1.2f));

        float noiseX = (Mathf.PerlinNoise(Time.time * 8f, 0f) - 0.5f) * 2f;
        float noiseY = (Mathf.PerlinNoise(0f, Time.time * 8f) - 0.5f) * 2f;

        Vector3 shakeOffset = new Vector3(noiseX, noiseY, 0f) * shakeStrength;
        text.rectTransform.localPosition = originalPosition + shakeOffset;
    }
}