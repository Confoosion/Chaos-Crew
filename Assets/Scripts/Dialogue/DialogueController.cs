using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image lineSeparator;
    [SerializeField] private Image characterPortrait;

    [Header("Timings")]
    [SerializeField] private float typeSpeed = 0.03f;
    [SerializeField] private float holdAfterFinish = 2f;
    [SerializeField] private float fadeDuration = 0.35f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        // Start hidden
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        nameText.text = "";
        dialogueText.text = "";
    }

    public void PlayLine(string speakerName, Sprite portrait, string line)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(PlayLineRoutine(speakerName, portrait, line));
    }

    public void FadeAwayNow(bool clearText = true)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(FadeOutRoutine(clearText));
    }

    private IEnumerator PlayLineRoutine(string speakerName, Sprite portrait, string line)
    {
        nameText.text = speakerName;
        dialogueText.text = "";
        characterPortrait.sprite = portrait;
        SetLineSeparatorWidthToName();

        // Fade in
        yield return FadeRoutine(1f);

        // Type
        yield return TypeTextRoutine(line);

        // Hold
        yield return new WaitForSeconds(holdAfterFinish);

        // Fade out
        yield return FadeOutRoutine(clearText: true);
        currentRoutine = null;
    }

    private IEnumerator TypeTextRoutine(string fullText)
    {
        dialogueText.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alph = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, alph);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeOutRoutine(bool clearText)
    {
        yield return FadeRoutine(0f);

        if (clearText)
        {
            nameText.text = "";
            dialogueText.text = "";
        }

        currentRoutine = null;
    }

    // Set Line Separator Width to match the character name like an underline
    private void SetLineSeparatorWidthToName()
    {
        nameText.ForceMeshUpdate();

        float padding = 5f;
        float w = nameText.preferredWidth + padding;

        RectTransform nameRT = nameText.rectTransform;
        RectTransform lineRT = lineSeparator.rectTransform;

        // Set width
        lineRT.sizeDelta = new Vector2(w, lineRT.sizeDelta.y);

        // Align left edge under the name's left edge
        lineRT.anchoredPosition = new Vector2(nameRT.anchoredPosition.x, lineRT.anchoredPosition.y);
    }
}
