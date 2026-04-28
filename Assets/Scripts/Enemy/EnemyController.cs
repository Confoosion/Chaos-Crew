using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyController : MonoBehaviour
{
    public EnemySO enemyType;
    [SerializeField] private GameObject deathParticle;

    [SerializeField] protected bool canMove = true;
    [SerializeField] private Vector2 knockbackForce = new Vector2(3f, 5f);
    [SerializeField] private float knockbackCD = 0.75f; 

    protected float speed;
    private float health;
    protected int direction = 1;

    private void Start()
    {
        SpawnerManager.Singleton.allEnemiesInWorld.Add(gameObject);
    }

    protected void SetSpeed()
    {
        // Set speed (also checks for SlowEnemies perk)
        PerkSO slowPerk = PerksManager.Singleton.GetActivePerk();
        if(slowPerk != null && slowPerk.perkType == PerkType.SlowEnemies)
            speed = enemyType.speed - slowPerk.value;
        else
            speed = enemyType.speed;
    }

    protected void SetHealth()
    {
        health = enemyType.health;
    }

    public void SetMoveDirection(int dir)
    {
        direction = dir;
        UpdateSpriteDirection();
    }

    protected void UpdateSpriteDirection()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, transform.localScale.y, transform.localScale.z);
    }

    public void TakeKnockback(float kbForce)
    {
        Vector2 kb = new Vector2(knockbackForce.x * kbForce, knockbackForce.y);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        canMove = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(kb, ForceMode2D.Impulse);
        
        StartCoroutine(KnockedBack(knockbackCD));
    }

    IEnumerator KnockedBack(float duration)
    {
        yield return new WaitForSeconds(duration);
        canMove = true;
        yield return null;
    }

    //ENEMY Taking Damage and dying
    public void enemyTakeDamage(float damage)
    {
        float extraDamage = 0f;

        // Check for damage perk
        PerkSO dmgPerk = PerksManager.Singleton.GetActivePerk();
        if(dmgPerk != null && dmgPerk.perkType == PerkType.Damage)
            extraDamage += dmgPerk.value;

        // god mode debug
        if (PlayerControl.Singleton.GetGodMode()) 
            extraDamage += 100;

        health -= (damage + extraDamage);
        if (health <= 0)
        {
            enemyDeath();
            if (this.gameObject.name.Contains("AngryBasic"))
            {
                GameManager.Singleton.addAngryBasicEnemyKill();
            }
            else if (this.gameObject.name.Contains("AngryHeavy"))
            {
                GameManager.Singleton.addAngryHeavyEnemyKill();
            }
            else if (this.gameObject.name.Contains("Basic"))
            {
                GameManager.Singleton.addBasicEnemyKill();
            }
            else if (this.gameObject.name.Contains("Heavy"))
            {
                GameManager.Singleton.addHeavyEnemyKill();
            }
        }
    }

    private void enemyDeath()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    protected void KillPlayer(GameObject player)
    {
        player.GetComponent<PlayerControl>().playerDeath();
    }

}
