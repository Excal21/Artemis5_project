using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Lövedék beállításai
    [SerializeField] private Vector2 projectileVector = new Vector2(0, 1);
    [SerializeField] private float speed = 0.5f;
    #endregion
    private Rigidbody2D rb;

    public Vector2 ProjectileVector { get => projectileVector; set => projectileVector = value; }
    public float Speed { get => speed; set => speed = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioHandler.instance.PlayShootSound();
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = ProjectileVector * Speed;
        }
        //Ha el kell forgatni 90 fokkal később
        //transform.Rotate(0, 0, 90);
        //Ha el kell forgatni 90 fokkal később
        //transform.Rotate(0, 0, 90);
    }

    // Update is called once per frame
    // void Update()
    // {
    //     this.transform.position += new Vector3(ProjectileVector.x, ProjectileVector.y, 0) * speed * Time.deltaTime;
    // }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Projectile" && collision.name != "Level2MiniBoss(Clone)" && collision.name != "Boss(Clone)"){
           Destroy(this.gameObject);
        }
    }

}
