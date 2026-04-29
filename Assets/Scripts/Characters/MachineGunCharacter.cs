using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Machine Gun Character")]
public class MachineGunCharacter : CharacterSO
{
    [Header("Machine Gun Stats")]
    [SerializeField] private float speedPenalty = 2.5f;
    [SerializeField] private float bulletVelocity;
    [SerializeField] private float weaponKnockback;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float explosionDuration;

    public override void UseWeapon(Transform origin, PlayerAttack playerAttack)
    {
        float direction = (origin.localScale.x == 1) ? -1f : 1f;
        GameObject atk = Instantiate(attackObject, origin.position + new Vector3(direction * 0.5f, 0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, (direction == -1) ? 0f : 180f)));
        atk.GetComponent<ProjectileAttack>().SetData(attackPower, bulletVelocity, direction);
        
        if(atk.GetComponent<ProjectileAttack>().impactObject != null)
        {
            Debug.Log("Has impact object");
            atk.GetComponent<ProjectileAttack>().SetImpactData(explosionDamage, explosionDuration);
        }

        // Slow player down
        playerAttack.SlowPlayerMovement(speedPenalty);

        // Push player back
        playerAttack.GetPlayerControl().TryPushBack(weaponKnockback);
    }
}
