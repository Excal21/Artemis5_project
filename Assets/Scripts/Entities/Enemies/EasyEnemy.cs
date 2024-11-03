using System.Collections.Generic;
using UnityEngine;

public class EasyEnemy : MonoBehaviour
{
    #region Egyszerű ellenfél munkaváltozói
    public float speed = 2.0f;
    private int state = 0;
    private float distanceMoved = 0.0f;
    private float lastShotTime = 0;
    private bool hasEnteredPlayArea = false;
    #endregion

    #region Tulajdonságok privát mezői
    //A távolságok egységben unity képernyőegységben értendők
    [SerializeField]
    private float verticalMoveDistance = 1.0f;
    [SerializeField]
    private float horizontalMoveDistance = 2.0f;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float projectileSpeed = 5.0f;
    [SerializeField]
    private float fireRate = 2.0f;
    [SerializeField]
    private Vector3 projectileDirection = Vector2.down;
    [SerializeField]
    private float projectileOffset = -1f;
    [SerializeField]
    private List<Sprite> enemySprites = new List<Sprite>();
    #endregion

    #region Getterek/Setterek
    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public List<Sprite> EnemySprites { get => enemySprites; set => enemySprites = value; }
    #endregion

    //A start akkor fut le, mikor a Monobehaviour létrejön, az első Update előtt
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = enemySprites[0];
    }

    //Képkockánként egyszer fut le
    void Update()
    {
        float moveStep = speed * Time.deltaTime;
        switch (state)
        {
            case 0: //Le
                GetComponent<SpriteRenderer>().sprite = enemySprites[0];
                transform.Translate(Vector3.down * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= verticalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 1;
                }
                break;
            case 1: //Balra
                GetComponent<SpriteRenderer>().sprite = enemySprites[1];

                transform.Translate(Vector3.left * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= horizontalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 2;
                }
                break;
            case 2: //Megint le
                GetComponent<SpriteRenderer>().sprite = enemySprites[0];

                transform.Translate(Vector3.down * moveStep);
                distanceMoved += moveStep;
                if (distanceMoved >= verticalMoveDistance)
                {
                    distanceMoved = 0.0f;
                    state = 3;
                }
                break;
            case 3: //Jobbra
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
        if (hasEnteredPlayArea && other.tag == "PlayerProjectile" || other.tag == "Border")
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
    }

}
