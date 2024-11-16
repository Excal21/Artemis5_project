using System.Collections.Generic;
using UnityEngine;
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
    float dx, dy;
    #endregion

    #region Tulajdonságok private mezői
    [SerializeField]
    private List<Sprite> playerSprites;
    [SerializeField]
    private int health = 4;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float gravity = 0.2f;
    [SerializeField]
    private GameObject jetProp;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float jetPropOffset = 0.1f;
    [SerializeField]
    private float fireRate = 3;
    [SerializeField]
    private float projectileOffset = 1f;
    [SerializeField]
    private float projectileSpeed = 5f;
    [SerializeField]
    private bool controllable = true;
    [SerializeField]
    private Rigidbody2D rb;
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

    public void Up()
    {
        actPosition = transform.position;
        newPosition += Vector2.up * Time.deltaTime * speed;
        if (newPosition.y <= screenTop)
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
        else
        {
            newPosition = actPosition;
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
    public void Start()
    {
        prevYcord = transform.position.y;

        //Képernyő szélének meghatározása
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        screenTop = screenBounds.y;
        screenBottom = -screenBounds.y + 0.5f;
        screenLeft = -screenBounds.x;
        screenRight = screenBounds.x;

        transform.position = new Vector3(0, -4.5f, 0);
    }

    public void FixedUpdate(){
    rb.linearVelocity = new Vector2(dx, dy);
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
        }
        else
        {
            newPosition = actPosition;
        }


        if (newPosition.y >= screenBottom) this.transform.position = newPosition;


        Vector3 tilt = Input.acceleration;
        dx = Input.acceleration.x * speed;
        dy = (Input.acceleration.y + 0.7f) * speed;

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



        if (Input.GetKey(KeyCode.Space) || Input.touchCount > 0 && Time.time > lastShotTime + 1 / fireRate)
        {
            Shoot();
            lastShotTime = Time.time;
        }



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
        projectile.GetComponent<Projectile>().speed = projectileSpeed;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Destroy(collision.gameObject);
        }
        if (health == 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Damage();

        }
    }
}
