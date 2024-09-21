using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public float ScreenOffset = 0.5f;
    public float speed = 3f;
    public float gravity = 1.5f;
    public GameObject JetProp;
    public GameObject ProjectilePrefab;
    public float FireRate = 3;
    public float ProjectileSpeed = 0.1f;
    public float ProjectileOffset = 1f;



    private float prevYcord;
    private float screenTop;
    private float screenBottom;
    private float screenLeft;
    private float screenRight;
    private float lastShotTime = 0;

    public void Shoot()
    {
        GameObject projectile = Instantiate(ProjectilePrefab, new Vector3(this.transform.position.x, this.transform.position.y + ProjectileOffset, 0), Quaternion.identity);
        projectile.GetComponent<Projectile>().speed = ProjectileSpeed;
        //projectile.GetComponent<PlayerProjectile>().ProjectileVector = new Vector2(0, 1);
    }

    public void Awake()
    {
        prevYcord = transform.position.y;

        // Határozzuk meg a képernyő széleit
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        screenTop = screenBounds.y;
        screenBottom = -screenBounds.y;
        screenLeft = -screenBounds.x;
        screenRight = screenBounds.x;
    }

    public void Update()
    {
        Vector2 newPosition = transform.position;
    
        // Adjuk hozzá a gravitáció hatását
        newPosition.y -= gravity * Time.deltaTime;
    
        //Mozgás
        if (Input.GetKey(KeyCode.W))
        {
            newPosition += Vector2.up * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition += Vector2.down * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition += Vector2.left * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition += Vector2.right * Time.deltaTime * speed;
        }

        // Lövés
        if (Input.GetKey(KeyCode.Space) && Time.time - lastShotTime > 1 / FireRate)
        {
            Shoot();
            lastShotTime = Time.time;
        }


    
        // Korlátozzuk a karakter pozícióját a képernyő széleinél
        newPosition.x = Mathf.Clamp(newPosition.x, screenLeft + ScreenOffset, screenRight - ScreenOffset);
        newPosition.y = Mathf.Clamp(newPosition.y, screenBottom + ScreenOffset, screenTop - ScreenOffset);
    
        // Alkalmazzuk a korlátozott pozíciót
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    
        // Kis lángcsóva megjelenítése, ha az űrhajó y koordinátája pozitívan változik
        if (prevYcord < transform.position.y)
        {
            JetProp.transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);
            JetProp.SetActive(true);
        }
        else
        {
            JetProp.SetActive(false);
        }
    
        // Frissítsük a prevYcord értékét
        prevYcord = transform.position.y;
    }
}