using UnityEngine;

// DETECTS HITS!!! Use for range/projectile attacks
public class RangeAttack : MonoBehaviour
{
    private float damage;
    private bool destroyOnTerrain;
    public GameObject impactObject;
    private float impactDamage;
    private float impactDuration;
    public AudioClip impactSFX;

    public void SetData(float dmg, bool removeFromGround)
    {
        damage = dmg;
        destroyOnTerrain = removeFromGround;
    }

    public void SetImpactData(float dmg, float dur)
    {
        impactDamage = dmg;
        impactDuration = dur;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            //Debug.Log("Hit enemy!");
            EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();
            if(enemyController != null && enemyController.enemyTakeDamage(damage))
                ProjectileDespawn();
        }
        else if (destroyOnTerrain && collider.CompareTag("Terrain"))
        {
            //Debug.Log("Despawn attack");
            ProjectileDespawn();
        }
    }

    private void ProjectileDespawn()
    {
        //Debug.Log("Collided");
        SoundManager.Singleton.PlayAttackAudio(impactSFX);
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
