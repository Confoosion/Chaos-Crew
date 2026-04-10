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
        lastThreeCharacters[2] = lastThreeCharacters[1];
        lastThreeCharacters[1] = lastThreeCharacters[0];
        lastThreeCharacters[0] = currentCharacter;

        // Become specific character based on parameter
        if (specificCharacter != null)
        {
            currentCharacter = specificCharacter;
            remainingCharacters.Remove(specificCharacter);
        }
        // Become a randomly chosen character
        else
        {
            if(remainingCharacters.Count == 0)
                RefillCharacterList();

            currentCharacter = remainingCharacters[0];
            remainingCharacters.RemoveAt(0);
        }

        if (PlayerAttack.Singleton) 
            PlayerAttack.Singleton.SetCharacter(currentCharacter);
    }

    public CharacterSO GetNextCharacter()
    {
        if(remainingCharacters.Count == 0)
            RefillCharacterList();

        return(remainingCharacters[0]);
    }

    private void RefillCharacterList()
    {
        remainingCharacters = new List<CharacterSO>(characterList);

        // Shuffle the list
        for(int i = remainingCharacters.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (remainingCharacters[i], remainingCharacters[j]) = (remainingCharacters[j], remainingCharacters[i]);
        }

        int protectCount = characterList.Count switch
        {
            <= 3 => 1,
            <= 5 => 2,
            _    => 3
        };

        List<CharacterSO> recentCharacters = new();
        recentCharacters.Add(currentCharacter);
        for(int i = 0; i < lastThreeCharacters.Length && recentCharacters.Count < protectCount; i++)
        {
            if(lastThreeCharacters[i] != null)
                recentCharacters.Add(lastThreeCharacters[i]);
        }

        for(int i = 0; i < protectCount; i++)
        {
            if(!recentCharacters.Contains(remainingCharacters[i])) continue;

            int swapTarget = remainingCharacters.FindIndex(protectCount, c => !recentCharacters.Contains(c));
            if(swapTarget != -1)
                (remainingCharacters[i], remainingCharacters[swapTarget]) = (remainingCharacters[swapTarget], remainingCharacters[i]);
        }
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

        lastThreeCharacters = new CharacterSO[3];
        RefillCharacterList();
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
