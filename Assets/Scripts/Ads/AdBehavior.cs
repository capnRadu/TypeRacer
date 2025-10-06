using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdBehavior : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("UI References")]
    public Image adImage;
    public TextMeshProUGUI adText;

    // Flash settings
    private Color colorA = Color.white;
    private Color colorB = new Color(1f, 1f, 0.2f);
    private float flashSpeed = 3f;

    private List<string> adTexts = new List<string>()
    {
        "You Won a Free Prize!",
        "Install Now to Boost Your Focus!",
        "New Typing Assistant Available!",
        "Claim Your Bonus Stability Points!",
        "You Won’t Believe This One Trick!",
        "Stream Lofi+ Premium Today!",
        "Double Your Score Instantly!",
        "Optimize Your Typing Speed Now!"
    };

    // Dragging the window
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 dragOffset;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        adText.text = adTexts[Random.Range(0, adTexts.Count)];
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * flashSpeed;
            adImage.color = Color.Lerp(colorA, colorB, (Mathf.Sin(t) + 1f) / 2f);
            yield return null;
        }
    }

    public void CloseAd()
    {
        Destroy(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out dragOffset
        );

        dragOffset = rectTransform.anchoredPosition - dragOffset;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint + dragOffset;
        }
    }
}