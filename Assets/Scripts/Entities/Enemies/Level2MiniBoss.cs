using System.Collections.Generic;
using UnityEngine;

public class Level2MiniBoss : MonoBehaviour
{
#region Level2MiniBoss munkaváltozói
    private Vector3 targetPosition;
    private bool movingRight = true;
    private float stopTimer = 0f;
    private float lastShotTime = 0;
    private bool hasEnteredPlayArea = false;
#endregion

#region Level2MiniBoss beállításai
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject explosion;

    [SerializeField] private List<Sprite> shieldHitSprites;

    [SerializeField] private float projectileSpeed = 2f;
    [SerializeField] private float projectileOffset = -1f;
    [SerializeField] private float fireRate = 3;

    [SerializeField] private float stopDuration = 2f; // Mennyi ideig áll meg
    [SerializeField] private float rightLimit = 5f; // Jobb oldali határ
    [SerializeField] private float leftLimit = -5f; // Bal oldali határ
    [SerializeField] private int health = 3;
#endregion

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

#region Level2Miniboss metódusai
    public void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(-1.2f, projectileOffset), Quaternion.identity);
        projectile.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile.GetComponent<Projectile>().ProjectileVector = new Vector2(-0.7f, -1);
        projectile.transform.Rotate(0, 0, 150);

        GameObject projectile2 = Instantiate(projectilePrefab, transform.position + new Vector3(-1.2f, projectileOffset), Quaternion.identity);
        projectile2.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile2.GetComponent<Projectile>().ProjectileVector = new Vector2(0.7f, -1);
        projectile2.transform.Rotate(0, 0, 210);

        GameObject projectile3 = Instantiate(projectilePrefab, transform.position + new Vector3(+1.2f, projectileOffset), Quaternion.identity);
        projectile3.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile3.GetComponent<Projectile>().ProjectileVector = new Vector2(-0.7f, -1);
        projectile3.transform.Rotate(0, 0, 150);

        GameObject projectile4 = Instantiate(projectilePrefab, transform.position + new Vector3(+1.2f, projectileOffset), Quaternion.identity);
        projectile4.GetComponent<Projectile>().Speed = projectileSpeed;
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
                GameObject explosionInstance = Instantiate(explosion, new Vector3(other.transform.position.x, transform.position.y, -2), Quaternion.identity);
                explosionInstance.GetComponent<Explosion>().Sprites = shieldHitSprites;
                explosionInstance.transform.localScale = new Vector3(8f, 8f);
            }
            else{
                GameObject explosionInstance = Instantiate(explosion, new Vector3(other.transform.position.x, transform.position.y, -2), Quaternion.identity);
                explosionInstance.transform.localScale = new Vector3(8f, 8f);
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
#endregion