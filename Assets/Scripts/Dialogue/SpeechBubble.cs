using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to a Canvas that is a child of the speaking character.
/// </summary>
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

    private Coroutine typingCoroutine;
    private bool isShowing;
    private string fullText;

    public event Action OnMessageComplete;
    public event Action OnBubbleHidden;

    private void Awake()
    {
        // Start hidden
        bubbleContainer.localScale = Vector3.zero;
        bubbleText.text = string.Empty;
        SetVisible(false);
    }

    private void LateUpdate()
    {
        transform.localPosition = offset;
        transform.rotation = Quaternion.identity;
    }

    public void ShowMessage(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        fullText = message;
        SetVisible(true);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    public void Hide()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        StartCoroutine(AnimateHide());
    }

    //
    // Typing
    //

    private IEnumerator TypeText(string message)
    {
        yield return StartCoroutine(AnimateShow());

        bubbleText.text = message;
        bubbleText.maxVisibleCharacters = 0;
        bubbleText.ForceMeshUpdate();

        for (int i = 0; i < message.Length; i++)
        {
            bubbleText.maxVisibleCharacters = i + 1;
            RefreshBubbleSize();

            // Delay based on punctuation
            char c = message[i];
            if (c == '.' || c == '!' || c == '?')
                yield return new WaitForSeconds(punctuationPause);
            else if (c == ',')
                yield return new WaitForSeconds(punctuationPause * 0.5f);
            else
                yield return new WaitForSeconds(typingSpeed);
        }

        OnMessageComplete?.Invoke();
    }

    //
    // Sizing
    //

    private void RefreshBubbleSize()
    {
        bubbleText.ForceMeshUpdate();

        float textWidth = bubbleText.preferredWidth;
        float textHeight = bubbleText.preferredHeight;

        // clamp width, not height
        float width = Mathf.Min(textWidth + paddingX * 2f, maxBubbleWidth);
        float height = Mathf.Max(textHeight + paddingY * 2f, minHeight);

        if (textWidth + paddingX * 2f > maxBubbleWidth)
        {
            bubbleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxBubbleWidth - paddingX * 2f);
            bubbleText.ForceMeshUpdate();
            height = Mathf.Max(bubbleText.preferredHeight + paddingY * 2f, minHeight);
        }

        bubbleContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        bubbleContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    private IEnumerator AnimateShow()
    {
        isShowing = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleAnimSpeed;
            float scale = Mathf.LerpUnclamped(0f, 1f, EaseOutBack(t));
            bubbleContainer.localScale = Vector3.one * scale;
            yield return null;
        }

        bubbleContainer.localScale = Vector3.one;
    }

    private IEnumerator AnimateHide()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleAnimSpeed;
            float scale = Mathf.Lerp(1f, 0f, t * t);
            bubbleContainer.localScale = Vector3.one * scale;
            yield return null;
        }

        bubbleContainer.localScale = Vector3.zero;
        bubbleText.text = string.Empty;
        isShowing = false;
        SetVisible(false);
        OnBubbleHidden?.Invoke();
    }

    private void SetVisible(bool visible)
    {
        bubbleBackground.enabled = visible;
        bubbleText.enabled = visible;
    }

    // Ease out back for pop in
    private float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
