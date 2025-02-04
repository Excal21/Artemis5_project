using System.Collections.Generic;
using UnityEngine;

public class Level3Boss : MonoBehaviour
{
#region Level3Boss munkaváltozói
    private Vector3 targetPosition;
    private bool movingRight = true;
    private float stopTimer = 0f;

    private bool hasEnteredPlayArea = false;
    private float shootCycleTimer = 0f;
    private int shootCount = 0;
    private bool isShooting = true; // Jelzi, hogy lövési fázisban van-e
#endregion

#region Level3Boss beállításai
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject explosion;
    [SerializeField] private List<Sprite> shieldHitSprites;
    [SerializeField] private float projectileSpeed = 3f;
    [SerializeField] private float projectileOffset = -1f;
    [SerializeField] private float stopDuration = 2f; // Mennyi ideig áll meg
    [SerializeField] private float rightLimit = 5f; // Jobb oldali határ
    [SerializeField] private float leftLimit = -5f; // Bal oldali határ
    [SerializeField] private int health = 6;
#endregion


    void Start()
    {
        targetPosition = new Vector3(0, 4, 0);
    }

    void Update()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1f * Time.deltaTime);
        }
        else
        {
            HandleMovementPattern();
        }

        if (!hasEnteredPlayArea && transform.position.y <= Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane)).y - 0.5f)
        {
            hasEnteredPlayArea = true;
        }

        if (hasEnteredPlayArea)
        {
            HandleShooting();
        }
    }


#region Level3Boss metódusai
    void HandleShooting()
    {
        shootCycleTimer += Time.deltaTime;

        if (isShooting)
        {
            if (shootCycleTimer >= 0.33f && shootCount < 3) // Lövés 0,33 másodpercenként
            {
                Shoot();
                shootCount++;
                shootCycleTimer = 0f; // Újraindítjuk az időzítőt
            }

            if (shootCount == 3) // Ha 3 lövést leadott, álljunk meg
            {
                isShooting = false;
                shootCycleTimer = 0f;
            }
        }
        else
        {
            if (shootCycleTimer >= 1f) // Szünet 1 másodpercig
            {
                isShooting = true;
                shootCount = 0; // Újraindítjuk a lövési sorozatot
                shootCycleTimer = 0f;
            }
        }
    }

    public void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(-0.8f, projectileOffset), Quaternion.identity);
        projectile.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile.GetComponent<Projectile>().ProjectileVector = new Vector2(-0.7f, -1);
        projectile.transform.Rotate(0, 0, 150);

        projectile = Instantiate(projectilePrefab, transform.position + new Vector3(-1.2f, projectileOffset), Quaternion.identity);
        projectile.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile.GetComponent<Projectile>().ProjectileVector = new Vector2(-0.7f, -1);
        projectile.transform.Rotate(0, 0, 150);




        GameObject projectile2 = Instantiate(projectilePrefab, transform.position + new Vector3(-0.2f, projectileOffset + 0.3f), Quaternion.identity);
        projectile2.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile2.GetComponent<Projectile>().ProjectileVector = new Vector2(0f, -1);
        projectile2.transform.Rotate(0, 0, 180);

        projectile2 = Instantiate(projectilePrefab, transform.position + new Vector3(0.2f, projectileOffset + 0.3f), Quaternion.identity);
        projectile2.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile2.GetComponent<Projectile>().ProjectileVector = new Vector2(0f, -1);
        projectile2.transform.Rotate(0, 0, 180);




        GameObject projectile3 = Instantiate(projectilePrefab, transform.position + new Vector3(+1.2f, projectileOffset), Quaternion.identity);
        projectile3.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile3.GetComponent<Projectile>().ProjectileVector = new Vector2(0.7f, -1);
        projectile3.transform.Rotate(0, 0, 210);
        projectile3 = Instantiate(projectilePrefab, transform.position + new Vector3(+0.8f, projectileOffset), Quaternion.identity);
        projectile3.GetComponent<Projectile>().Speed = projectileSpeed;
        projectile3.GetComponent<Projectile>().ProjectileVector = new Vector2(0.7f, -1);
        projectile3.transform.Rotate(0, 0, 210);
    }

    void HandleMovementPattern()
    {
        if (stopTimer > 0)
        {
            stopTimer -= Time.deltaTime;
        }
        else
        {
            if (movingRight)
            {
                targetPosition = new Vector3(rightLimit, transform.position.y, 0);
                movingRight = false;
            }
            else
            {
                targetPosition = new Vector3(leftLimit, transform.position.y, 0);
                movingRight = true;
            }
            stopTimer = stopDuration;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerProjectile")
        {
            Destroy(other.gameObject);
            health--;
            if (health > 1)
            {
                GameObject explosionInstance = Instantiate(explosion, new Vector3(other.transform.position.x, transform.position.y, -2), Quaternion.identity);
                explosionInstance.GetComponent<Explosion>().Sprites = shieldHitSprites;
                explosionInstance.transform.localScale = new Vector3(8f, 8f);
            }
            else
            {
                GameObject explosionInstance = Instantiate(explosion, new Vector3(other.transform.position.x, transform.position.y, -2), Quaternion.identity);
                explosionInstance.transform.localScale = new Vector3(8f, 8f);
            }
            if (health == 1)
            {
                shield.SetActive(false);
            }
            if (health == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

#endregion