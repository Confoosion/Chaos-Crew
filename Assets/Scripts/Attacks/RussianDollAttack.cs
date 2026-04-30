using UnityEngine;

// Sally the Mechanic's attack
public class RussianDollAttack : MonoBehaviour
{
    private float damage;
    private int extraDolls = 0;
    private Vector3 extraDollOffset = new Vector3(3f, 0f, 0f);

    public GameObject impactObject;
    private float impactDamage;
    private float impactDuration;
    public AudioClip impactSFX;

    public void SetData(float dmg, int _extraDolls)
    {
        damage = dmg;
        extraDolls = _extraDolls;
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
                DollDespawn();
        }
    }

    private void DollDespawn()
    {
        SoundManager.Singleton.PlayAttackAudio(impactSFX);
        SpawnImpact();
        TrySpawnMoreDolls();
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

    private void TrySpawnMoreDolls()
    {
        if(extraDolls > 0)
        {
            GameObject doll = Instantiate(this.gameObject, transform.position + extraDollOffset, Quaternion.identity);
            doll.GetComponent<RussianDollAttack>().SetData(damage, extraDolls - 1);
            doll.GetComponent<RussianDollAttack>().SetImpactData(impactDamage, impactDuration);

            doll = Instantiate(this.gameObject, transform.position - extraDollOffset, Quaternion.identity);
            doll.GetComponent<RussianDollAttack>().SetData(damage, extraDolls - 1);
            doll.GetComponent<RussianDollAttack>().SetImpactData(impactDamage, impactDuration);
        }
    }
}
