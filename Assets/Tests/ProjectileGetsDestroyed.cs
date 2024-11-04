using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ProjectileGetsDestroyed
{
    [UnityTest]
    public IEnumerator ProjectileGetsDestroyedWithEnumeratorPasses()
    {
        // Betöltjük a játékos objektumot
        SceneManager.LoadScene("EntityTest");
        GameObject player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        yield return new WaitForSeconds(1); // Várunk, amíg betöltődik a játékos
        GameObject spaceship = Object.Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity);

        spaceship.GetComponent<Player>().Speed = 40f;
        spaceship.GetComponent<Player>().ProjectileSpeed = 40f;
        Assert.IsNotNull(spaceship, "A teszt nem tudta létrehozni a játékos űrhajóját");

        spaceship.GetComponent<Player>().Shoot();
        GameObject projectile1 = GameObject.FindGameObjectWithTag("PlayerProjectile");
        Assert.IsNotNull(projectile1, "Lövedék nem jött létre");


        // 500 képkockán keresztül meghívjuk a játékos Left metódusát
        for (int i = 0; i < 500; i++)
        {
            spaceship.GetComponent<Player>().Left();
            yield return null;
        }

        // Meghívjuk a játékos Shoot metódusát
        spaceship.GetComponent<Player>().Shoot();

        // Ellenőrizzük, hogy a lövedék létrejött
        GameObject projectile2 = GameObject.FindGameObjectWithTag("PlayerProjectile");
        Assert.IsNotNull(projectile2, "Lövedék nem jött létre");


        //jobb szélen
        for (int i = 0; i < 500; i++)
        {
            spaceship.GetComponent<Player>().Right();
            yield return null;
        }
        spaceship.GetComponent<Player>().Shoot();
        GameObject projectile3 = GameObject.FindGameObjectWithTag("PlayerProjectile");
        Assert.IsNotNull(projectile3, "Lövedék nem jött létre");


        //Várunk 500 képkockát, hogy biztos eltűnjön minden lövedék
        for (int i = 0; i < 500; i++)
        {
            yield return null;
        }

        //Léteznek még?
        Assert.IsTrue(projectile1 == null || projectile1.Equals(null), "Az első lövedék még létezik");
        Assert.IsTrue(projectile2 == null || projectile2.Equals(null), "A második lövedék még létezik");
        Assert.IsTrue(projectile3 == null || projectile3.Equals(null), "A harmadik lövedék még létezik");

        yield return null;
    }
}
