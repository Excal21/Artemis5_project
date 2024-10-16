using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SpaceshipMotion
{
// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    private float TopBorder;
    private float BottomBorder;
    private float LeftBorder;
    private float RBorder;
    
    [UnityTest]
    public IEnumerator MotionTestWithEnumeratorPasses()
    {
        SceneManager.LoadScene("Level1");
        yield return new WaitForSeconds(1); // Wait for the scene to load

        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        TopBorder = screenBounds.y;
        BottomBorder = -screenBounds.y;
        LeftBorder = -screenBounds.x;
        RBorder = screenBounds.x;



        Debug.Log("Scene loaded.");
        // Find the PlayerSpaceship object by tag
        GameObject spaceship = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(spaceship, "Űrhajó nem található");

        // Get the screen bounds
        float RightBorder = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        spaceship.GetComponent<PlayerMovement>().speed = 24f;


        spaceship.GetComponent<PlayerMovement>().gravity = 10f;
        Assert.IsTrue(spaceship.transform.position.y >= BottomBorder, "A hajó kiment a képernyő aljáról.");

        for (int i = 0; i < 500; i++) // Simulate 100 frames
        {
            yield return null;
        }

        //spaceship.GetComponent<PlayerMovement>().gravity = 0f;


        for (int i = 0; i < 500; i++) // Simulate 100 frames
        {
            spaceship.GetComponent<PlayerMovement>().Right();
            yield return null; // Skip a frame
        }
        Assert.IsTrue(spaceship.transform.position.x <= RightBorder, "A hajó kiment a képernyő jobb széléről.");



        for (int i = 0; i < 1000; i++) // Simulate 100 frames
        {
            spaceship.GetComponent<PlayerMovement>().Left();
            yield return null; // Skip a frame
        }
        Assert.IsTrue(spaceship.transform.position.x >= LeftBorder, "A hajó kiment a képernyő bal széléről.");
        

        for (int i = 0; i < 500; i++) // Simulate 100 frames
        {
            spaceship.GetComponent<PlayerMovement>().Up();
            yield return null; // Skip a frame
        }
        Assert.IsTrue(spaceship.transform.position.y <= TopBorder, "A hajó kiment a képernyő tetejéről.");


        for (int i = 0; i < 500; i++) // Simulate 100 frames
        {
            spaceship.GetComponent<PlayerMovement>().Down();
            yield return null; // Skip a frame
        }
        Assert.IsTrue(spaceship.transform.position.y >= BottomBorder, "A hajó kiment a képernyő aljáról.");

    }
}
