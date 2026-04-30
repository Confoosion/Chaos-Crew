using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Bowling Ball Character")]
public class BowlingBallCharacter : CharacterSO
{
    private GameObject bowlingBall;
    [Header("Bowling Ball Stats")]
    [SerializeField] private float rollingSpeed;
    [SerializeField] private int pierceAmount;
    [SerializeField] private int bounceAmount = 100;

    [Space]

    [SerializeField] private float coinDamage;
    [SerializeField] private float coinExplosionDuration;

    public override void UseWeapon(Transform origin, PlayerAttack playerAttack)
    {
        if (bowlingBall == null)
        {
            float direction = (origin.localScale.x == 1) ? -1f : 1f;
            bowlingBall = Instantiate(attackObject, origin.position + new Vector3(direction * 0.5f, 0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, (direction == -1) ? 0f : 180f)));

            bowlingBall.GetComponent<ProjectileAttack>().SetData(attackPower, rollingSpeed, direction, pierceAmount, bounceAmount);
            bowlingBall.GetComponent<ProjectileAttack>().SetImpactData(coinDamage, coinExplosionDuration);
        }
        else
        {
            bowlingBall.GetComponent<ProjectileAttack>().ManuallyDetonate();
            bowlingBall = null;
        }
    }
}
