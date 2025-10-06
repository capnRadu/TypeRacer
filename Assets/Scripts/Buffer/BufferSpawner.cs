using UnityEngine;
using System.Collections;

public class BufferSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject bufferPrefab;
    public GameObject parent;
    public Typer typer;

    // Timing
    private float minDelay = 20f;
    private float maxDelay = 30f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            SpawnBuffer();
        }
    }

    public void SpawnBuffer()
    {
        GameObject instance = Instantiate(bufferPrefab, parent.transform);
        var buffer = instance.GetComponent<BufferBehavior>();
        buffer.Setup(typer);
    }
}