using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextMapPortal : MonoBehaviour
{
    private float portalAnimationSpeed = 1.5f;
    private Vector3 portalScale = new Vector3(0.75f, 1f, 1f);
    private bool isOpened = false;

    void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.CompareTag("Player") && isOpened)
        {
            GameManager.Singleton.NextLevel();   
        }
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
        if(open && transform.localScale == Vector3.zero)
        {
            while(transform.localScale.z < portalScale.z)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, portalScale, portalAnimationSpeed * Time.deltaTime);

                yield return new WaitForSeconds(Time.deltaTime * portalAnimationSpeed);
            }

            transform.localScale = portalScale;
            isOpened = true;
        }
        else
        {
            while(transform.localScale.z > 0f)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, portalAnimationSpeed * Time.deltaTime);

                yield return new WaitForSeconds(Time.deltaTime * portalAnimationSpeed);
            }

            transform.localScale = Vector3.zero;
        }
    }
}
