using System;
using System.Collections.Generic;
using UnityEngine;

public class DuoFighters : MonoBehaviour
{
    #region DuoFighters munkaváltozói
    private GameObject player;
    private float lastShotTime = 0;
    private float distanceMoved = 0.0f;
    private int state = 0;
    private bool hasEnteredPlayArea = false;
    #endregion

    #region Tulajdonságok privát mezői
    [SerializeField]
    private GameObject fighter1;
    [SerializeField]
    private GameObject fighter2;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Vector3 projectileDirection = Vector2.down;
    [SerializeField]
    private float speed = 2.0f;
    [SerializeField]
    private float verticalMoveDistance = 1.0f; // Függőleges távolság
    [SerializeField]
    private float horizontalMoveDistance = 2.0f; // Horizontális távolság
    [SerializeField]
    private float projectileSpeed = 5.0f;
    [SerializeField]
    private float fireRate = 2.0f;
    [SerializeField]
    private float projectileOffset = -1f;
    #endregion


    #region Getterek/Setterek
    public List<Sprite> enemySprites = new List<Sprite>(); //Sima, balra, jobbra manőverező
    public GameObject Fighter1 { get => fighter1; set => fighter1 = value; }
    public GameObject Fighter2 { get => fighter2; set => fighter2 = value; }
    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    public Vector3 ProjectileDirection { get => projectileDirection; set => projectileDirection = value; }
    public float Speed { get => speed; set => speed = value; }
    public float VerticalMoveDistance { get => verticalMoveDistance; set => verticalMoveDistance = value; }
    public float HorizontalMoveDistance { get => horizontalMoveDistance; set => horizontalMoveDistance = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public float ProjectileOffset { get => projectileOffset; set => projectileOffset = value; }
    #endregion

    //A start akkor fut le, mikor a Monobehaviour létrejön, az első Update előtt
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    //Képkockánként egyszer fut le
    void Update()
    {
        if (fighter1 == null && fighter2 == null) Destroy(this.gameObject);
        else
        {
            float moveStep = speed * Time.deltaTime;
            switch (state)
            {
                case 0: // Mozgás lefelé
                    SpriteChange(0);
                    if (fighter1 != null) fighter1.transform.Translate(Vector3.down * moveStep);
                    if (fighter2 != null) fighter2.transform.Translate(Vector3.down * moveStep);
                    distanceMoved += moveStep;
                    if (distanceMoved >= verticalMoveDistance)
                    {
                        distanceMoved = 0.0f;
                        float fighterX;
                        if (fighter1 != null && fighter2 != null) fighterX = (fighter1.transform.position.x + fighter2.transform.position.x) / 2;
                        else if (fighter1 != null) fighterX = fighter1.transform.position.x;
                        else fighterX = fighter2.transform.position.x;

                        if (player != null && fighterX > player.transform.position.x) state = 1;
                        else state = 2;
                    }
                    break;
                case 1: // Mozgás balra
                    SpriteChange(1);

                    if (fighter1 != null) fighter1.transform.Translate(Vector3.left * moveStep);
                    if (fighter2 != null) fighter2.transform.Translate(Vector3.left * moveStep);
                    distanceMoved += moveStep;
                    if (distanceMoved >= horizontalMoveDistance)
                    {
                        distanceMoved = 0.0f;
                        state = 0;
                    }
                    break;
                case 2: // Mozgás jobbra
                    SpriteChange(2);

                    if (fighter1 != null) fighter1.transform.Translate(Vector3.right * moveStep);
                    if (fighter2 != null) fighter2.transform.Translate(Vector3.right * moveStep);
                    distanceMoved += moveStep;
                    if (distanceMoved >= horizontalMoveDistance)
                    {
                        distanceMoved = 0.0f;
                        state = 0;
                    }
                    break;

            }

            // Ellenőrizzük, hogy az ellenség belépett-e a játéktérre
            if (!hasEnteredPlayArea && fighter1.transform.position.y <= Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane)).y-0.5f)
            {
                hasEnteredPlayArea = true;
            }

            if (hasEnteredPlayArea && Time.time - lastShotTime > 1 / fireRate)
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
        projectile.transform.Rotate(0, 0, 180);
    }

    private void SpriteChange(int spriteIndex)
    {
        if (fighter1 != null) fighter1.GetComponent<SpriteRenderer>().sprite = enemySprites[spriteIndex];
        if (fighter2 != null) fighter2.GetComponent<SpriteRenderer>().sprite = enemySprites[spriteIndex];
    }

}
