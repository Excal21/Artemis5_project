using System.Collections.Generic;
using UnityEngine;

public class Level1Spawner : MonoBehaviour
{
    public GameObject EasyEnemyPrefab;
    
    public List<Sprite> easyEnemySprites;

    public int numberOfEnemies = 5;
    public float spawnOffset = 0.5f; // Offset to ensure enemies don't move off the screen
    public List<Sprite> enemySprites = new List<Sprite>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the screen width in world units
        float screenWidth = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        float startX = -screenWidth / 2.0f + spawnOffset;
        float spacing = (screenWidth - 2 * spawnOffset) / (numberOfEnemies + 1);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Calculate the position for each enemy
            float xPosition = startX + spacing * (i + 1);
            Vector3 spawnPosition = new Vector3(xPosition, Camera.main.orthographicSize + 2, 0);
            GameObject enemy = Instantiate(EasyEnemyPrefab, spawnPosition, Quaternion.identity);
            enemy.GetComponent<EasyEnemy>().enemySprites = enemySprites;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

