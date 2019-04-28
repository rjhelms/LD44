using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum GameState
{
    STARTING,
    RUNNING,
    WIN,
    LOSE,
    PAUSED,
    INVALID
}

public class GameController : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private Canvas canvasUI;

    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private Color fadeColor;

    private RawImage fadeCover;
    private float currentFadeTime;

    [SerializeField]
    private int cerealRemaining;

    public GameState State { get; private set; }




    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        State = GameState.STARTING;
        fadeCover = GameObject.Find("FadeCover").GetComponent<RawImage>();
        mainCamera = FindObjectOfType<Camera>();
        canvasUI = GameObject.Find("UICanvas").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case GameState.STARTING:
                currentFadeTime += Time.unscaledDeltaTime;
                fadeCover.color = Color.Lerp(fadeColor, Color.clear, currentFadeTime / fadeTime);
                if (currentFadeTime >= fadeTime)
                {
                    Time.timeScale = 1;
                    State = GameState.RUNNING;
                }
                break;
            case GameState.RUNNING:
                if (cerealRemaining == 0)
                {
                    Time.timeScale = 0;
                    currentFadeTime = 0;
                    State = GameState.WIN;
                }
                break;
            case GameState.WIN:
                currentFadeTime += Time.unscaledDeltaTime;
                fadeCover.color = Color.Lerp(Color.clear, fadeColor, currentFadeTime / fadeTime);
                if (currentFadeTime >= fadeTime)
                {
                    SceneManager.LoadScene("test");
                }
                break;
        }
    }

    void UpdateUI()
    {
        canvasUI.GetComponent<CanvasScaler>().scaleFactor = mainCamera.GetComponent<PixelPerfectCamera>().pixelRatio;
    }

    public void RegisterCereal(GameObject cereal)
    {
        cerealRemaining++;
    }

    public void RemoveCereal(GameObject cereal)
    {
        cerealRemaining--;
    }
}
