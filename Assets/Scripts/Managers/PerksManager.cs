using UnityEngine;
using System.Collections.Generic;

public class PerksManager : MonoBehaviour
{
    public static PerksManager Singleton { get; private set; }

    private PlayerControl playerControl;
    private PlayerAttack playerAttack;
    [SerializeField] private int maxActivePerks = 1; // Change to 2 whenever you're ready

    private CharacterSO character;
    [SerializeField] private List<PerkSO> activePerks = new List<PerkSO>();

    [Space]

    [SerializeField] private PerkMachineSO perkMachine;

    void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        // if (playerAttack.GetCharacter() != null)
        //     character = playerAttack.GetCharacter();
    }

    public void SetPlayerControlReference(PlayerControl pControl)
    {
        playerControl = pControl;
    }

    public void SetPlayerAttackReference(PlayerAttack pAttack)
    {
        playerAttack = pAttack;
    }

    // Returns true if successful
    public bool EquipPerk(PerkSO newPerk, int slot = 0)
    {
        if (slot < 0 || slot >= maxActivePerks)
        {
            Debug.LogWarning($"Invalid perk slot: {slot}");
            return false;
        }

        // Pad list to fit slots
        while (activePerks.Count <= slot)
            activePerks.Add(null);

        // Don't re-equip the same perk
        if (activePerks[slot] == newPerk) return false;

        // Unequip whatever is currently in that slot
        if (activePerks[slot] != null)
            UnapplyPerk(activePerks[slot]);

        activePerks[slot] = newPerk;
        // ApplyPerk(newPerk);
        return true;
    }

    public void UnequipPerk(int slot = 0)
    {
        if (slot >= activePerks.Count || activePerks[slot] == null) return;

        // UnapplyPerk(activePerks[slot]);
        activePerks[slot] = null;
    }

    public PerkSO GetActivePerk(int slot = 0)
    {
        if (slot >= activePerks.Count) return null;
        return activePerks[slot];
    }

    public void ApplyActivePerk()
    {
        if(activePerks.Count > 0)
            ApplyPerk(activePerks[0]);
    }

    public bool IsEquipped(PerkSO perk) => activePerks.Contains(perk);

    private void ApplyPerk(PerkSO perk)
    {
        if(perk == null) return;
        
        switch (perk.perkType)
        {
            case PerkType.DoubleJump:
                playerControl.extraJumps += (int)perk.value;
                break;
            case PerkType.Speed:
                playerControl.MAXSPEED += perk.value;
                break;
            case PerkType.ThickBoots:
                playerControl.EquipThickBoots(true);
                break;
            // case PerkType.Damage:
            //     // Implemented in EnemyController (when taking damage, checks for damage perk and applies dmg value)
            //     break;
            // case PerkType.SlowEnemies:
            //     // Implemented in EnemyController (when spawned, checks for slow enemies perk and decreases speed)
            //     break;
            // case PerkType.FortuneTeller:
            //     // Would like to implement in the Muffin class and have a head sprite of the character show above the muffin, but we don't have any character sprites yet
            //     break;
        }
    }

    private void UnapplyPerk(PerkSO perk)
    {
        switch (perk.perkType)
        {
            case PerkType.DoubleJump:
                playerControl.extraJumps -= (int)perk.value;
                break;
            case PerkType.Speed:
                playerControl.MAXSPEED -= perk.value;
                break;
            case PerkType.ThickBoots:
                playerControl.EquipThickBoots(false);
                break;
            // case PerkType.FortuneTeller:
            //     break;
            // case PerkType.Damage:
            //     // Nothing to reverse since it's all done in EnemyController
            //     break;
            // case PerkType.SlowEnemies:
            //     // Nothing to reverse since it's all done in EnemyController
            //     break;
        }
    }

    public PerkMachineSO GetPerkMachine()
    {
        return(perkMachine);
    }
}