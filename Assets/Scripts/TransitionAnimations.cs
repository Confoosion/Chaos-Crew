using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TransitionAnimations : MonoBehaviour
{
    public static TransitionAnimations Singleton;
    void Awake()
    {
        if(Singleton == null)
            Singleton = this;
    }

    [SerializeField] private Image blackOutImage;
    private float fadeAnimSpeed = 1f;
    private Coroutine fadeRoutine;
    
    public void FadeIn()
    {
        if(fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
        fadeRoutine = StartCoroutine(Fade(true));
    }

    public void FadeOut()
    {
        if(fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
        fadeRoutine = StartCoroutine(Fade(false));
    }

    IEnumerator Fade(bool fadeIn)
    {
        float duration = 0f;
        Color startColor = blackOutImage.color;
        Color targetColor = new Color(0f, 0f, 0f, fadeIn ? 1f : 0f);

        while(duration < 1f)
        {
            duration += Time.deltaTime * fadeAnimSpeed;
            blackOutImage.color = Color.Lerp(startColor, targetColor, duration);
            yield return null;
        }

        blackOutImage.color = targetColor;
        fadeRoutine = null;
    }
}
