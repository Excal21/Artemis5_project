using UnityEngine;


public class EasyEnemySpawner : MonoBehaviour
{
    public GameObject EasyEnemyPrefab;
    public int numberOfEnemies = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the screen width in world units
        float screenWidth = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        float startX = -screenWidth / 2.0f;
        float spacing = screenWidth / (numberOfEnemies + 1);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Calculate the position for each enemy
            float xPosition = startX + spacing * (i + 1);
            Vector3 spawnPosition = new Vector3(xPosition, Camera.main.orthographicSize + 2, 0);
            Instantiate(EasyEnemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
