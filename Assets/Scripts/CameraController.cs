using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    public Vector3 targetPosition = new Vector3(0,0,-10);
    [SerializeField]
    private Vector2 targetShift = new Vector2(8, 6);
    [SerializeField]
    private float targetLerpSpeed = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, targetLerpSpeed * Time.deltaTime);
    }

    public void ShiftCamera(Vector2 shift)
    {
        targetPosition += (Vector3)(shift * targetShift);
    }
}
