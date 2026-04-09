using UnityEngine;

public class BasicEnemyMovement : EnemyController
{
    [SerializeField] private bool canMove = true;
    [SerializeField] private LayerMask wallLayer;
    private int direction = 1;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Sets speed in EnemyController since we're inheriting the class
        SetSpeed();
        SetHealth();
    }

    void FixedUpdate()
    {
        if(canMove)
        {
            rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((wallLayer.value & 1 << collision.transform.gameObject.layer) != 0)
        {
            direction *= -1;
            UpdateSpriteDirection();
        }
        else if(collision.transform.CompareTag("Player") && !PlayerControl.Singleton.IsStandingOnEnemy())
        {
            KillPlayer(collision.gameObject);
        }
    }

    public void SetMoveDirection(int dir)
    {
        direction = dir;
        UpdateSpriteDirection();
    }

    private void UpdateSpriteDirection()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, transform.localScale.y, transform.localScale.z);
    }
}


