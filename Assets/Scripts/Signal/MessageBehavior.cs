using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBehavior : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI messageText;

    private List<string> messageTexts = new List<string>()
    {
        "ZINKY ZOBU ",
        "BOGOS BINTED ",
        "GROBNAR VELTIK ",
        "SHLORP V’NAKA ",
        "BEEZOR KLAX ",
        "MUNGRA FLA’TOON ",
        "VOORB GLEEGA ",
        "SHNOKK RA-ZIBBA ",
        "KROONTA LAG’MAR ",
        "VORDAH BEEP-BLOP ",
        "ZOLTRONI YABBA ",
        "ZILDOX PRONK ",
        "SNORKA FLENT "
    };

    private void Start()
    {
        messageText.text = messageTexts[Random.Range(0, messageTexts.Count)] + messageTexts[Random.Range(0, messageTexts.Count)];
    }
}