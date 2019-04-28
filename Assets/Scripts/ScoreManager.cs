using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    protected ScoreManager() { }

    public int Score = 0;
    public int Mans = 3;
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
        Mans = 3;
        Score = 0;
        life = MaxLife;
        Level = 1;
    }

    public void Die()
    {
        MaxLife = 20;
        life = MaxLife;
        Mans -= 1;
    }

    public void AddLife(int value)
    {
        life += value;
        if (life > MaxLife)
        {
            MaxLife = life;
        }
    }
}
