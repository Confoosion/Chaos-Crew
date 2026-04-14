using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SpawnerManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static SpawnerManager Singleton { get; private set; }

    [Header("Enemies")]
    [SerializeField] private GameObject normalEnemy;
    [SerializeField] private GameObject heavyEnemy;
    [SerializeField] private GameObject angryNormalEnemy;
    [SerializeField] private GameObject angryHeavyEnemy;

    [Header("OtherVariables")]

    [SerializeField] public List<GameObject> spawnersInWorld;
    [SerializeField] public GameObject oneAngrySpawner;
    public List<GameObject> allEnemiesInWorld;
    [SerializeField] private int waveNumber = 1;
    private int difficultyIncreaseValue = 7;
    private int spawnerIndex = 0;
    [SerializeField] float baseWaveInterval = 5.0f;

    private List<GameObject> specificSpawnList;

    [SerializeField] public bool stopGame = false;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }

    // void Start()
    // {
    //     StartCoroutine(BeginSpawning());
    // }

    public void StartSpawners()
    {
        StartCoroutine(BeginSpawning());
    }

    IEnumerator BeginSpawning()
    {
        while (!stopGame)
        {
            populateSpawners(waveNumber);
            yield return new WaitForSeconds(baseWaveInterval + LevelManager.Singleton.difficulty);
        }

    }

    private void populateSpawners(int wave)
    {
        specificSpawnList = spawnersInWorld[spawnerIndex].GetComponent<SpawnerController>().spawnList;

        if (waveNumber % difficultyIncreaseValue == 0)
        {
            LevelManager.Singleton.increaseDifficulty();
        }

        int randNum = GetRandomNumber(1, 3) + LevelManager.Singleton.difficulty;
        if (waveNumber <= 3)
        {
            for (int i = 0; i < randNum; i++)
            {
                specificSpawnList.Add(normalEnemy);
            }
        }
        else
        {
            for (int i = 0; i < randNum; i++)
            {
                float temp = Random.value;
                if (temp <= 0.5f)
                {
                    specificSpawnList.Add(normalEnemy);
                }
                else
                {
                    specificSpawnList.Add(heavyEnemy);
                }
            }
        }

        // update information
        spawnersInWorld[spawnerIndex].GetComponent<SpawnerController>().StartSpawning();

        updateSpawnerIndex();
        waveNumber++;
    }

    private void updateSpawnerIndex()
    {

        int newSpawnerIndex = GetRandomNumber(0, spawnersInWorld.Count - 1);
        while (newSpawnerIndex == spawnerIndex)
        {
            newSpawnerIndex = GetRandomNumber(0, spawnersInWorld.Count - 1);
        }
        spawnerIndex = newSpawnerIndex;


    }

    private int GetRandomNumber(int min, int max)
    {
        return Random.Range(min, max + 1);
    }



    public void CreateAngryVariant(int num)
    {
        if (oneAngrySpawner == null)
        {
            if (num == 0)
            {
                specificSpawnList.Add(angryNormalEnemy);
            }
            else if (num == 1)
            {
                specificSpawnList.Add(angryHeavyEnemy);
            }
        }
        else
        {
            if (num == 0)
            {
                oneAngrySpawner.GetComponent<SpawnerController>().spawnList.Add(angryNormalEnemy);
            }
            else if (num == 1)
            {
                oneAngrySpawner.GetComponent<SpawnerController>().spawnList.Add(angryHeavyEnemy);

            }
        }

    }

    public void RemoveAllEnemies()
    {
        foreach (GameObject enemy in allEnemiesInWorld)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        allEnemiesInWorld.Clear();
    }
}
