using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private int CerealRemaining;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterCereal(GameObject cereal)
    {
        CerealRemaining++;
    }

    public void RemoveCereal(GameObject cereal)
    {
        CerealRemaining--;
    }
}
