using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class CharacterSO : ScriptableObject
{
    public string characterName;
    public Sprite characterModel;
    public GameObject attackObject;
    public Sprite characterIcon;

    [Header("Basic Stats")]
    public int level = 1;
    public float attackPower;
    public float attackDuration;
    public float attackCD;

    [Header("Sounds")]
    public AudioClip attackSFX;
    public float attackVolume = 1f;

    public void EquipCharacter()
    {
        if (characterModel == null)
        {
            return;
        }
        CharacterManager.Singleton.ChangeCharacter(characterModel, attackCD);
    }

    // Use this function for coding attacks
    public abstract void UseWeapon(Transform origin, PlayerAttack playerAttack);
}
