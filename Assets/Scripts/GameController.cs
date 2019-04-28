﻿using System.Collections;
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
    [SerializeField]
    private Color lifeDefaultColor;
    [SerializeField]
    private Color lifeWarningColor;

    private RawImage fadeCover;
    private RawImage lifeBar;
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
        fadeCover.color = fadeColor;
        mainCamera = FindObjectOfType<Camera>();
        canvasUI = GameObject.Find("UICanvas").GetComponent<Canvas>();
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
                    SceneManager.LoadScene("Main");
                }
                break;
        }
    }

    void UpdateUI()
    {
        canvasUI.GetComponent<CanvasScaler>().scaleFactor = mainCamera.GetComponent<PixelPerfectCamera>().pixelRatio * 2;
        lifeBar.rectTransform.localScale = new Vector3(ScoreManager.Instance.Life, 1, 1);
        if (ScoreManager.Instance.Life <= 5)
        {
            lifeBar.color = lifeWarningColor;
        } else
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
