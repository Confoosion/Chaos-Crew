using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextMapPortal : MonoBehaviour
{
    private float portalAnimationDuration = 1.5f;
    private Vector3 portalScale = new Vector3(0.75f, 1f, 1f);
    private bool isOpened = false;
    // private float enterPortalAnimDuration = 1.5f;
    private Coroutine enterPortalRoutine;

    void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.CompareTag("Player") && isOpened && enterPortalRoutine == null)
        {
            collider.gameObject.SetActive(false);
            enterPortalRoutine = StartCoroutine(EnterPortalAnimation());

            // GameManager.Singleton.NextLevel();   
        }
    }

    IEnumerator EnterPortalAnimation()
    {
        LeanTween.scale(gameObject, Vector3.zero, portalAnimationDuration).setEase(LeanTweenType.easeInBack);
        yield return new WaitForSeconds(portalAnimationDuration);

        TransitionAnimations.Singleton.FadeIn();
        yield return new WaitForSeconds(1.2f);

        GameManager.Singleton.NextLevel();
        // Destroy(this.gameObject);
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


        // if(open && transform.localScale == Vector3.zero)
        // {
        //     while(transform.localScale.z < portalScale.z)
        //     {
        //         transform.localScale = Vector3.MoveTowards(transform.localScale, portalScale, portalAnimationSpeed * Time.deltaTime);

        //         yield return new WaitForSeconds(Time.deltaTime * portalAnimationSpeed);
        //     }

        //     transform.localScale = portalScale;
        //     isOpened = true;
        // }
        // else
        // {
        //     while(transform.localScale.z > 0f)
        //     {
        //         transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, portalAnimationSpeed * Time.deltaTime);

        //         yield return new WaitForSeconds(Time.deltaTime * portalAnimationSpeed);
        //     }

        //     transform.localScale = Vector3.zero;
        // }
    }
}
