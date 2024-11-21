using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Spawner : MonoBehaviour
{
    public GameObject EasyEnemyPrefab;
    public GameObject DuoFighterPrefab;

    public List<Sprite> easyEnemySprites;
    public List<Sprite> duoFighterSprites;

    public int numberOfEnemies = 3;
    public int numberOfEnemiesSecondWave = 5;
    public int numberOfEnemiesThirdWave = 7;
    public float spawnOffset = 0.5f; // Offset to ensure enemies don't move off the screen
    public List<Sprite> enemySprites = new List<Sprite>();

    private bool finishable = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (finishable && enemies.Length == 0)
        {
            Debug.Log("Akkora győzelmet arattunk, hogy a holdról is látszik");
        }
    }
    private IEnumerator SpawnEnemies()
    {
        // Get the screen width in world units

        SpawnWave(numberOfEnemies);

        yield return new WaitForSeconds(10);
        Vector3 spawnPosition;
        GameObject enemy;

        spawnPosition = new Vector3(0, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;

        yield return new WaitForSeconds(10);

        SpawnWave(numberOfEnemiesSecondWave);
        yield return new WaitForSeconds(10);

        spawnPosition = new Vector3(0, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;

        yield return new WaitForSeconds(10);
        SpawnWave(numberOfEnemiesThirdWave);
        yield return new WaitForSeconds(10);

        // Az első ellenség a képernyő bal oldalán spawnol
        spawnPosition = new Vector3(-Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;
        enemy.GetComponent<DuoFighters>().FireRate = 0.5f;

        yield return new WaitForSeconds(4);

        // A második ellenség a képernyő jobb oldalán spawnol
        spawnPosition = new Vector3(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;
        enemy.GetComponent<DuoFighters>().FireRate = 0.5f;

        finishable = true;

    }
    private void SpawnWave(int enemyNumber)
    {
        for (int i = 0; i < enemyNumber; i++)
        {
            float screenWidth = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
            float startX = -screenWidth / 2.0f + spawnOffset;
            float spacing = (screenWidth - 2 * spawnOffset) / (enemyNumber + 1);
            Vector3 spawnPosition;
            GameObject enemy;

            // Calculate the position for each enemy
            float xPosition = startX + spacing * (i + 1);
            spawnPosition = new Vector3(xPosition, Camera.main.orthographicSize + 2, 0);
            enemy = Instantiate(EasyEnemyPrefab, spawnPosition, Quaternion.identity);
            enemy.GetComponent<EasyEnemy>().FireRate = 0.3f;
            enemy.GetComponent<EasyEnemy>().EnemySprites = enemySprites;


        }
    }
}

