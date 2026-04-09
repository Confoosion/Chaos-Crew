using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Singleton { get; private set; }

    [SerializeField] private List<CharacterSO> characterList = new();
    [SerializeField] private List<CharacterSO> remainingCharacters = new();

    [HideInInspector]
    public Transform characterTransform;
    [HideInInspector]
    public SpriteRenderer characterModel;

    private CharacterSO currentCharacter;
    private CharacterSO nextCharacter;
    [SerializeField] private CharacterSO[] lastThreeCharacters = new CharacterSO[3];

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

    // void Start()
    // {
    //     remainingCharacters = new List<CharacterSO>(characterList);
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
        // Become specific character based on parameter
        if (specificCharacter != null)
        {
            currentCharacter = specificCharacter;
            remainingCharacters.Remove(specificCharacter);
        }
        // Become a randomly chosen character
        else
        {
            // If the next character is known, become it
            if(nextCharacter != null)
            {
                currentCharacter = nextCharacter;
            }
            // Otherwise, just randomly pick the character
            else
            {
                currentCharacter = PickRandomCharacter();
            }
        }

        if(characterList.Count > 4)
        {
            lastThreeCharacters[2] = lastThreeCharacters[1];
            lastThreeCharacters[1] = lastThreeCharacters[0];
            lastThreeCharacters[0] = currentCharacter;
        }

        if (PlayerAttack.Singleton) 
            PlayerAttack.Singleton.SetCharacter(currentCharacter);

        nextCharacter = PickRandomCharacter();
    }

    private CharacterSO PickRandomCharacter()
    {
        if(remainingCharacters.Count == 0)
        {   // Refill the list
            RefillCharacterList();
        }

        CharacterSO selected = nextCharacter;
        int randomIndex = 0;
        if(characterList.Count > 4)
        {
            while(lastThreeCharacters.Contains(selected))
            {
                randomIndex = Random.Range(0, remainingCharacters.Count);
                selected = remainingCharacters[randomIndex];
            }
        }
        else
        {
            randomIndex = Random.Range(0, remainingCharacters.Count);
            selected = remainingCharacters[randomIndex];
        }

        remainingCharacters.RemoveAt(randomIndex);

        return(selected);
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

        RefillCharacterList();
    }

    private void RefillCharacterList()
    {
        remainingCharacters = new List<CharacterSO>(characterList);
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
