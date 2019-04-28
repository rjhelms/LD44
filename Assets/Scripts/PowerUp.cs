using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private int valueMans;
    [SerializeField]
    private int valueLife;
    [SerializeField]
    private int valueScore;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.gameObject.tag == "Player")
        {
            ScoreManager.Instance.Life += valueLife;
            ScoreManager.Instance.Mans += valueMans;
            ScoreManager.Instance.Score += valueScore;
            Destroy(gameObject);
        }
    }
}
