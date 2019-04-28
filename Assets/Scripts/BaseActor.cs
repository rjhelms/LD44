using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
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

public class BaseActor : MonoBehaviour
{
    [Header("Base Actor Settings")]
    [SerializeField]
    protected Sprite[] idleSprites;
    [SerializeField]
    protected WalkCycle[] walkCycles;
    [SerializeField]
    protected float animTime = .3f;
    [SerializeField]
    protected Direction direction;
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected Vector2 moveVector = new Vector2(0f, 0f);

    float nextSpriteChangeTime;
    SpriteRenderer spriteRenderer;
    int idxSprite = 0;
    protected Rigidbody2D rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer.sprite = walkCycles[(int)direction].walkSprites[idxSprite];

        nextSpriteChangeTime = Time.time + (animTime / moveSpeed);

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Time.time >= nextSpriteChangeTime)
        {
            idxSprite++;
            idxSprite %= walkCycles[(int)direction].walkSprites.Length;
            nextSpriteChangeTime += (animTime / moveSpeed);
        }
        UpdateDirection();
        UpdateSprite();
        rb.velocity = moveVector * moveSpeed;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    void UpdateDirection()
    {
        if (moveVector.magnitude == 0)
            return;
        float move_angle = Vector2.SignedAngle(Vector2.right, moveVector);
        if (-180 <= move_angle && move_angle < -135)
        {
            direction = Direction.WEST;
        }
        else if (-135 <= move_angle && move_angle <= -45)
        {
            direction = Direction.SOUTH;
        }
        else if (-45 < move_angle && move_angle <= 45)
        {
            direction = Direction.EAST;
        }
        else if (45 < move_angle && move_angle < 135)
        {
            direction = Direction.NORTH;
        }
        else if (135 <= move_angle && move_angle <= 180)
        {
            direction = Direction.WEST;
        }
    }

    void UpdateSprite()
    {
        if (moveVector.magnitude == 0)
        {
            spriteRenderer.sprite = idleSprites[(int)direction];
            idxSprite = 0;
        }
        else
        {
            spriteRenderer.sprite = walkCycles[(int)direction].walkSprites[idxSprite];
        }
    }
}
