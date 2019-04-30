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

    private bool found = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (!found && collision.gameObject.tag == "Player")
        {
            found = true;
            ScoreManager.Instance.AddLife(valueLife);
            ScoreManager.Instance.Mans += valueMans;
            ScoreManager.Instance.Score += valueScore;
            GameController controller = GameObject.Find("GameController").GetComponent<GameController>();
            controller.PlaySound(controller.PowerupSound);
            Destroy(gameObject);
        }
    }
}
