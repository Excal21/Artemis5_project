using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Spawner : MonoBehaviour
{
    #region Level2 Spawner beállításai
    [SerializeField] private GameObject EasyEnemyPrefab;
    [SerializeField] private GameObject DuoFighterPrefab;
    [SerializeField] private GameObject MiniBossPrefab;

    [SerializeField] private List<Sprite> easyEnemySprites;
    [SerializeField] private List<Sprite> duoFighterSprites;

    [SerializeField] private int numberOfEnemiesSecondWave = 5;
    [SerializeField] private float spawnOffset = 0.5f;
    #endregion

    private bool finishable = false;

    void Awake(){
        Application.targetFrameRate = 60;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioHandler.instance.PlayMusic(AudioHandler.Music.LEVEL2);
        StartCoroutine(SpawnEnemies());
    }
    void Update()
    {
        if (finishable)
        {
            //Debug.Log("Finishable");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
            {
                GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
                foreach (GameObject projectile in projectiles)
                {
                    Destroy(projectile);
                }

                //Debug.Log("No more enemies");
                StartCoroutine(FinishLevel());
                finishable = false;
            }
        }
    }

    #region Level2 Spawner metódusai
    private IEnumerator FinishLevel()
    {
        //Debug.Log("Level finished");
        GameObject.Find("HandleNavigation").GetComponent<HandleNavigation>().IsPlayerDeadOrCleared = true;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.GetComponent<Player>().Controllable = false;
            player.GetComponent<Player>().Invincible = true;
            player.GetComponent<Player>().Speed = 5f;
            while (!(player.transform.position.x < 0.1 && player.transform.position.x > -0.1))
            {
                if (player.transform.position.x > 0)
                {
                    player.GetComponent<Player>().Left();
                }
                else
                {
                    player.GetComponent<Player>().Right();
                }
                yield return new WaitForSeconds(0.005f);
            }
            player.GetComponent<Player>().Speed = 10f;
            yield return new WaitForSeconds(1);
            while (player.transform.position.y < 6f)
            {
                player.GetComponent<Player>().Up(true);
                yield return new WaitForSeconds(0.005f);
            }
            player.SetActive(false);
            yield return new WaitForSeconds(1);
            GameObject pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").transform.Find("Canvas - Pause Menu").gameObject;

            GameObject.Find("HandleNavigation").GetComponent<HandleNavigation>().isGamePaused = true;

            GameObject.Find("SaveManager").GetComponent<SaveManager>().SaveGame();

            pauseMenu.transform.Find("Image - Pause Menu Background").gameObject.SetActive(true);
            pauseMenu.transform.Find("Panel - SECTOR CLEARED").gameObject.SetActive(true);
            AudioHandler.instance.StopMusic();
            finishable = false;
        }
    }
    private IEnumerator SpawnEnemies()
    {
        // Get the screen width in world units

        SpawnWave(Random.Range(3, 6));

        yield return new WaitForSeconds(10);
        Vector3 spawnPosition;
        GameObject enemy;

        spawnPosition = new Vector3(0, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;



        yield return new WaitForSeconds(10);

        SpawnWave(numberOfEnemiesSecondWave);
        yield return new WaitForSeconds(10);

        // Az első ellenség a képernyő bal oldalán spawnol
        spawnPosition = new Vector3(-Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;
        enemy.GetComponent<DuoFighters>().FireRate = 0.5f;


        yield return new WaitForSeconds(10);
        SpawnWave(Random.Range(2, 6));
        yield return new WaitForSeconds(10);



        // // A második ellenség a képernyő jobb oldalán spawnol
        // spawnPosition = new Vector3(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize + 2, 0);
        // enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        // enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;
        // enemy.GetComponent<DuoFighters>().FireRate = 0.5f;

        spawnPosition = new Vector3(0, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(MiniBossPrefab, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(5);


        if(GameObject.FindWithTag("Player") != null) finishable = true;
        yield return true;
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
            enemy.GetComponent<EasyEnemy>().EnemySprites = easyEnemySprites;


        }
    }
}

#endregion