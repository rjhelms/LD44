using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum ScreenState
{
    IN,
    RUN,
    OUT
}

public class ScreenController : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    protected Canvas canvasUI;
    [SerializeField]
    protected string nextScene;
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private Color fadeColor;

    public ScreenState State;
    private RawImage fadeCover;
    private float currentFadeTime;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        canvasUI = GameObject.Find("UICanvas").GetComponent<Canvas>();
        canvasUI.GetComponent<CanvasScaler>().scaleFactor = mainCamera.GetComponent<PixelPerfectCamera>().pixelRatio * 2;
        fadeCover = GameObject.Find("FadeCover").GetComponent<RawImage>();
        fadeCover.color = fadeColor;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        canvasUI.GetComponent<CanvasScaler>().scaleFactor = mainCamera.GetComponent<PixelPerfectCamera>().pixelRatio * 2;
        switch (State)
        {
            case ScreenState.IN:
                currentFadeTime += Time.deltaTime;
                fadeCover.color = Color.Lerp(fadeColor, Color.clear, currentFadeTime / fadeTime);
                if (currentFadeTime >= fadeTime)
                {
                    Time.timeScale = 1;
                    State = ScreenState.RUN;
                }
                break;
            case ScreenState.RUN:
                if (Input.anyKeyDown)
                {
                    currentFadeTime = 0;
                    State = ScreenState.OUT;
                }
                break;
            case ScreenState.OUT:
                currentFadeTime += Time.deltaTime;
                fadeCover.color = Color.Lerp(Color.clear, fadeColor, currentFadeTime / fadeTime);
                if (currentFadeTime >= fadeTime)
                {
                    SceneManager.LoadScene(nextScene);
                }
                break;
        }
    }
}
