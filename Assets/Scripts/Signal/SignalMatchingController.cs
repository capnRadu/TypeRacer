using UnityEngine;
using UnityEngine.UI;

public class SignalMatchingController : MonoBehaviour
{
    [Header("References")]
    public Sinewave targetWave;
    public Sinewave playerWave;
    public Slider frequencySlider;
    public Slider amplitudeSlider;

    [Header("Target Wave Randomization")]
    public Vector2 randomFrequencyRange = new Vector2(0.5f, 3f);
    public Vector2 randomAmplitudeRange = new Vector2(0.5f, 2f);

    [Header("Matching Settings")]
    public float matchMargin = 0.05f; // allowed margin of error

    private void OnEnable()
    {
        targetWave.frequency = Random.Range(randomFrequencyRange.x, randomFrequencyRange.y);
        targetWave.amplitude = Random.Range(randomAmplitudeRange.x, randomAmplitudeRange.y);

        frequencySlider.value = playerWave.frequency;
        amplitudeSlider.value = playerWave.amplitude;
    }

    private void Start()
    {
        frequencySlider.onValueChanged.AddListener(OnFrequencyChanged);
        amplitudeSlider.onValueChanged.AddListener(OnAmplitudeChanged);
    }

    private void OnFrequencyChanged(float value)
    {
        playerWave.frequency = value;
    }

    private void OnAmplitudeChanged(float value)
    {
        playerWave.amplitude = value;
    }

    public void Submit()
    {
        bool frequencyMatch = Mathf.Abs(playerWave.frequency - targetWave.frequency) <= matchMargin;
        bool amplitudeMatch = Mathf.Abs(playerWave.amplitude - targetWave.amplitude) <= matchMargin;

        if (frequencyMatch && amplitudeMatch)
        {
            Debug.Log("Match success! Alien message received!");
        }
        else
        {
            Debug.Log("Match failed. Try again.");
        }
    }

    private void OnDestroy()
    {
        frequencySlider.onValueChanged.RemoveListener(OnFrequencyChanged);
        amplitudeSlider.onValueChanged.RemoveListener(OnAmplitudeChanged);
    }
}