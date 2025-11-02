using System.Collections;
using UnityEngine;

public class AlienSignalManager : MonoBehaviour
{
    [SerializeField] private GameObject signalUIPanel;
    [SerializeField] private Typer typer;
    [SerializeField] private StabilityMeter stabilityMeter;

    [SerializeField] private float minSignalDelay = 2f;
    [SerializeField] private float maxSignalDelay = 3f;

    private bool signalActive = false;

    private void Start()
    {
        StartCoroutine(SignalRoutine());
    }

    private IEnumerator SignalRoutine()
    {
        while (true)
        {
            if (signalActive)
            {
                yield return null;
                continue;
            }

            float waitTime = Random.Range(minSignalDelay, maxSignalDelay);
            yield return new WaitForSeconds(waitTime);
            TriggerSignal();
        }
    }

    private void TriggerSignal()
    {
        if (signalActive) return;

        signalActive = true;
        typer.enabled = false;
        signalUIPanel.SetActive(true);
    }

    public void CompleteSignal()
    {
        signalActive = false;
        signalUIPanel.SetActive(false);
        typer.enabled = true;
        stabilityMeter.IncreaseStability(10f);
        HighScoreManager.Instance.AddDecodedMessage();
    }
}