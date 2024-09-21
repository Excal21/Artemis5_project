using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public Vector2 ProjectileVector = new Vector2(0, 1);
    public float speed = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Ha el kell forgatni 90 fokkal később
        //transform.Rotate(0, 0, 90);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3(ProjectileVector.x, ProjectileVector.y, 0) * speed;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(this.gameObject);
    }

}
