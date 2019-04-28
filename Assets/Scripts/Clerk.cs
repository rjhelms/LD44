using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ClerkState
{
    PATROL,
    ALERT,
    CONFUSED,
    INVALID
}

public class Clerk : Enemy
{
    [SerializeField]
    private ClerkState state;
    [SerializeField]
    private Sprite[] alertSprite;
    [SerializeField]
    private SpriteRenderer alertRenderer;

    [SerializeField]
    private float lookCurrentAngle;
    [SerializeField]
    private float lookDistance;
    [SerializeField]
    private float lookFOV;
    [SerializeField]
    private float lookStep;
    [SerializeField]
    private float lookTime;
    [SerializeField]
    LayerMask lookLayerMask;
    [SerializeField]
    Transform lookSource;

    private float nextLookTime;
    protected override void Start()
    {
        nextLookTime = Time.time + nextLookTime;
        base.Start();
    }

    protected override void Update()
    {
        switch (state)
        {
            case ClerkState.PATROL:
                break;
            case ClerkState.ALERT:
                break;
            case ClerkState.CONFUSED:
                break;
            default:
                SetState(ClerkState.PATROL);
                break;
        }

        alertRenderer.sprite = alertSprite[(int)state];
        if (Time.time >= nextLookTime)
            Look();
        base.Update();
    }

    private void SetState(ClerkState newState)
    {
        // put logic to set up states in here
        switch (newState)
        {
            case ClerkState.PATROL:
                state = ClerkState.PATROL;
                break;
            case ClerkState.ALERT:
                state = ClerkState.ALERT;
                break;
            case ClerkState.CONFUSED:
                state = ClerkState.CONFUSED;
                break;
            default:
                Debug.Log("Invalid state! " + newState);
                break;
            
        }
    }
    private void Look()
    {
        Vector2 lookVector;
        switch (direction)
        {
            case Direction.NORTH:
                lookVector = Vector2.up;
                break;
            case Direction.EAST:
                lookVector = Vector2.right;
                break;
            case Direction.SOUTH:
                lookVector = Vector2.down;
                break;
            case Direction.WEST:
                lookVector = Vector2.left;
                break;
            default:
                Debug.Log("Invalid direction: " + direction);
                return;
        }
        float rad = lookCurrentAngle * Mathf.Deg2Rad;
        float s = Mathf.Sin(rad);
        float c = Mathf.Cos(rad);
        Vector2 rotatedVector = Vector2.zero;
        rotatedVector.x = lookVector.x * c - lookVector.y * s;
        rotatedVector.y = lookVector.x * s + lookVector.y * c;
        rotatedVector = rotatedVector.normalized * lookDistance;

        Debug.DrawLine(lookSource.position, lookSource.position + (Vector3)rotatedVector);

        lookCurrentAngle += lookStep;
        if (Mathf.Abs(lookCurrentAngle) >= lookFOV)
        {
            lookStep *= -1;
        }

        nextLookTime = Time.time + lookTime;
    }
}
