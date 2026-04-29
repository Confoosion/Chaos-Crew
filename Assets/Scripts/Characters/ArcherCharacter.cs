using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Archer Character")]
public class ArcherCharacter : CharacterSO
{
    [Header("Archer Stats")]
    [SerializeField] float projectileForce;
    [SerializeField] bool shootExtraArrows;
    [SerializeField] float arrowOffset;
    private bool destroyArrowOnTerrain = true;

    public override void UseWeapon(Transform origin, PlayerAttack playerAttack)
    {
        float direction = (origin.localScale.x == 1) ? -1f : 1f;
        GameObject atk = Instantiate(attackObject, origin.position + new Vector3(0f, 0.25f, 0f), Quaternion.Euler(new Vector3(0f, 0f, (direction == -1) ? 180f : 0f)));

        atk.GetComponent<RangeAttack>().SetData(attackPower, destroyArrowOnTerrain);
        atk.GetComponent<Rigidbody2D>().AddForce(((direction == 1) ? Vector3.right : Vector3.left) * projectileForce, ForceMode2D.Impulse);

        if (shootExtraArrows)
        {
            float[] offsets = { arrowOffset, -arrowOffset };

            foreach (float offset in offsets)
            {
                Vector2 force = ((direction == 1) ? Vector3.right : Vector3.left) * projectileForce + new Vector3(0f, offset, 0f);
                float angle = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;

                atk = Instantiate(attackObject, origin.position + new Vector3(-direction * 0.5f, 0.25f, 0f), Quaternion.Euler(0f, 0f, angle));

                var rangeAtk = atk.GetComponent<RangeAttack>();
                var rb = atk.GetComponent<Rigidbody2D>();
                rangeAtk.SetData(attackPower, destroyArrowOnTerrain);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    } 
}
