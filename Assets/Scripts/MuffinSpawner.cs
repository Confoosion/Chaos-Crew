using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MuffinSpawner : MonoBehaviour
{
    public static MuffinSpawner Singleton { get; private set; }
    [SerializeField] private GameObject potionObj;
    [SerializeField] private float spawnOffset = 1.5f;
    [SerializeField] private float spawnHeight = 5f;
    private List<Transform> platforms = new List<Transform>();
    private Transform potionPlatform;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            ClearPlatforms();
            AddPlatforms();
        }
    }

    public void SpawnPotion()
    {
        if(potionPlatform != null)
            platforms.Add(potionPlatform);

        if (platforms.Count == 0)
        {
            Debug.LogWarning("No platforms available to spawn Potion!");
            return;
        }
    
        // int newMuffinPlatform_Index = Random.Range(0, platforms.Count);
        // muffinPlatform = platforms[newMuffinPlatform_Index];
        // platforms.RemoveAt(newMuffinPlatform_Index);

        // Bounds bounds = muffinPlatform.GetComponent<SpriteRenderer>().bounds;
        // float randomX = Random.Range(bounds.min.x + spawnOffset, bounds.max.x - spawnOffset);
        // float y = bounds.max.y + spawnHeight;

        Instantiate(potionObj, GetRandomLocation(), Quaternion.identity);
    }

    private void AddPlatforms()
    {
        foreach(Transform platform in transform)
        {
            platforms.Add(platform);
        }
    }

    private void ClearPlatforms()
    {
        platforms.Clear();
    }

    public Vector2 GetRandomLocation()
    {
        int newPotionPlatform_Index = Random.Range(0, platforms.Count);
        potionPlatform = platforms[newPotionPlatform_Index];
        platforms.RemoveAt(newPotionPlatform_Index);

        Bounds bounds = potionPlatform.GetComponent<SpriteRenderer>().bounds;
        float randomX = Random.Range(bounds.min.x + spawnOffset, bounds.max.x - spawnOffset);
        float y = bounds.max.y + spawnHeight;
        
        return(new Vector2(randomX, y));
    }
}