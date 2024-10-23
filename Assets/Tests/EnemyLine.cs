using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class EnemyLine
{

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator EnemyLineWithEnumeratorPasses()
    {
         // Betöltjük az EntityTest jelenetet
        SceneManager.LoadScene("EntityTest");
        yield return new WaitForSeconds(1);
        // Betöltjük az EasyEnemy prefabot a Resources mappából
        GameObject easyEnemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EasyEnemyBase.prefab");
        Assert.IsNotNull(easyEnemyPrefab, "A teszt nem találta az ellenség prefabot");
        
        //Növeljük a sebességét, hogy a teszt gyorsan lefusson

        // Véletlenszerű pozíció generálása a képernyőn belül
        Vector3 randomPosition = new Vector3(
            Random.Range(-Screen.width / 2, Screen.width / 2),
            Random.Range(-Screen.height / 2, Screen.height / 2),
            0
        );

        // Az EasyEnemy példányosítása a véletlenszerű pozícióban
        GameObject easyEnemy = Object.Instantiate(easyEnemyPrefab, new Vector3(0 , 4, 0), Quaternion.identity);
        GameObject easyEnemy2 = Object.Instantiate(easyEnemyPrefab, new Vector3(-8.5f, 4.5f), Quaternion.identity);
        GameObject easyEnemy3 = Object.Instantiate(easyEnemyPrefab, new Vector3(8.5f, 4.5f), Quaternion.identity);
        easyEnemy.GetComponent<EasyEnemy>().speed = 20;
        easyEnemy2.GetComponent<EasyEnemy>().speed = 20;
        easyEnemy3.GetComponent<EasyEnemy>().speed = 20;
        

        easyEnemy.GetComponent<EasyEnemy>().fireRate = 0;
        easyEnemy2.GetComponent<EasyEnemy>().fireRate = 0;
        easyEnemy3.GetComponent<EasyEnemy>().fireRate = 0;        


        Assert.IsNotNull(easyEnemy, "Failed to instantiate EasyEnemy");
        Assert.IsNotNull(easyEnemy2, "Failed to instantiate EasyEnemy2");
        Assert.IsNotNull(easyEnemy3, "Failed to instantiate EasyEnemy3");

        // Várunk 500 képkockát
        for (int i = 0; i < 2500; i++)
        {
            yield return null;
        }

        // Ellenőrizzük, hogy az EasyEnemy elpusztult-e
        Assert.IsTrue(easyEnemy == null || easyEnemy.Equals(null), "EasyEnemy is still alive after 2500 frames");
        Assert.IsTrue(easyEnemy2 == null || easyEnemy2.Equals(null), "EasyEnemy2 is still alive after 2500 frames");
        Assert.IsTrue(easyEnemy3 == null || easyEnemy3.Equals(null), "EasyEnemy3 is still alive after 2500 frames");
    }
}
