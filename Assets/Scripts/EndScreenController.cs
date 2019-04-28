using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;


public class EndScreenController : ScreenController
{

    private Text scoreText;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        scoreText.text = ScoreManager.Instance.Score.ToString();
        ScoreManager.Instance.Reset();
    }
}
