using UnityEngine;
using System.Collections.Generic;

public enum MagicianAttacks { Cards, Rabbit, Birds }

[CreateAssetMenu(menuName = "Characters/Magician Character")]
public class MagicianCharacter : CharacterSO
{
    [SerializeField] private MagicianAttacks[] availableAttacks;
    [SerializeField] private MagicianAttacks currentAttack;
    private int potionCount;

    [Header("Card Settings")]
    [SerializeField] private GameObject cardObject;
    [SerializeField] private int cardCount;
    [SerializeField] private float cardInterval;
    [SerializeField] private float cardVelocity;

    [Header("Rabbit Settings")]
    [SerializeField] private GameObject rabbitObject;
    [SerializeField] private int MAX_RABBITS = 3;
    [SerializeField] private List<GameObject> spawnedRabbits = new List<GameObject>();

    [Header("Bird Settings")]
    [SerializeField] private GameObject birdObject;
    [SerializeField] private int birdCount;
    [SerializeField] private float birdInterval;

    public override void UseWeapon(Transform origin, PlayerAttack playerAttack)
    {
        if(potionCount != GameManager.Singleton.GetPotionSum())
        {
            potionCount = GameManager.Singleton.GetPotionSum();
            currentAttack = (MagicianAttacks)Random.Range(0, availableAttacks.Length);
        }

        switch(currentAttack)
        {
            case MagicianAttacks.Cards:
                {
                    CardAttack(origin, playerAttack);
                    break;
                }
            case MagicianAttacks.Rabbit:
                {
                    RabbitAttack(origin, playerAttack);
                    break;
                }
            case MagicianAttacks.Birds:
                {
                    BirdAttack(origin, playerAttack);
                    break;
                }
        }
    }

    private void CardAttack(Transform origin, PlayerAttack playerAttack)
    {
        playerAttack.BurstAttack(cardObject, cardCount, cardInterval, cardVelocity, attackPower);
    }

    private void RabbitAttack(Transform origin, PlayerAttack playerAttack)
    {
        if(spawnedRabbits.Count > MAX_RABBITS - 1)
        {
            // Check if any of the rabbits were destroyed
            for(int i = 0; i < MAX_RABBITS; i++)
            {
                // If null, remove it from the list
                if(spawnedRabbits[i] == null)
                {
                    spawnedRabbits.RemoveAt(i);
                    break;
                }
                // If we reach the end and no rabbits were removed, get rid of last in the list
                else if (i == MAX_RABBITS - 1 && spawnedRabbits.Count > MAX_RABBITS - 1)
                {
                    Destroy(spawnedRabbits[i]);
                    spawnedRabbits.RemoveAt(i);
                }
            }
        }

        GameObject rabbit = Instantiate(rabbitObject, origin.position, Quaternion.identity); 
        spawnedRabbits.Add(rabbit);
    }

    private void BirdAttack(Transform origin, PlayerAttack playerAttack)
    {
        playerAttack.SimpleBurstAttack(birdObject, birdCount, birdInterval);
    }
}
