using UnityEngine;

public enum PerkRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum PerkType
{
    DoubleJump,
    Speed,
    Damage,
    SlowEnemies,
    FortuneTeller,
    ThickBoots
}

[CreateAssetMenu(menuName = "Perks/Perk")]
public class PerkSO : ScriptableObject
{
    public string perkName;
    [TextArea] public string perkDescription;
    public Sprite icon;

    public PerkRarity perkRarity;
    public PerkType perkType;
    public float value; // jumps, speed amount, dmg multiplier, slow %, etc.
}