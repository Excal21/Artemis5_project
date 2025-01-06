using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Spawner : MonoBehaviour
{
    #region Level3 Spawner beállításai
    [SerializeField] private GameObject EasyEnemyPrefab;
    [SerializeField] private GameObject DuoFighterPrefab;
    [SerializeField] private GameObject BossPrefab;

    [SerializeField] private List<Sprite> easyEnemySprites;
    [SerializeField] private List<Sprite> duoFighterSprites;
    [SerializeField] private float spawnOffset = 0.5f;
    #endregion
    private bool finishable = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioHandler.instance.PlayMusic(AudioHandler.Music.LEVEL3);
        StartCoroutine(SpawnEnemies());
    }
    void Update()
    {
        if(finishable){
            Debug.Log("Finishable");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
            {
                GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
                foreach (GameObject projectile in projectiles)
                {
                    Destroy(projectile);
                }

                Debug.Log("No more enemies");
                StartCoroutine(FinishLevel());
                finishable = false;
            }
        }
    }

    #region Level3 Spawner metódusai
    private IEnumerator FinishLevel(){
        //Debug.Log("Level finished");
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Player>().Controllable = false;
        player.GetComponent<Player>().Invincible = true;
        player.GetComponent<Player>().Speed = 5f;
        while(!(player.transform.position.x < 0.1 && player.transform.position.x > -0.1)){
            if(player.transform.position.x > 0){
                player.GetComponent<Player>().Left();
            }
            else{
                player.GetComponent<Player>().Right();
            }
            yield return new WaitForSeconds(0.005f);
        }
        player.GetComponent<Player>().Speed = 10f;
        yield return new WaitForSeconds(1);
        while(player.transform.position.y < 6f)
        {
            player.GetComponent<Player>().Up(true);
            yield return new WaitForSeconds(0.005f);
        }
        player.SetActive(false);
        yield return new WaitForSeconds(1);
        GameObject pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").transform.Find("Canvas - Pause Menu").gameObject;

        GameObject.Find("SaveManager").GetComponent<SaveManager>().SaveGame();

        pauseMenu.transform.Find("Image - Pause Menu Background").gameObject.SetActive(true);
        pauseMenu.transform.Find("Panel - SECTOR CLEARED").gameObject.SetActive(true);
        AudioHandler.instance.StopMusic();
        finishable = false;
    }
    private IEnumerator SpawnEnemies()
    {


        Vector3 spawnPosition;
        GameObject enemy;

        
        yield return new WaitForSeconds(3.5f);


        // Az első ellenség a képernyő bal oldalán spawnol
        spawnPosition = new Vector3(-Camera.main.aspect * Camera.main.orthographicSize+1f, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;
        enemy.GetComponent<DuoFighters>().FireRate = 0.5f;


        yield return new WaitForSeconds(4f);



        // A második ellenség a képernyő jobb oldalán spawnol
        spawnPosition = new Vector3(Camera.main.aspect * Camera.main.orthographicSize-2f, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;
        enemy.GetComponent<DuoFighters>().FireRate = 0.5f;


        yield return new WaitForSeconds(8f);


        // Az első ellenség a képernyő bal oldalán spawnol
        spawnPosition = new Vector3(-Camera.main.aspect * Camera.main.orthographicSize + 1f, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;
        enemy.GetComponent<DuoFighters>().FireRate = 0.5f;


        yield return new WaitForSeconds(4f);



        // A második ellenség a képernyő jobb oldalán spawnol
        spawnPosition = new Vector3(Camera.main.aspect * Camera.main.orthographicSize - 2f, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(DuoFighterPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<DuoFighters>().enemySprites = duoFighterSprites;
        enemy.GetComponent<DuoFighters>().FireRate = 0.8f;

        yield return new WaitForSeconds(10);

        spawnPosition = new Vector3(0, Camera.main.orthographicSize + 2, 0);
        enemy = Instantiate(BossPrefab, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(5);


        finishable = true;
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