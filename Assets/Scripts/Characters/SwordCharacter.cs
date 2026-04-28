using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Sword Character")]
public class SwordCharacter : CharacterSO
{
    [SerializeField] private bool dualWield = false;

    public override void UseWeapon(Transform origin, PlayerAttack playerAttack)
    {
        float direction = (origin.localScale.x == 1) ? -1f : 1f;
        GameObject atk = Instantiate(attackObject, origin.position + new Vector3(direction, 0f, 0f), Quaternion.identity, origin);
        atk.GetComponent<MeleeAttack>().SetData(attackPower, attackDuration);

        if(dualWield)
        {
            atk = Instantiate(attackObject, origin.position + new Vector3(-direction, 0f, 0f), Quaternion.Euler(0f, 0f, 180f), origin);
            atk.GetComponent<MeleeAttack>().SetData(attackPower, attackDuration);
        }
    }
}
