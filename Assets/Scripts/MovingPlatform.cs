using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float moveTime = 1f;
    [SerializeField] private float waitTime = 0.5f;

    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private bool canPlatformMove = true;

    private int currentIndex = 0;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if(waypoints.Count < 2)
        {
            Debug.LogWarning("Not enough waypoints in a Moving Platform!");
            return;   
        }

        StartCoroutine(ContinuousMovement());
    }

    IEnumerator ContinuousMovement()
    {
        int waypointCount = waypoints.Count;

        gameObject.transform.position = waypoints[currentIndex].position;

        while(canPlatformMove)
        {
            currentIndex = (currentIndex + 1) % waypointCount;
            LeanTween.move(gameObject, waypoints[currentIndex].position, moveTime).setEase(LeanTweenType.easeInOutSine);

            yield return new WaitForSeconds(moveTime + waitTime);  
        }

        yield return null;  
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == null) return;
        if (!gameObject.activeInHierarchy) return;
        if (!collision.gameObject.activeInHierarchy) return;

        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(null);
    }
}
