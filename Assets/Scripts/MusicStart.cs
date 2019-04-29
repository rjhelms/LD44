using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStart : MonoBehaviour
{
    public GameObject MusicPrefab;

    private GameObject musicPlayer;
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GameObject.Find("MusicPlayer");
        if (musicPlayer == null)
        {
            musicPlayer = Instantiate(MusicPrefab);
            DontDestroyOnLoad(musicPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
