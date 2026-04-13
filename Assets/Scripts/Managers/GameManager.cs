using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }

    [Header("Player")]
    public GameObject playerObject;
    public GameObject spawnedPlayer {get; private set;}
    private bool isAlive = true;
    private bool isPaused = false;

    [Header("Screens")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject pauseScreen;

    [Space]
    [SerializeField] private int START_POTIONS_NEEDED = 5;
    [SerializeField] private int potionsNeededToMoveOn = 5;
    private int potionIncrement = 5;

    private EndScreenScript endScreenScript;

    private int mapCount;
    private List<int> visitedMapIndices = new List<int>();

    // variables for the whole game. End of game stats
    private int potionSum = 0;
    private int basicEnemiesKilled = 0;
    private int angryBasicEnemiesKilled = 0;
    private int angryHeavyEnemiesKilled = 0;
    private int heavyEnemiesKilled = 0;

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

        mapCount = SceneManager.sceneCountInBuildSettings - 3; // subtract 3 removes MainMenu, Shop, and RuinedCity 
        endScreenScript = gameObject.GetComponent<EndScreenScript>();
    }

    void Start()
    {
        int tempCurrency = 0;
        ShopSaveSystem.Load(CharacterManager.Singleton.GetFullCharacterList(), out tempCurrency);
        // if(DebugScene_Script.Singleton)
        //     DebugScene_Script.Singleton.DebugStart();
        pauseScreen.SetActive(false);
    }

    public void RestartGame()
    {
        CharacterManager.Singleton.InitializeCharacters();
        // CharacterManager.Singleton.UpdateCharacterList();

        disableEndScreen();
        disableDeathScreen();

        resetEnemyKills();
        ResetPotionCount();
        resetMapPool();

        potionsNeededToMoveOn = START_POTIONS_NEEDED;

        SceneManager.LoadScene("RuinedCityMap");

        if(PlayerControl.Singleton)
        {
            PlayerControl.Singleton.movementLocked = false;
        }

        isAlive = true;
    }

    public void NextLevel()
    {
        if (visitedMapIndices.Count == mapCount) // when beating the game
        {
            PlayerControl.Singleton.movementLocked = true;
            enableEndScreen();
            SpawnerManager.Singleton.stopGame = true;

            return;
        }

        int newMapIndex = GetRandomNumber(0, mapCount) + 3; // skip MainMenu, Shop, and Ruined City
        
        while (visitedMapIndices.Contains(newMapIndex)) // find map not visited yet
        {
            newMapIndex = GetRandomNumber(0, mapCount) + 3;
        }

        visitedMapIndices.Add(newMapIndex);

        PrepareSceneChange();
        SceneManager.LoadScene(newMapIndex);

        potionsNeededToMoveOn += potionIncrement;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Runs this only if it's a MAP (player is in game)
        if (scene.name != "MainMenu" && scene.name != "ShopTest")
        {
            if (!GameObject.FindGameObjectWithTag("Player"))
            {
                spawnedPlayer = Instantiate(playerObject);
                DontDestroyOnLoad(spawnedPlayer);
                CharacterManager.Singleton.characterTransform = spawnedPlayer.transform;
                CharacterManager.Singleton.characterModel = spawnedPlayer.GetComponentInChildren<SpriteRenderer>();
            }

            if (spawnedPlayer != null)
            {
            #if UNITY_EDITOR
                int tempCurrency;
                ShopSaveSystem.Load(CharacterManager.Singleton.GetFullCharacterList(), out tempCurrency);
                CharacterManager.Singleton.InitializeCharacters();
            #endif

                Transform spawnPoint = GetRandomSpawnPoint();
                if (spawnPoint != null)
                {
                    LevelManager.Singleton.SpawnStartMapPortal(spawnPoint.position);
                    spawnedPlayer.transform.position = spawnPoint.position;
                }
            }
        }
    }

    private void PrepareSceneChange()
    {
        spawnedPlayer.transform.SetParent(null);
        spawnedPlayer.SetActive(true);
    }

    private Transform GetRandomSpawnPoint()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        return spawnPoints.Length > 0 ? spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform : null;
    }

    private void resetMapPool()
    {
        visitedMapIndices.Clear();
    }
    public bool IsPlayerAlive()
    {
        return (isAlive);
    }

    public int GetNumberOfMapsVisited()
    {
        return visitedMapIndices.Count;
    }

    public int GetCurrentPotionsNeeded()
    {
        return(potionsNeededToMoveOn);
    }

    public void PauseGame(bool pause)
    {
        Time.timeScale = (pause) ? 0f : 1f;
        isPaused = pause;
        pauseScreen.SetActive(pause);
        SoundManager.Singleton.PauseAudio(pause);
    }

    public bool IsPaused()
    {
        return(isPaused);
    }

    //end screen stats stuff

    public int GetPotionSum()
    {
        return potionSum;
    }

    public int getBasicEnemiesKilled()
    {
        return basicEnemiesKilled;
    }

    public int getHeavyEnemiesKilled()
    {
        return heavyEnemiesKilled;
    }

    public int getAngryBasicEnemiesKilled()
    {
        return angryBasicEnemiesKilled;
    }

    public int getAngryHeavyEnemiesKilled()
    {
        return angryHeavyEnemiesKilled;
    }

    public void AddPotionCount()
    {
        potionSum++;
    }

    public void ResetPotionCount()
    {
        potionSum = 0;
    }

    public void addBasicEnemyKill()
    {
        basicEnemiesKilled++;
    }

    public void addHeavyEnemyKill()
    {
        heavyEnemiesKilled++;
    }

    public void addAngryBasicEnemyKill()
    {
        angryBasicEnemiesKilled++;
    }

    public void addAngryHeavyEnemyKill()
    {
        angryHeavyEnemiesKilled++;
    }

    public void resetEnemyKills()
    {
        basicEnemiesKilled = 0;
        heavyEnemiesKilled = 0;
        angryBasicEnemiesKilled = 0;
        angryHeavyEnemiesKilled = 0;
    }

    public void enableEndScreen()
    {
        endScreen.SetActive(true);
        endScreenScript.updateText();
        endScreenScript.updateText2();
    }
    public void disableEndScreen()
    {
        endScreen.SetActive(false);
        endScreenScript.updateText();
        endScreenScript.updateText2();
    }

    public void enableDeathScreen()
    {
        isAlive = false;
        deathScreen.SetActive(true);
        endScreenScript.updateText();
        endScreenScript.updateText2();
    }

    public void disableDeathScreen()
    {
        deathScreen.SetActive(false);
        endScreenScript.updateText();
        endScreenScript.updateText2();
    }

    private int GetRandomNumber(int min, int max) //max exclusive
    {
        return UnityEngine.Random.Range(min, max);
    }
}
