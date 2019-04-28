using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    protected ScoreManager() { }

    public int Score = 0;

    public int Life
    {
        get
        {
            return life;
        }
        set
        {
            if (value < MaxLife)
            {
                life = value;
            }
            else
            {
                life = MaxLife;
            }
        }
    }

    public int MaxLife = 20;
    public int Level = 1;

    [SerializeField]
    private int life = 20;

    public void Reset()
    {
        MaxLife = 20;
        Score = 0;
        Life = MaxLife;
        Level = 1;
    }
}
