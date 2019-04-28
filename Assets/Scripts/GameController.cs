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
    [SerializeField]
    private Color lifeDefaultColor;
    [SerializeField]
    private Color lifeWarningColor;

    [SerializeField]
    private GameObject[] levels;
    [SerializeField]
    private bool instantiateLevel;

    [SerializeField]
    private int mansUnit = 9;

    private RawImage fadeCover;
    private RawImage lifeBar;
    private Text levelText;
    private Text scoreText;
    private Image mansImage;
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
        lifeBar = GameObject.Find("LifeBar").GetComponent<RawImage>();
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        mansImage = GameObject.Find("MansImage").GetComponent<Image>();
        levelText.text = "LEVEL " + ScoreManager.Instance.Level;
        fadeCover.color = fadeColor;
        mainCamera = FindObjectOfType<Camera>();
        canvasUI = GameObject.Find("UICanvas").GetComponent<Canvas>();
        SetupLevel();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
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
                if (cerealRemaining <= 0)
                {
                    Time.timeScale = 0;
                    currentFadeTime = 0;
                    State = GameState.WIN;
                }
                if (ScoreManager.Instance.Life <= 0)
                {
                    Time.timeScale = 0;
                    currentFadeTime = 0;
                    ScoreManager.Instance.Mans--;
                    State = GameState.LOSE;
                }
                break;
            case GameState.WIN:
                currentFadeTime += Time.unscaledDeltaTime;
                fadeCover.color = Color.Lerp(Color.clear, fadeColor, currentFadeTime / fadeTime);
                if (currentFadeTime >= fadeTime)
                {
                    if (ScoreManager.Instance.Level < levels.Length)
                    {
                        ScoreManager.Instance.Level++;
                        SceneManager.LoadScene("Main");
                    }
                    else
                    {
                        Debug.Log("You win!");
                        // TODO: go to win screen
                    }
                }
                break;
            case GameState.LOSE:
                currentFadeTime += Time.unscaledDeltaTime;
                fadeCover.color = Color.Lerp(Color.clear, fadeColor, currentFadeTime / fadeTime);
                if (currentFadeTime >= fadeTime)
                {
                    if (ScoreManager.Instance.Mans >= 0)
                    {
                        ScoreManager.Instance.Die(); ;
                        SceneManager.LoadScene("Main");
                    }
                    else
                    {
                        Debug.Log("You lose!");
                        // TODO: go to win screen
                    }
                }
                break;
        }
    }

    void SetupLevel()
    {
        if (instantiateLevel)
        {
            Instantiate(levels[ScoreManager.Instance.Level - 1], Vector3.zero, Quaternion.identity);
        }
        AstarPath.active.Scan();
        Transform player = FindObjectOfType<PlayerController>().transform;
        mainCamera.transform.position = new Vector3(player.position.x, player.position.y, -10);
        mainCamera.GetComponent<CameraController>().targetPosition = mainCamera.transform.position;
    }

    void UpdateUI()
    {
        canvasUI.GetComponent<CanvasScaler>().scaleFactor = mainCamera.GetComponent<PixelPerfectCamera>().pixelRatio * 2;
        if (ScoreManager.Instance.Life > 0)
        {
            lifeBar.rectTransform.localScale = new Vector3(ScoreManager.Instance.Life, 1, 1);
        }
        else
        {
            lifeBar.rectTransform.localScale = new Vector3(ScoreManager.Instance.Life, 1, 1);
        }
        scoreText.text = ScoreManager.Instance.Score.ToString();
        mansImage.rectTransform.sizeDelta = new Vector2(mansUnit * ScoreManager.Instance.Mans, 8);
        if (ScoreManager.Instance.Life <= 5)
        {
            lifeBar.color = lifeWarningColor;
        }
        else
        {
            lifeBar.color = lifeDefaultColor;
        }
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
