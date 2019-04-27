using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Sprite[] idleSprites;

    [SerializeField]
    float animTime = 1f;

    float nextSpriteChangeTime;
    int idxSprite = 0;

    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextSpriteChangeTime = Time.time + animTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpriteChangeTime)
        {
            idxSprite++;
            idxSprite %= idleSprites.Length;
            spriteRenderer.sprite = idleSprites[idxSprite];
            nextSpriteChangeTime += animTime;
        }
    }
}
