using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Singleton { get; private set; }

    [SerializeField] private List<CharacterSO> characterList = new();
    private List<CharacterSO> inGameCharacterList = new();
    private int cooldownThreshold = 3;
    private bool cooldownActive = false;

    [HideInInspector]
    public Transform characterTransform;
    [HideInInspector]
    public SpriteRenderer characterModel;

    private CharacterSO currentCharacter;
    private CharacterSO nextCharacter;
    private CharacterSO lastCharacter;

    [Space]
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

    public void ChangeCharacter(Sprite newModel, float atkCD)
    {
        characterModel.sprite = newModel;
        PlayerAttack.Singleton.attackCD = atkCD;
    }

    public void InitializeCharacters()
    {
        cooldownActive = false;
        UpdateCharacterList();

        // currentCharacter = characterList[startingCharacterIndex];
        // playedCharacters.Add(currentCharIndex);

        nextCharacter = RollNext();
    }

    public void BecomeNewCharacter(CharacterSO specificCharacter = null)
    {
        // Add last character back to the character pool
        // ONLY IF cooldown is active
        if(lastCharacter != null && cooldownActive)
        {
            inGameCharacterList.Add(lastCharacter);
        }
        
        // Get temp character data
        CharacterSO previous = currentCharacter;
        CharacterSO next = specificCharacter != null ? specificCharacter : nextCharacter;

        // Get new last character (current character)
        lastCharacter = previous;

        // HERE is where we actually get the new character
        currentCharacter = next;

        // Remove current character from character pool
        inGameCharacterList.Remove(currentCharacter);

        // Add last character back to the character pool
        // EARLY, but only if the cooldown isn't active
        if(!cooldownActive && lastCharacter != null)
        {
            inGameCharacterList.Add(lastCharacter);
            lastCharacter = null;
        }

        nextCharacter = RollNext();

        if (PlayerAttack.Singleton) 
            PlayerAttack.Singleton.SetCharacter(currentCharacter);
    }

    private CharacterSO RollNext()
    {
        return(inGameCharacterList[Random.Range(0, inGameCharacterList.Count)]);
    }

    public CharacterSO GetCurrentCharacter()
    {
        return(currentCharacter);
    }

    public CharacterSO GetNextCharacter()
    {
        return(nextCharacter);
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

        inGameCharacterList = new List<CharacterSO>(characterList);

        if(characterList.Count > cooldownThreshold)
            cooldownActive = true;
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
