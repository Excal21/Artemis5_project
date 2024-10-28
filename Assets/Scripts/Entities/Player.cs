using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = 0.2f;
    public GameObject JetProp;
    public GameObject ProjectilePrefab;
    public float FireRate = 3;
    public float ProjectileOffset = 1f;
    private float prevYcord;
    private float screenTop;
    private float screenBottom;
    private float screenLeft;
    private float screenRight;
    private float lastShotTime = 0;
    public Sprite designersprite;
    private Vector2 newPosition;
    private Vector2 actPosition;

    public void Shoot()
    {
        GameObject projectile = Instantiate(ProjectilePrefab, new Vector3(this.transform.position.x, this.transform.position.y + ProjectileOffset, 0), Quaternion.identity);

        //projectile.GetComponent<PlayerProjectile>().ProjectileVector = new Vector2(0, 1);
    }

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

    public void Start()
    {
        prevYcord = transform.position.y;

        // Határozzuk meg a képernyő széleit
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
        if (newPosition.y >= screenBottom){
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
        else{
            newPosition = actPosition;
        }


        if (newPosition.y >= screenBottom) this.transform.position = newPosition;

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
            JetProp.transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);
            JetProp.SetActive(true);
        }
        else
        {
            JetProp.SetActive(false);
        }
        prevYcord = transform.position.y;



        if (Input.GetKey(KeyCode.Space) && Time.time > lastShotTime + 1 / FireRate)
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
