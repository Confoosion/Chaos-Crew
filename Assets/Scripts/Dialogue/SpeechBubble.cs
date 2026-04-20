using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SpeechBubble : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image bubbleBackground;
    [SerializeField] private TextMeshProUGUI bubbleText;
    [SerializeField] private RectTransform bubbleContainer;

    [Header("Positioning")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 1.5f);

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private float punctuationPause = 0.12f;

    [Header("Sizing")]
    [SerializeField] private float maxBubbleWidth = 4f;
    [SerializeField] private float paddingX = 0.3f;
    [SerializeField] private float paddingY = 0.2f;
    [SerializeField] private float minHeight = 0.5f;

    [Header("Animation")]
    [SerializeField] private float scaleAnimSpeed = 8f;
    [SerializeField] private float overshootScale = 1.1f;

}
