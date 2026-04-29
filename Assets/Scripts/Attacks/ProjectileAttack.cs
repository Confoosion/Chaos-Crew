using UnityEngine;

// DETECTS HITS!!! Use for range/projectile attacks that move forward in a constant velocity
public class ProjectileAttack : MonoBehaviour
{
    private float damage;
    private float speed;
    private float facing;

    private Rigidbody2D rb;

    [SerializeField] private bool canBounceOffWalls;
    [SerializeField] private bool canBounceOffEnemies;
    [SerializeField] private bool canRollOnGround;
    [SerializeField] private bool physicsObject;
    [SerializeField] private int bounceAmount;
    [SerializeField] private AudioClip collision_SFX;
    private int pierceAmount;
    private int pierces;
    public GameObject impactObject;
    private float impactDamage;
    private float impactDuration;
    private LayerMask layerMask;
    [SerializeField] private float raycastDistance = 0.5f;
    [SerializeField] private float boxHeight = 0.5f;
    [SerializeField] private Transform raycastTransform;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Ground", "MovingGround");
    }

    public void SetData(float dmg, float vel, float direction, int piercing = 0, int bounces = 0)
    {
        damage = dmg;
        speed = vel;
        facing = direction;
        pierceAmount = piercing;
        pierces = 0;
        bounceAmount = bounces;

        rb = GetComponent<Rigidbody2D>();
    }

    public void SetImpactData(float dmg, float dur)
    {
        impactDamage = dmg;
        impactDuration = dur;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(speed * facing, (physicsObject) ? rb.linearVelocity.y : 0f);

        // Uncomment this line below if you have the DrawBoxCastGizmo also uncommented
        // DrawBoxCastGizmo(raycastTransform.position, new Vector2(0.1f, boxHeight), new Vector2(facing, 0f), raycastDistance, Color.red);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            collider.gameObject.GetComponent<EnemyController>().enemyTakeDamage(damage);
            if (pierceAmount > 0 && pierces < pierceAmount)
            {
                pierces++;
            }
            else if (canBounceOffEnemies && bounceAmount > 0)
            {
                DoBounce();
            }
            else
            {
                ProjectileDespawn();
            }
        }

        SoundManager.Singleton.PlayAttackAudio(collision_SFX);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if(!collider.CompareTag("Enemy"))
        {
            RaycastHit2D hitCollision = Physics2D.BoxCast(
                raycastTransform.position,
                new Vector2(0.1f, boxHeight),
                0f,
                new Vector2(facing, 0f),
                raycastDistance,
                layerMask
            );

            if(hitCollision.collider != null)
            {
                if(canBounceOffWalls && bounceAmount > 0)
                {
                    DoBounce();
                }
                else if(!canRollOnGround)
                {
                    ProjectileDespawn();
                }
            }
        }
    }

    void DoBounce()
    {
        bounceAmount--;
        facing *= -1f;
        pierces = 0;
    }

    private void ProjectileDespawn()
    {
        SpawnImpact();
        Destroy(this.gameObject);
    }

    private void SpawnImpact()
    {
        if (impactObject != null)
        {
            GameObject impact = Instantiate(impactObject, transform.position, Quaternion.identity);
            if (impact.GetComponent<MeleeAttack>())
            {
                impact.GetComponent<MeleeAttack>().SetData(impactDamage, impactDuration);
            }
        }
    }

    public void ManuallyDetonate()
    {
        ProjectileDespawn();
    }

    // UNCOMMENT THIS CODE if you wanna see the raycast box for collisions (wall detection) 

    // void DrawBoxCastGizmo(Vector2 origin, Vector2 size, Vector2 direction, float distance, Color color)
    // {
    //     Vector2 end = origin + direction * distance;
    //     // Draw the start box
    //     Debug.DrawLine(origin + new Vector2(-size.x / 2, -size.y / 2), origin + new Vector2(size.x / 2, -size.y / 2), color);
    //     Debug.DrawLine(origin + new Vector2(-size.x / 2,  size.y / 2), origin + new Vector2(size.x / 2,  size.y / 2), color);
    //     Debug.DrawLine(origin + new Vector2(-size.x / 2, -size.y / 2), origin + new Vector2(-size.x / 2, size.y / 2), color);
    //     Debug.DrawLine(origin + new Vector2( size.x / 2, -size.y / 2), origin + new Vector2( size.x / 2, size.y / 2), color);
    //     // Draw the end box
    //     Debug.DrawLine(end + new Vector2(-size.x / 2, -size.y / 2), end + new Vector2(size.x / 2, -size.y / 2), color);
    //     Debug.DrawLine(end + new Vector2(-size.x / 2,  size.y / 2), end + new Vector2(size.x / 2,  size.y / 2), color);
    //     Debug.DrawLine(end + new Vector2(-size.x / 2, -size.y / 2), end + new Vector2(-size.x / 2, size.y / 2), color);
    //     Debug.DrawLine(end + new Vector2( size.x / 2, -size.y / 2), end + new Vector2( size.x / 2, size.y / 2), color);
    //     // Connect start and end
    //     Debug.DrawLine(origin + new Vector2(-size.x / 2, -size.y / 2), end + new Vector2(-size.x / 2, -size.y / 2), color);
    //     Debug.DrawLine(origin + new Vector2( size.x / 2, -size.y / 2), end + new Vector2( size.x / 2, -size.y / 2), color);
    //     Debug.DrawLine(origin + new Vector2(-size.x / 2,  size.y / 2), end + new Vector2(-size.x / 2,  size.y / 2), color);
    //     Debug.DrawLine(origin + new Vector2( size.x / 2,  size.y / 2), end + new Vector2( size.x / 2,  size.y / 2), color);
    // }
}
