using UnityEngine;
using UnityEngine.UI;

public class SignalMatchingController : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private Sinewave targetWave;
    [SerializeField] private Sinewave playerWave;
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private Slider amplitudeSlider;

    [Header("Other References")]
    [SerializeField] private AlienSignalManager alienSignalManager;
    [SerializeField] private GameObject decodedMessagePrefab;
    [SerializeField] private GameObject messageParent;

    [Header("Target Wave Randomization")]
    [SerializeField] private Vector2 randomFrequencyRange = new Vector2(0.5f, 3f);
    [SerializeField] private Vector2 randomAmplitudeRange = new Vector2(0.5f, 2f);

    [Header("Matching Settings")]
    [SerializeField] private float matchMargin = 0.05f; // allowed margin of error

    private bool hasSubmitted = false;

    private void OnEnable()
    {
        targetWave.frequency = Random.Range(randomFrequencyRange.x, randomFrequencyRange.y);
        targetWave.amplitude = Random.Range(randomAmplitudeRange.x, randomAmplitudeRange.y);

        frequencySlider.value = playerWave.frequency;
        amplitudeSlider.value = playerWave.amplitude;

        hasSubmitted = false;
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
        if (hasSubmitted) return;

        bool frequencyMatch = Mathf.Abs(playerWave.frequency - targetWave.frequency) <= matchMargin;
        bool amplitudeMatch = Mathf.Abs(playerWave.amplitude - targetWave.amplitude) <= matchMargin;

        if (frequencyMatch && amplitudeMatch)
        {
            hasSubmitted = true;

            GameObject decodedMessage = Instantiate(decodedMessagePrefab, messageParent.transform);
            Destroy(decodedMessage, 3f);
            alienSignalManager.Invoke(nameof(alienSignalManager.CompleteSignal), 3f);
        }
    }

    private void OnDestroy()
    {
        frequencySlider.onValueChanged.RemoveListener(OnFrequencyChanged);
        amplitudeSlider.onValueChanged.RemoveListener(OnAmplitudeChanged);
    }
}