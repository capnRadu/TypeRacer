using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StabilityMeter stabilityMeter;
    [SerializeField] private RectTransform rocketTransform;
    [SerializeField] private ParticleSystem thrusterParticles;

    [Header("Rocket Movement Settings")]
    [SerializeField] private float lowY = -240f;
    [SerializeField] private float midY = 12f;
    [SerializeField] private float highY = 240f;
    [SerializeField] private float moveLerpSpeed = 5f;

    [Header("Thruster Settings")]
    [SerializeField] private float lowRate = 4f;
    [SerializeField] private float highRate = 15f;
    [SerializeField] private float rateLerpSpeed = 5f;

    private ParticleSystem.EmissionModule emission;

    private void Awake()
    {
        rocketTransform = GetComponent<RectTransform>();
        emission = thrusterParticles.emission;
    }

    private void Update()
    {
        float stability = stabilityMeter.GetCurrentStability() / 100f;
        UpdateRocket(stability);
    }

    private void UpdateRocket(float stability)
    {
        float targetY = Mathf.Lerp(lowY, highY, stability);
        float targetRate = Mathf.Lerp(lowRate, highRate, stability);

        Vector2 pos = rocketTransform.anchoredPosition;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * moveLerpSpeed);
        rocketTransform.anchoredPosition = pos;

        var rate = emission.rateOverTime;
        rate.constant = Mathf.Lerp(rate.constant, targetRate, Time.deltaTime * rateLerpSpeed);
        emission.rateOverTime = rate;
    }
}