using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 moveVector;
    public GameObject parent;

    [SerializeField]
    private int moveSpeed;

    [SerializeField]
    private int damage;

    [SerializeField]
    string targetTag;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = moveVector * moveSpeed;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == parent)
        {
            return;
        }
        else if (collision.gameObject.tag == targetTag)
        {
            Debug.Log("Hit " + collision.gameObject);
            collision.gameObject.GetComponentInParent<BaseActor>().Hit(damage);
            Destroy(gameObject);
        }
    }
}
