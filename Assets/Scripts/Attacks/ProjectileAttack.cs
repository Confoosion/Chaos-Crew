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
    [SerializeField] private AudioClip collision_SFX;
    private int bounceAmount = 0;
    private int pierceAmount = 0;
    private int pierces;
    public GameObject impactObject;
    private float impactDamage;
    private float impactDuration;
    // private LayerMask layerMask;
    // [SerializeField] private float raycastDistance = 0.5f;
    // [SerializeField] private float boxHeight = 0.5f;
    // [SerializeField] private Transform raycastTransform;
    [SerializeField] private WallCheck wallCheck;

    void Awake()
    {
        // layerMask = LayerMask.GetMask("Wall", "Ground", "MovingGround");
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
        else if(wallCheck != null)
        {
            if(bounceAmount > 0 && wallCheck.IsTouching())
                DoBounce();
            else if(bounceAmount <= 0)
                ProjectileDespawn();
        }
        else
        {
            ProjectileDespawn();
        }

        SoundManager.Singleton.PlayAttackAudio(collision_SFX);
    }

    void DoBounce()
    {
        bounceAmount--;
        facing *= -1f;
        pierces = 0;
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
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
            else if(impact.GetComponent<ParticleAttack>())
            {
                impact.GetComponent<ParticleAttack>().SetData(impactDamage, impactDuration);
            }
        }
    }

    public void ManuallyDetonate()
    {
        ProjectileDespawn();
    }
}
