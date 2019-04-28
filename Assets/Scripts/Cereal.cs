using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cereal : MonoBehaviour
{

    [SerializeField]
    private int valueLife = 3;
    [SerializeField]
    private int valueScore = 100;

    private void Start()
    {
        FindObjectOfType<GameController>().RegisterCereal(gameObject);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.gameObject.tag == "Player")
        {
            ScoreManager.Instance.Life += valueLife;
            ScoreManager.Instance.Score += valueScore;
            Destroy(gameObject);
            FindObjectOfType<GameController>().RemoveCereal(gameObject);
        }
    }
}
