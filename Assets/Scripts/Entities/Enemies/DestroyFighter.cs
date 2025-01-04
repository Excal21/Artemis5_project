using UnityEngine;

public class DestroyChild : MonoBehaviour
{
    public GameObject explosion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.parent.GetComponent<DuoFighters>().HasEnteredPlayArea && other.tag == "PlayerProjectile" || other.tag == "Border")
        {
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
