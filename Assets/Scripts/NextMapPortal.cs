using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextMapPortal : MonoBehaviour
{
    private float portalAnimationDuration = 1.5f;
    private Vector3 portalScale = new Vector3(0.75f, 1f, 1f);
    private bool isOpened = false;
    private Coroutine enterPortalRoutine;

    void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.CompareTag("Player") && isOpened && enterPortalRoutine == null)
        {
            collider.gameObject.SetActive(false);
            enterPortalRoutine = StartCoroutine(EnterPortalAnimation());
        }
    }

    IEnumerator EnterPortalAnimation()
    {
        LeanTween.scale(gameObject, Vector3.zero, portalAnimationDuration).setEase(LeanTweenType.easeInBack);
        yield return new WaitForSeconds(portalAnimationDuration);

        TransitionAnimations.Singleton.FadeIn();
        yield return new WaitForSeconds(1.2f);

        GameManager.Singleton.NextLevel();
    }

    public void ShowPortal()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(ShowPortal(true));
    }

    public void HidePortal()
    {
        transform.localScale = portalScale;
        StartCoroutine(ShowPortal(false));
    }

    IEnumerator ShowPortal(bool open)
    {
        LeanTween.scale(gameObject, open? portalScale : Vector3.zero, portalAnimationDuration);
        yield return new WaitForSeconds(portalAnimationDuration);
        isOpened = open;
    }
}
