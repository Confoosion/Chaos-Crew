using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum SpawnDirection { Right, Left , Random }
public class SpawnerController : MonoBehaviour
{
    public List<GameObject> spawnList;
    [SerializeField] private SpawnDirection spawnDirection; // Which way should the objects spawn
    private float portalAnimationSpeed = 0.1f;
    private Vector3 spawnerScale = new Vector3(1f, 1f, 1f);

    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        yield return ShowPortal(true);

        while (spawnList.Count > 0)
        {
            if(spawnDirection == SpawnDirection.Random)
            {
                SpawnEnemiesIntoWorld();
            }
            else
            {
                SpawnEnemiesIntoWorld(spawnDirection == SpawnDirection.Left ? -1 : 1);                
            }
            
            yield return new WaitForSeconds(0.7f);
        }

        yield return ShowPortal(false);
    }

    private void SpawnEnemiesIntoWorld(int dir = 0)
    {
        if(dir == 0) // Doesn't have a direction, so randomly picks for you
            dir = Random.value < 0.5f ? -1 : 1;

        GameObject enemy = Instantiate(spawnList[0], transform.position, Quaternion.identity);
        // Instantiate(spawnList[0], transform.position);

        if (enemy.GetComponent<BasicEnemyMovement>())//make sure we get this information,....
            enemy.GetComponent<BasicEnemyMovement>().SetMoveDirection(dir); //set direction of enemy

        //SpawnerManager.Singleton.allEnemiesInWorld.Add(spawnList[spawnIndex]); //add to big enemy list

        spawnList.RemoveAt(0);
    }

    IEnumerator ShowPortal(bool open)
    {
        float currScale = transform.localScale.x;
        if(open && transform.localScale == Vector3.zero)
        {
            while(transform.localScale.x < spawnerScale.x)
            {
                currScale += Time.deltaTime;
                transform.localScale = new Vector3(currScale, currScale, currScale);

                yield return new WaitForSeconds(Time.deltaTime * portalAnimationSpeed);
            }

            transform.localScale = spawnerScale;
        }
        else
        {
            while(transform.localScale.x > 0f)
            {
                currScale -= Time.deltaTime;
                transform.localScale = new Vector3(currScale, currScale, currScale);

                yield return new WaitForSeconds(Time.deltaTime * portalAnimationSpeed);
            }

            transform.localScale = Vector3.zero;
        }
    }
}
