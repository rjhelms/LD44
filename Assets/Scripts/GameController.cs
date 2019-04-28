using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private Canvas canvasUI;

    [SerializeField]
    private int CerealRemaining;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        canvasUI = GameObject.Find("UICanvas").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateUI()
    {
        canvasUI.GetComponent<CanvasScaler>().scaleFactor = mainCamera.GetComponent<PixelPerfectCamera>().pixelRatio;
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
