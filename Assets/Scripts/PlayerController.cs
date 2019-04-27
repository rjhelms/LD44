using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction
{
    NORTH,
    EAST,
    SOUTH,
    WEST,
    INVALID
}

[Serializable]
public class WalkCycle
{
    public Sprite[] walkSprites;
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Sprite[] idleSprites;
    [SerializeField]
    WalkCycle[] walkCycles;
    [SerializeField]
    float animTime = .2f;
    [SerializeField]
    float rotateTime = 2f;
    [SerializeField]
    Direction direction;

    float nextSpriteChangeTime;
    float nextRotateTime;

    int idxSprite = 0;

    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextSpriteChangeTime = Time.time + animTime;
        nextRotateTime = Time.time + rotateTime;
        spriteRenderer.sprite = walkCycles[(int)direction].walkSprites[idxSprite];
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextRotateTime)
        {
            direction++;
            if (direction == Direction.INVALID)
            {
                direction = Direction.NORTH;
            }
            idxSprite = 0;
            spriteRenderer.sprite = walkCycles[(int)direction].walkSprites[idxSprite];
            nextRotateTime += rotateTime;
            nextSpriteChangeTime = Time.time + animTime;
        }
        
        if (Time.time >= nextSpriteChangeTime)
        {
            idxSprite++;
            idxSprite %= walkCycles[(int)direction].walkSprites.Length;
            spriteRenderer.sprite = walkCycles[(int)direction].walkSprites[idxSprite];
            nextSpriteChangeTime += animTime;
        }
    }
}
