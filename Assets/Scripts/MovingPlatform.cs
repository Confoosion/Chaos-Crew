using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float waitTime = 0.5f;

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool canPlatformMove = true;

    private int currentIndex = 0;
    private float waitTimer = 0f;
    private bool waiting = false;
    private Rigidbody2D rb;

    private float movingProgress = 0f;
    private Vector2 movingStart;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if(waypoints.Length < 2)
        {
            Debug.LogWarning("Not enough waypoints in a Moving Platform!");
            canPlatformMove = false;
        }
        else
        {
            transform.position = waypoints[currentIndex].position;
            currentIndex++;
            movingStart = rb.position;
        }
    }

    void FixedUpdate()
    {
        if(canPlatformMove)
        {
            if(waiting)
            {
                waitTimer -= Time.fixedDeltaTime;
                if(waitTimer <= 0f)
                {
                    waiting = false;
                    movingProgress = 0f;
                    movingStart = rb.position;
                }
                return;
            }

            Transform target = waypoints[currentIndex];

            float moveLength = Vector2.Distance(movingStart, target.position);
            float step = (speed * Time.fixedDeltaTime) / Mathf.Max(moveLength, 0.0001f);
            movingProgress = Mathf.Clamp01(movingProgress + step);

            float easedT = EaseInOut(movingProgress);

            Vector2 newPos = Vector2.Lerp(movingStart, target.position, easedT);
            rb.MovePosition(newPos);

            if(movingProgress >= 1f)
            {
                currentIndex = (currentIndex + 1) % waypoints.Length;
                waiting = true;
                waitTimer = waitTime;
            }
        }
    }

    private float EaseInOut(float t)
    {
        return(t * t * (3f- 2f * t));
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
