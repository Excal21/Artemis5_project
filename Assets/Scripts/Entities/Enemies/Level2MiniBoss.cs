using UnityEngine;

public class Level2MiniBoss : MonoBehaviour
{
    public GameObject shield;
    public GameObject projectilePrefab;
    public GameObject explosion;


    public float projectileSpeed = 2f;
    public float projectileOffset = -1f;
    public float fireRate = 3;

    public float speed = 5f; // Mozgás sebessége
    public float stopDuration = 2f; // Mennyi ideig áll meg
    public float rightLimit = 5f; // Jobb oldali határ
    public float leftLimit = -5f; // Bal oldali határ
    public int health = 3;

    private Vector3 targetPosition;
    private bool movingRight = true;
    private float stopTimer = 0f;
    private float lastShotTime = 0;
    private bool hasEnteredPlayArea = false;


    void Start()
    {
        // Cél az első mozgásra (lentebb a pályára)
        targetPosition = new Vector3(0, 4, 0);
    }

    void Update()
    {
        // Pajzs követi a minibosst
        //shield.transform.position = transform.position + new Vector3(0, -1, -1);

        // Mozgási logika
        if (transform.position != targetPosition)
        {
            // Mozgás a célpozíció felé
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1f * Time.deltaTime);
        }
        else
        {
            // Ha elérte a célját, kezeljük az ismétlődő mozgást
            HandleMovementPattern();
        }
        // Ellenőrizzük, hogy az ellenség belépett-e a játéktérre
        if (!hasEnteredPlayArea && transform.position.y <= Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane)).y-0.5f)
        {
            hasEnteredPlayArea = true;
        }


        if (hasEnteredPlayArea && Time.time - lastShotTime > 1 / fireRate)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }
    public void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(-1.2f, projectileOffset), Quaternion.identity);
        projectile.GetComponent<Projectile>().speed = projectileSpeed;
        projectile.GetComponent<Projectile>().ProjectileVector = new Vector2(-0.7f, -1);
        projectile.transform.Rotate(0, 0, 150);

        GameObject projectile2 = Instantiate(projectilePrefab, transform.position + new Vector3(-1.2f, projectileOffset), Quaternion.identity);
        projectile2.GetComponent<Projectile>().speed = projectileSpeed;
        projectile2.GetComponent<Projectile>().ProjectileVector = new Vector2(0.7f, -1);
        projectile2.transform.Rotate(0, 0, 210);

        GameObject projectile3 = Instantiate(projectilePrefab, transform.position + new Vector3(+1.2f, projectileOffset), Quaternion.identity);
        projectile3.GetComponent<Projectile>().speed = projectileSpeed;
        projectile3.GetComponent<Projectile>().ProjectileVector = new Vector2(-0.7f, -1);
        projectile3.transform.Rotate(0, 0, 150);

        GameObject projectile4 = Instantiate(projectilePrefab, transform.position + new Vector3(+1.2f, projectileOffset), Quaternion.identity);
        projectile4.GetComponent<Projectile>().speed = projectileSpeed;
        projectile4.GetComponent<Projectile>().ProjectileVector = new Vector2(0.7f, -1);
        projectile4.transform.Rotate(0, 0, 210);


    }

    void HandleMovementPattern()
    {
        if (stopTimer > 0)
        {
            // Várakozás a megállás során
            stopTimer -= Time.deltaTime;
        }
        else
        {
            // Ha nincs megállás, váltsunk mozgást
            if (movingRight)
            {
                targetPosition = new Vector3(rightLimit, transform.position.y, 0);
                movingRight = false; // Következő mozgás balra lesz
            }
            else
            {
                targetPosition = new Vector3(leftLimit, transform.position.y, 0);
                movingRight = true; // Következő mozgás jobbra lesz
            }

            // Állj meg a következő cél elérése után
            stopTimer = stopDuration;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerProjectile")
        {
            // Ha a játékos lövése érinti a minibosst
            Destroy(other.gameObject); // Töröljük a lövedéket
            health--; // Csökkentsük az életerejét
            if(health > 1){

                Instantiate(explosion, transform.position + new Vector3(0f, -0.5f, 0), Quaternion.identity); // Részecskerendszer az animációhoz
            }
            else{
                Instantiate(explosion, transform.position, Quaternion.identity); // Részecskerendszer az animációhoz
            }
            if(health == 1){

                shield.SetActive(false);

            }
            if(health == 0){
                Destroy(gameObject); // Ha elfogyott az életereje, töröljük a minibosst
            }
        }
    }

}
