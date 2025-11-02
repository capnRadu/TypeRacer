using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    private StabilityMeter stabilityMeter;
    private TextMeshProUGUI distanceText;
    private TextMeshProUGUI messagesText;

    private float currentDistanceTraveled = 0f;
    private int currentMessagesDecoded = 0;

    private float highestDistanceTraveled = 0f;
    private int highestMessagesDecoded = 0;

    private const float baseSpeed = 1500f; // km per second at full stability

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindTextReferences();

        if (scene.name == "MainMenu")
        {
            stabilityMeter = null;
            
            if (currentDistanceTraveled > highestDistanceTraveled)
            {
                highestDistanceTraveled = currentDistanceTraveled;
            }

            if (currentMessagesDecoded > highestMessagesDecoded)
            {
                highestMessagesDecoded = currentMessagesDecoded;
            }

            UpdateMenuTexts();
        }
        else if (scene.name == "SampleScene")
        {
            stabilityMeter = FindFirstObjectByType<StabilityMeter>();
            currentDistanceTraveled = 0f;
            currentMessagesDecoded = 0;
        }
    }

    private void Update()
    {
        if (stabilityMeter != null)
        {
            float stability = stabilityMeter.GetCurrentStability();
            float speed = Mathf.Lerp(100f, baseSpeed, Mathf.Pow(stability / 100f, 2));
            currentDistanceTraveled += speed * Time.deltaTime;
            distanceText.text = $"Travelled Distance: {FormatDistance(currentDistanceTraveled)}";
        }
    }

    private void FindTextReferences()
    {
        distanceText = GameObject.FindWithTag("TraveledDistanceText").GetComponent<TextMeshProUGUI>();
        messagesText = GameObject.FindWithTag("MessagesDecodedText").GetComponent<TextMeshProUGUI>();
    }

    public void AddDecodedMessage()
    {
        currentMessagesDecoded++;
        messagesText.text = $"Messages Decoded: {currentMessagesDecoded}";
    }

    private void UpdateMenuTexts()
    {
        distanceText.text = $"Highest Travelled Distance: {FormatDistance(highestDistanceTraveled)}";
        messagesText.text = $"Most Messages Decoded: {highestMessagesDecoded}";
    }

    private string FormatDistance(float distance)
    {
        if (distance >= 1_000_000f)
            return $"{distance / 1_000_000f:F2} Mm";
        else
            return $"{distance:N0} km";
    }
}