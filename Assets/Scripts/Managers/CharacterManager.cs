using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Singleton { get; private set; }

    [SerializeField] private List<CharacterSO> characterList = new();
    // [SerializeField] public CharacterSO startingCharacter;
    public Transform characterTransform;
    [SerializeField] public SpriteRenderer characterModel;
    //[SerializeField] public PlayerAttack playerAttack;

    [SerializeField] private CharacterSO currentCharacter;

    [SerializeField] private CharacterSetSO[] FULL_CHARACTER_LIST; 

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // void Start()
    // {
    //     UpdateCharacterList();
    // }

    public void ChangeCharacter(Sprite newModel, float atkCD)
    {
        characterModel.sprite = newModel;
        PlayerAttack.Singleton.attackCD = atkCD;
    }

    public CharacterSO GetCurrentCharacter()
    {
        return(currentCharacter);
    }

    public void BecomeNewCharacter(CharacterSO specificCharacter = null)
    {
        if (specificCharacter != null)
        {
            if (currentCharacter && !characterList.Contains(currentCharacter))
            {
                characterList.Add(currentCharacter);
            }
            currentCharacter = specificCharacter;
            characterList.Remove(specificCharacter);
        }
        else
        {
            int newChar_Index = Random.Range(0, characterList.Count);
            CharacterSO newCharacter = characterList[newChar_Index];
            characterList.RemoveAt(newChar_Index);

            if(!characterList.Contains(currentCharacter))
                characterList.Add(currentCharacter);
                
            currentCharacter = newCharacter;
        }

        if (PlayerAttack.Singleton) 
            PlayerAttack.Singleton.SetCharacter(currentCharacter);
    }

    public void UpdateCharacterList()
    {
        characterList = new();

        foreach(CharacterSetSO character in FULL_CHARACTER_LIST)
        {
            if(ShopSaveSystem.GetCharacterData(character.name).isUnlocked)
            {
                characterList.Add(character.GetCurrentUpgrade().character);
            }
        }
    }

    public void AddCharacterToList(CharacterSO character)
    {
        characterList.Add(character);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "RuinedCityMap")
        {
            BecomeNewCharacter(characterList[0]);
        }
    }

    public CharacterSetSO[] GetFullCharacterList()
    {
        return(FULL_CHARACTER_LIST);
    }

    public void HandleCharacterDeath()
    {
        AddCharacterToList(currentCharacter);
        currentCharacter = null;
    }
}
