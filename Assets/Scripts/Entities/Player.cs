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
    #endregion

    #region Tulajdonságok private mezői
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float gravity = 0.2f;
    [SerializeField]
    private GameObject jetProp;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float fireRate = 3;
    [SerializeField]
    private float projectileOffset = 1f;
    [SerializeField]
    private Sprite designersprite;
    #endregion

    #region Getterek/Setterek
    public float Speed { get => speed; set => speed = value; }
    public float Gravity { get => gravity; set => gravity = value; }
    public GameObject JetProp { get => jetProp; set => jetProp = value; }
    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public float ProjectileOffset { get => projectileOffset; set => projectileOffset = value; }
    #endregion

    public void Shoot() //Lövésért felelős metódus
    {
        GameObject projectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x, this.transform.position.y + projectileOffset, 0), Quaternion.identity);
        projectile.tag = "PlayerProjectile";
    }

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
        screenBottom = -screenBounds.y;
        screenLeft = -screenBounds.x;
        screenRight = screenBounds.x;

        transform.position = new Vector3(0, -4.5f, 0);
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
            Debug.Log("D");
            Right();
        }


        //Jet Propulsion
        if (prevYcord < transform.position.y)
        {
            jetProp.transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);
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
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.B))
        {
            GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite = designersprite;
        }

    }
}
