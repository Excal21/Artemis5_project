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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveStep = speed * Time.deltaTime;
        switch (state)
        {
            case 0: // Move down
                transform.Translate(Vector3.down * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= verticalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 1;
                }
                break;
            case 1: // Move left
                transform.Translate(Vector3.left * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= horizontalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 2;
                }
                break;
            case 2: // Move down
                transform.Translate(Vector3.down * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= verticalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 3;
                }
                break;
            case 3: // Move right
                transform.Translate(Vector3.right * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= horizontalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 0;
                }
                break;
        }
        if (Time.time - lastShotTime > 1 / fireRate)
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
        //projectile.GetComponent<PlayerProjectile>().ProjectileVector = new Vector2(0, 1);
    }

}
