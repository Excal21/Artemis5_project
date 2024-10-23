using UnityEditorInternal;
using UnityEngine;

public class DuoFighters : MonoBehaviour
{
    public GameObject fighter1;
    public GameObject fighter2;

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

    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (fighter1 == null && fighter2 == null) Destroy(this.gameObject);
        else{
        float moveStep = speed * Time.deltaTime;
        switch (state)
        {
            case 0: // Move down
                if (fighter1 != null) fighter1.transform.Translate(Vector3.down * moveStep);
                if (fighter2 != null) fighter2.transform.Translate(Vector3.down * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= verticalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 1;
                }
                break;
            case 1: // Move left or right based on Player position
                float fighterX;
                if (fighter1 != null && fighter2 != null) fighterX = fighter1.transform.position.x + fighter2.transform.position.x / 2;
                else if (fighter1 != null) fighterX = fighter1.transform.position.x;
                else fighterX = fighter2.transform.position.x;

                if (player.transform.position.x < fighterX)
                {
                    if (fighter1 != null) fighter1.transform.Translate(Vector3.left * moveStep);
                    if (fighter2 != null) fighter2.transform.Translate(Vector3.left * moveStep);
                }
                else
                {
                    if (fighter1 != null) fighter1.transform.Translate(Vector3.right * moveStep);
                    if (fighter2 != null) fighter2.transform.Translate(Vector3.right * moveStep);
                }

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
            
            if (fighter1 != null) Shoot(fighter1);
            if (fighter2 != null) Shoot(fighter2);
            lastShotTime = Time.time;
        }

        }

       
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile" || other.tag == "Border")
        {
            Destroy(this.gameObject);
        }
    }
    public void Shoot(GameObject fighter)
    {
        GameObject projectile = Instantiate(projectilePrefab, fighter.transform.position + new Vector3(0, projectileOffset), Quaternion.identity);
        projectile.GetComponent<Projectile>().ProjectileVector = projectileDirection;
        projectile.GetComponent<Projectile>().speed = projectileSpeed;
        //projectile.GetComponent<PlayerProjectile>().ProjectileVector = new Vector2(0, 1);
    }
}
