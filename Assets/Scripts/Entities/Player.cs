using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    #region Képernyőhatárok munkaváltozói
    private float screenTop;
    private float screenBottom;
    private float screenLeft;
    private float screenRight;
    #endregion

    #region Játékos munkaváltozói
    private float prevYcord;
    private float lastShotTime = 0;
    private Vector2 newPosition;
    private Vector2 actPosition;
    private bool invincible = false;

    #endregion

    #region Tulajdonságok private mezői
    [SerializeField] private List<Sprite> playerSprites;
    [SerializeField] private int health = 4;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = 0.2f;
    [SerializeField] private GameObject jetProp;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject explosion;
    [SerializeField] private float jetPropOffset = 0.1f;
    [SerializeField] private float fireRate = 3;
    [SerializeField] private float projectileOffset = 1f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private bool controllable = true;
    #endregion

    #region Getterek/Setterek
    public float Speed { get => speed; set => speed = value; }
    public float Gravity { get => gravity; set => gravity = value; }
    public GameObject JetProp { get => jetProp; set => jetProp = value; }
    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public float ProjectileOffset { get => projectileOffset; set => projectileOffset = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public List<Sprite> PlayerSprites { get => playerSprites; set => playerSprites = value; }
    public int Health { get => health; }
    public bool Controllable { set => controllable = value; }
    public bool Invincible { get => invincible; set => invincible = value; }
    #endregion

    #region Mozgások metódusai
    //Gombok megnyomásakor ezek a metódusok hívódnak meg az Update()-ben és ezeket hívják meg a tesztek is
    public void Left()
    {
        actPosition = transform.position;
        newPosition += Vector2.left * Time.deltaTime * speed;
        if (newPosition.x >= screenLeft)
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
        else
        {
            newPosition = actPosition;
        }
    }

    public void Right()
    {
        actPosition = transform.position;
        newPosition += Vector2.right * Time.deltaTime * speed;
        if (newPosition.x <= screenRight)
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
        else
        {
            newPosition = actPosition;
        }
    }

    public void Up(bool overridable = false)
    {
        actPosition = transform.position;
        newPosition += Vector2.up * Time.deltaTime * speed;
        if (overridable)
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
        else
        {

            if (newPosition.y <= screenTop)
            {
                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            }
            else
            {
                newPosition = actPosition;
            }
        }
    }

    public void Down()
    {
        actPosition = transform.position;
        newPosition += Vector2.down * Time.deltaTime * speed;
        if (newPosition.y >= screenBottom)
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
        else
        {
            newPosition = actPosition;
        }
    }
    #endregion

    #region Események kezelői
    public void DeathScreen()
    {
        AudioHandler.instance.StopMusic();
          GameObject.Find("HandleNavigation").GetComponent<HandleNavigation>().IsPlayerDeadOrCleared = true;

        GameObject pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").transform.Find("Canvas - Pause Menu").gameObject;
        
        GameObject.Find("HandleNavigation").GetComponent<HandleNavigation>().isGamePaused = true;
        
        pauseMenu.transform.Find("Image - Pause Menu Background").gameObject.SetActive(true);
        pauseMenu.transform.Find("Panel - YOU DIED").gameObject.SetActive(true);
    }

    private IEnumerator MoveToStartPosition(float targetY)
    {
        
        controllable = false;
        float gravity = this.gravity;
        this.gravity = 0;
        invincible = true;
        while(transform.position.y < targetY){
            transform.position += new Vector3(0, 0.01f, 0);
            yield return new WaitForSeconds(0.005f);
        }
        invincible = false;
        this.gravity = gravity/2;
        yield return new WaitForSeconds(1);
        this.gravity = gravity;
        controllable = true;
        GameObject.FindWithTag("HealthIndicator").GetComponent<HealtIndicator>().Show();
    }


    #endregion
    public void Start()
    {
        prevYcord = transform.position.y;


        //Képernyő szélének meghatározása
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        screenTop = screenBounds.y;
        screenBottom = -screenBounds.y + 0.5f;
        screenLeft = -screenBounds.x;
        screenRight = screenBounds.x;

        //transform.position = new Vector3(0, -6f, 0);
        StartCoroutine(MoveToStartPosition(screenBottom + 0.5f));
    }

    public void Update()
    {

        // Adjuk hozzá a gravitáció hatását
        actPosition = transform.position;
        newPosition = transform.position;
        newPosition += Vector2.down * Time.deltaTime * gravity;
        if (newPosition.y >= screenBottom)
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            this.transform.position = newPosition;
        }
        else
        {
            newPosition = actPosition;
        }


        if (controllable)
        {


            //Inputok lekezelése
            if (Input.GetKey(KeyCode.W))
            {
                Up();
            }
            if (Input.GetKey(KeyCode.S))
            {
                Down();
            }
            if (Input.GetKey(KeyCode.A))
            {
                Left();
            }
            if (Input.GetKey(KeyCode.D))
            {
                Right();
            }

        }
        //Jet Propulsion
        if (prevYcord < transform.position.y)
        {
            jetProp.transform.position = new Vector3(transform.position.x, transform.position.y - jetPropOffset, 1);
            jetProp.SetActive(true);
        }
        else
        {
            jetProp.SetActive(false);
        }
        prevYcord = transform.position.y;



        if (Input.GetKey(KeyCode.Space) && Time.time > lastShotTime + 1 / fireRate)
        {
            Shoot();
            lastShotTime = Time.time;
        }

        /*
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        */


    }



    public void Damage()
    {
        health--;
        this.GetComponent<SpriteRenderer>().sprite = playerSprites[3 - health];
        GameObject.FindGameObjectWithTag("HealthIndicator").GetComponent<HealtIndicator>().UpdateHealth(health);
    }


    public void Shoot() //Lövésért felelős metódus
    {
        GameObject projectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x, this.transform.position.y + projectileOffset, 0), Quaternion.identity);
        projectile.tag = "PlayerProjectile";
        projectile.GetComponent<Projectile>().Speed = projectileSpeed;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!invincible)
        {
            GameObject explosionInstance = Instantiate(explosion, this.transform.position, Quaternion.identity);

            if (collision != null)
            {
                Destroy(collision.gameObject);
            }
            if (health == 0)
            {

                Destroy(this.gameObject);
                DeathScreen();

            }
            else
            {
                Damage();
            }
        }
        
    }
}