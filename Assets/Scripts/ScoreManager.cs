using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    protected ScoreManager() { }

    public int Score = 0;
    public int Life = 20;
    public int MaxLife = 20;
    public int Level = 1;

    public void Reset()
    {
        MaxLife = 20;
        Score = 0;
        Life = 20;
        Level = 1;
    }
}
