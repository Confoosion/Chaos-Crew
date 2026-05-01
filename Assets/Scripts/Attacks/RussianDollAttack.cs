using UnityEngine;
using System.Collections.Generic;

// Sally the Mechanic's attack
public class RussianDollAttack : MonoBehaviour
{
    private float damage;
    [SerializeField] private List<GameObject> extraDolls;
    // private Vector3 extraDollOffset = new Vector3(3f, 0f, 0f);
    private Vector3 dollLaunchVector = new Vector2(2f, 8f);

    public GameObject impactObject;
    private float impactDamage;
    private float impactDuration;
    public AudioClip impactSFX;

    public void SetData(float dmg, List<GameObject> _extraDolls)
    {
        damage = dmg;
        extraDolls = new List<GameObject>(_extraDolls);
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
        if(extraDolls.Count > 0)
        {
            GameObject temp = extraDolls[0];
            extraDolls.RemoveAt(0);

            GameObject doll = Instantiate(temp, transform.position, Quaternion.identity);
            LaunchDoll(doll, new Vector2(-1f, 1f));
            doll.GetComponent<RussianDollAttack>().SetData(damage, extraDolls);
            doll.GetComponent<RussianDollAttack>().SetImpactData(impactDamage, impactDuration);

            doll = Instantiate(temp, transform.position, Quaternion.identity);
            LaunchDoll(doll, new Vector2(1f, 1f));
            doll.GetComponent<RussianDollAttack>().SetData(damage, extraDolls);
            doll.GetComponent<RussianDollAttack>().SetImpactData(impactDamage, impactDuration);
        }
    }

    private void LaunchDoll(GameObject dollObj, Vector2 direction)
    {
        Rigidbody2D dollRb = dollObj.GetComponent<Rigidbody2D>();
        dollRb.AddForce(dollLaunchVector * direction, ForceMode2D.Impulse);
    }
}
