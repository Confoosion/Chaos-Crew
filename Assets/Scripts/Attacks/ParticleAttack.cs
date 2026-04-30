using UnityEngine;
using System.Collections;

public class ParticleAttack : MonoBehaviour
{
    private ParticleSystem particle;
    private float damage;
    private float duration;
    private Transform origin;
    private float direction;
    private float timer;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void SetData(float dmg, float atkDuration, Transform facing = null)
    {
        if (duration != atkDuration)
        {
            damage = dmg;
            duration = atkDuration;
            origin = facing;

            particle.Play();
        }

        timer = duration;
    }

    void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
        else if(particle.main.loop)
        {
            particle.Stop(true);
            StartCoroutine(RemoveParticleObject(0.1f));
        }

        // Rotates particles to spawn in the correct direction
        if (origin != null)
        {
            direction = (origin.localScale.x == 1) ? 1f : -1f;
            var particleShape = particle.shape;
            particleShape.rotation = new Vector3(0f, -180f + 90f * direction, 0f);
        }
    }

    IEnumerator RemoveParticleObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }

    void OnParticleCollision(GameObject collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy!");
            collider.gameObject.GetComponent<EnemyController>().enemyTakeDamage(damage);
        }
    }
}
