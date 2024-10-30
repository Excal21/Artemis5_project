using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Spawner : MonoBehaviour
{
    public GameObject EasyEnemyPrefab;
    public GameObject DuoFighterPrefab;
    
    public List<Sprite> easyEnemySprites;
    public List<Sprite> duoFighterSprites;

    public int numberOfEnemies = 5;
    public float spawnOffset = 0.5f; // Offset to ensure enemies don't move off the screen
    public List<Sprite> enemySprites = new List<Sprite>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies(){
        // Get the screen width in world units
        float screenWidth = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        float startX = -screenWidth / 2.0f + spawnOffset;
        float spacing = (screenWidth - 2 * spawnOffset) / (numberOfEnemies + 1);
        Vector3 spawnPosition;
        GameObject enemy;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Calculate the position for each enemy
            float xPosition = startX + spacing * (i + 1);
            spawnPosition = new Vector3(xPosition, Camera.main.orthographicSize + 2, 0);
            enemy = Instantiate(EasyEnemyPrefab, spawnPosition, Quaternion.identity);
            enemy.GetComponent<EasyEnemy>().EnemySprites = enemySprites;


        }
        yield return new WaitForSeconds(10);
        spawnPosition = new Vector3(0, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = enemySprites;

    }

}

