using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class SpawnKillBlocked
{

    [UnityTest]
    public IEnumerator SpawnKillBlockedWithEnumeratorPasses()
    {
        // Betöltjük az EntityTest jelenetet
        SceneManager.LoadScene("EntityTest");
        GameObject player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");

        // Várunk, amíg a jelenet betöltődik
        yield return new WaitForSeconds(1);
        GameObject spaceship = Object.Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity);
        spaceship.GetComponent<Player>().Speed = 40f;
        spaceship.GetComponent<Player>().ProjectileSpeed = 40f;


        // Betöltjük az EasyEnemy prefabot az Assets/Prefabs mappából
        GameObject easyEnemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EasyEnemyBase.prefab");
        Assert.IsNotNull(easyEnemyPrefab, "A teszt nem találta az ellenség prefabot");

        float screenWidth = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        float startX = -screenWidth / 2.0f + 0.5f;
        float spacing = (screenWidth - 2 * 0.5f) / 11;
        Vector3 spawnPosition;
        GameObject enemy;

        GameObject[] enemies = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {
            // Calculate the position for each enemy
            float xPosition = startX + spacing * (i + 1);
            spawnPosition = new Vector3(xPosition, Camera.main.orthographicSize + 2, 0);
            enemy = Object.Instantiate(easyEnemyPrefab, spawnPosition, Quaternion.identity);
            enemies[i] = enemy;
            enemy.GetComponent<EasyEnemy>().speed = 0;
             Assert.IsNotNull(enemies[i], $"Nem sikerült létrehozni a(z) {i + 1}. ellenséget");
        }
        // Várunk 500 képkockát
        for (int i = 0; i < 100; i++)
        {
            spaceship.GetComponent<Player>().Right();
            spaceship.GetComponent<Player>().Right();
            spaceship.GetComponent<Player>().Right();
            spaceship.GetComponent<Player>().Right();

            yield return null;
        }
        for (int i = 0; i < 200; i++)
        {
            spaceship.GetComponent<Player>().Shoot();
            spaceship.GetComponent<Player>().Left();
            spaceship.GetComponent<Player>().Left();


            yield return null;
        }

        // Ellenőrizzük, hogy az EasyEnemy elpusztult-e
        for (int i = 0; i < 10; i++)
        {
            Assert.IsFalse(enemies[i] == null || enemies[i].Equals(null), $"A(z) {i + 1}. ellenfél meghalt");
        }
    }
}


