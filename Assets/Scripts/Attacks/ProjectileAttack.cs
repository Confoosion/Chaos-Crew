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
    private float raycastDistance = 0.5f;
    [SerializeField] private Transform raycastTransform;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Ground", "Enemy");
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

        RaycastHit2D hitCollision = Physics2D.Raycast(raycastTransform.position, new Vector2(facing, 0f), raycastDistance, layerMask);
        Debug.DrawRay(raycastTransform.position, new Vector2(facing, 0f) * raycastDistance, Color.red);

        if(hitCollision.collider != null)
        {
            if(hitCollision.distance <= 0.2f)
            {
                CollisionActive(hitCollision.collider);
            }
        }
    }

    private void CollisionActive(Collider2D collider)
    {
        if(collider.CompareTag("Enemy"))
        {
            collider.gameObject.GetComponent<EnemyController>().enemyTakeDamage(damage);

            if(pierceAmount > 0 && pierces < pierceAmount)
                pierces++;
            else if(canBounceOffEnemies && bounceAmount > 0)
                DoBounce();
            else
                ProjectileDespawn();
        }
        else
        {
            if(canBounceOffWalls && bounceAmount > 0)
                DoBounce();
            else if(!canRollOnGround)
                ProjectileDespawn();
        }

        SoundManager.Singleton.PlayAttackAudio(collision_SFX);
    }

    // void OnTriggerEnter2D(Collider2D collider)
    // {
    //     if (collider.CompareTag("Enemy"))
    //     {
    //         collider.gameObject.GetComponent<EnemyController>().enemyTakeDamage(damage);
    //         if (pierceAmount > 0 && pierces < pierceAmount)
    //         {
    //             pierces++;
    //         }
    //         else if (canBounceOffEnemies && bounceAmount > 0)
    //         {
    //             DoBounce();
    //         }
    //         else
    //         {
    //             ProjectileDespawn();
    //         }
    //     }

    //     if (collider.CompareTag("Terrain"))
    //     {
    //         if (canBounceOffWalls && bounceAmount > 0 && collider.gameObject.layer == layerMask)
    //         {
    //             DoBounce();
    //         }
    //         else if(!canRollOnGround)
    //         {
    //             ProjectileDespawn();
    //         }
    //     }

    //     SoundManager.Singleton.PlayAttackAudio(collision_SFX);
    // }

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
}
