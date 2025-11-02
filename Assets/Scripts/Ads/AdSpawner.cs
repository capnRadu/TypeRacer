using System.Collections;
using UnityEngine;

public class AdSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject adPrefab;
    public GameObject parent;

    // Spawn settings
    private int maxAds = 8;
    private float minSpawnDelay = 8f;
    private float maxSpawnDelay = 15f;

    private RectTransform parentRect;

    private void Start()
    {
        parentRect = parent.GetComponent<RectTransform>();
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            if (GameObject.FindGameObjectsWithTag("FakeAd").Length < maxAds)
            {
                SpawnAd();
            }
        }
    }

    private void SpawnAd()
    {
        GameObject newAd = Instantiate(adPrefab, parent.transform);

        RectTransform adRect = newAd.GetComponent<RectTransform>();

        float x = Random.Range(0f, parentRect.rect.width);
        float y = Random.Range(0f, parentRect.rect.height);
        adRect.anchoredPosition = new Vector2(
            x - parentRect.rect.width / 2,
            y - parentRect.rect.height / 2
        );
    }
}