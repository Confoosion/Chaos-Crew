using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private CharacterSO character;
    //[SerializeField] private float baseAttackPower;
    public float attackCD;

    private bool canAttack = true;
    private int nukeUses;
    private Coroutine cooldownRoutine;

    public static PlayerAttack Singleton;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        character = CharacterManager.Singleton.GetCurrentCharacter();
        character.EquipCharacter();

        PerksManager.Singleton.SetPlayerAttackReference(this);
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && GameManager.Singleton.IsPlayerAlive() && !GameManager.Singleton.IsPaused())
        {
            Attack();
        }
    }

    public CharacterSO GetCharacter()
    {
        return character;
    }

    public void SetCharacter(CharacterSO characterSO)
    {
        character = characterSO;
        character.EquipCharacter();
        nukeUses = 0;
    }

    private void Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            character.UseWeapon(transform, this);
            cooldownRoutine = StartCoroutine(AttackCooldown(attackCD));
            SoundManager.Singleton.PlayCharacterAttackAudio(character);
        }
    }

    IEnumerator AttackCooldown(float cd)
    {
        yield return new WaitForSeconds(cd);
        canAttack = true;

        PlayerControl pControl = GetPlayerControl();
        if(pControl)
        {
            pControl.ResetSlow();
        }
    }

    public void ResetAttackCooldown()
    {
        if(cooldownRoutine != null)
            StopCoroutine(cooldownRoutine);
        canAttack = true;

        PlayerControl pControl = GetPlayerControl();
        if(pControl)
        {
            pControl.ResetSlow();
        }
    }

    public void SlowPlayerMovement(float slow)
    {
        if(!canAttack)
        {
            GetPlayerControl().SlowMovement(slow);
        }
    }

    public void BurstAttack(GameObject atkObject, int count, float interval, float burstVelocity, float atkPower, Vector2 angleForce = default, bool destroyOnTerrain = false, float impactDamage = 0f, float impactDuration = 0f)
    {
        StartCoroutine(DoBurstAttack(atkObject, count, interval, burstVelocity, atkPower, angleForce, destroyOnTerrain, impactDamage, impactDuration));
    }

    IEnumerator DoBurstAttack(GameObject atkObject, int count, float interval, float burstVelocity, float atkPower, Vector2 angleForce, bool destroyOnTerrain, float impactDamage, float impactDuration)
    {
        float direction = (transform.localScale.x == 1) ? -1f : 1f;

        for (int i = 0; i < count; i++)
        {
            GameObject atk = Instantiate(atkObject, transform.position + new Vector3(direction * 0.5f, 0f, 0f), Quaternion.identity);
            Transform atkTransform = atk.transform;
            atkTransform.localScale = new Vector3(atkTransform.localScale.x * -direction, atkTransform.localScale.y, atkTransform.localScale.z);

            if (angleForce != Vector2.zero)
            {
                atk.GetComponent<RangeAttack>().SetData(atkPower, destroyOnTerrain);
                atk.GetComponent<RangeAttack>().SetImpactData(impactDamage, impactDuration);

                Vector2 directedForce = new Vector2(angleForce.x * direction, angleForce.y);
                atk.GetComponent<Rigidbody2D>().AddForce(directedForce, ForceMode2D.Impulse);
            }
            else
            {
                atk.GetComponent<ProjectileAttack>().SetData(atkPower, burstVelocity, direction);
            }

            yield return new WaitForSeconds(interval);
        }
    }

    public void SimpleBurstAttack(GameObject atkObject, int count, float interval)
    {
        StartCoroutine(SimpleBurst(atkObject, count, interval));
    }

    // Velocity and damage does NOT matter for this burst. The burst object is assumed to have its own behaviors
    IEnumerator SimpleBurst(GameObject atkObject, int count, float interval)
    {
        float direction = (transform.localScale.x == 1) ? -1f : 1f;

        for(int i = 0; i < count; i++)
        {
            GameObject atk = Instantiate(atkObject, transform.position, Quaternion.identity);
            Transform atkTransform = atk.transform;
            atkTransform.localScale = new Vector3(atkTransform.localScale.x * -direction, atkTransform.localScale.y, atkTransform.localScale.z);

            yield return new WaitForSeconds(interval);
        }
    }

    public PlayerControl GetPlayerControl()
    {
        return (GetComponent<PlayerControl>());
    }

    public bool ActivateNuke(int maxUses)
    {
        if (nukeUses < maxUses)
        {
            nukeUses++;
            SpawnerManager.Singleton.RemoveAllEnemies();
            return (true);
        }
        return (false);
    }
}
