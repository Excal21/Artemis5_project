using System.Collections.Generic;
using UnityEngine;

public class EasyEnemy : MonoBehaviour
{
    public float speed = 2.0f;
    private int state = 0; // 0: down, 1: left, 2: down, 3: right
    private float distanceMoved = 0.0f;
    public float verticalMoveDistance = 1.0f; // Distance to move down
    public float horizontalMoveDistance = 2.0f; // Distance to move left/right

    public GameObject projectilePrefab;
    public float projectileSpeed = 5.0f;
    public float fireRate = 2.0f;
    public Vector3 projectileDirection = Vector2.down;
    public float projectileOffset = -1f;
    private float lastShotTime = 0;
    private bool hasEnteredPlayArea = false;
    public List<Sprite> enemySprites = new List<Sprite>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = enemySprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        float moveStep = speed * Time.deltaTime;
        switch (state)
        {
            case 0: // Move down
                GetComponent<SpriteRenderer>().sprite = enemySprites[0];
                transform.Translate(Vector3.down * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= verticalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 1;
                }
                break;
            case 1: // Move left
                GetComponent<SpriteRenderer>().sprite = enemySprites[1];

                transform.Translate(Vector3.left * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= horizontalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 2;
                }
                break;
            case 2: // Move down
                GetComponent<SpriteRenderer>().sprite = enemySprites[0];

                transform.Translate(Vector3.down * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= verticalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 3;
                }
                break;
            case 3: // Move right
                GetComponent<SpriteRenderer>().sprite = enemySprites[2];

                transform.Translate(Vector3.right * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= horizontalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 0;
                }
                break;
        }

        // Ellenőrizzük, hogy az ellenség belépett-e a játéktérre
        if (!hasEnteredPlayArea && transform.position.y <= Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane)).y-0.5f)
        {
            hasEnteredPlayArea = true;
        }

        // Csak akkor lőjön, ha belépett a játéktérre
        if (hasEnteredPlayArea && Time.time - lastShotTime > 1 / fireRate)
        {
            Shoot();
            lastShotTime = Time.time;
        }


    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile" || other.tag == "Border")
        {
            Destroy(this.gameObject);
        }
    }
    public void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, projectileOffset), Quaternion.identity);
        projectile.GetComponent<Projectile>().ProjectileVector = projectileDirection;
        projectile.GetComponent<Projectile>().speed = projectileSpeed;
        projectile.transform.Rotate(0, 0, 180);
        //projectile.GetComponent<PlayerProjectile>().ProjectileVector = new Vector2(0, 1);
    }

}
