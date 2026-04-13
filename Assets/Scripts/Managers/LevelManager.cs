using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    // should take care of everything that happens throughout the level. By level i mean a single map. Should 
    //handle potion count, difficulty. Depending on potion count, it should tell game manager to switch maps. 
    public static LevelManager Singleton { get; private set; }

    private int potionCount;
    public Slider potionProgressBar;
    public int difficulty = 1;

    [SerializeField] private GameObject portalObject;
    private Vector2 portalHeightOffset = new Vector2(0f, -3f);

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }
    void Start()
    {
        StartCoroutine(StartTheScene());
    }

    IEnumerator StartTheScene()
    {
        ResetPotionCount();
        resetDifficulty();
        updateProgressBar();
        
        yield return new WaitForSeconds(0.5f);

        if(TransitionAnimations.Singleton)
            TransitionAnimations.Singleton.FadeOut();
            
        GameManager.Singleton.spawnedPlayer.GetComponent<PlayerAttack>().ResetAttackCooldown();

        yield return new WaitForSeconds(1f);

        MuffinSpawner.Singleton.SpawnPotion();
        SpawnerManager.Singleton.StartSpawners();
    }

    public void AddPotion()
    {
        potionCount++;
        updateProgressBar();
        GameManager.Singleton.AddPotionCount();

        if (PotionGoalReached())
        {
            SpawnNextMapPortal();
        }
    }

    public bool PotionGoalReached() 
    {
        return(potionCount >= GameManager.Singleton.GetCurrentPotionsNeeded());
    }

    private void SpawnNextMapPortal()
    {
        Vector2 spawnLocation = MuffinSpawner.Singleton.GetRandomLocation() + portalHeightOffset;
        GameObject _portal = Instantiate(portalObject, spawnLocation, Quaternion.identity);
        _portal.GetComponent<NextMapPortal>().ShowPortal();
    }

    public void SpawnStartMapPortal(Vector2 location)
    {
        GameObject _portal = Instantiate(portalObject, location, Quaternion.identity);
        _portal.GetComponent<NextMapPortal>().HidePortal();
    }

    public void increaseDifficulty()
    {
        difficulty++;
    }

    public void resetDifficulty()
    {
        difficulty = 1;
    }

    public void ResetPotionCount()
    {
        potionCount = 0;
    }

    private void updateProgressBar()
    {
        float percentageComplete = (float)potionCount / (float)GameManager.Singleton.GetCurrentPotionsNeeded();
        potionProgressBar.value = percentageComplete;
    }

    public float GetPredictionProgress()
    {
        float predPercentage = (float)(potionCount + 1) / (float)GameManager.Singleton.GetCurrentPotionsNeeded();
        return((predPercentage > 1f) ? 1f : predPercentage);
    }
}
