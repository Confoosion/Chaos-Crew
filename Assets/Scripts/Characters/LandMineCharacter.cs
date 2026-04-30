using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Characters/Land Mine Character")]
public class LandMineCharacter : CharacterSO
{
    [SerializeField] private int extraDolls = 0;
    private List<GameObject> landMines = new List<GameObject>();
    private float explosionDuration = 0.2f;

    public override void UseWeapon(Transform origin, PlayerAttack playerAttack)
    {
        if (landMines.Count > 9)
        {   // Check if any of the 10 land mines were destroyed
            for (int i = 0; i < landMines.Count; i++)
            {
                // If null, remove it from the list
                if (landMines[i] == null)
                {
                    landMines.RemoveAt(i);
                    // i--;
                    break;
                }
                // If we reach the end and no mines were removed, get rid of last in the list
                else if (i == landMines.Count - 1 && landMines.Count > 9)
                {
                    Destroy(landMines[i]);
                    landMines.RemoveAt(i);
                }
            }
        }

        GameObject atk = Instantiate(attackObject, origin.position, Quaternion.identity);
        landMines.Insert(0, atk);

        atk.GetComponent<RussianDollAttack>().SetData(attackPower, extraDolls);
        atk.GetComponent<RussianDollAttack>().SetImpactData(attackPower, explosionDuration);
    }
}
