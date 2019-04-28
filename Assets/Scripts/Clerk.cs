using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
enum ClerkState
{
    PATROL,
    ALERT,
    CONFUSED,
    DOWN,
    INVALID
}

public class Clerk : Enemy
{
    [SerializeField]
    private Sprite[] alertSprite;

    [Header("State Machine Settings")]
    [SerializeField]
    private ClerkState state;
    [SerializeField]
    private SpriteRenderer alertRenderer;
    [SerializeField]
    private float[] stateMoveSpeeds;

    private float nextStateChangeTime;
    private float nextLookTime;

    [Header("Patrol Settings")]
    [SerializeField]
    private Transform[] patrolPoints;

    private int currentPatrolPoint;

    [Header("Alert Settings")]
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
    private LayerMask lookLayerMask;
    [SerializeField]
    private Transform lookSource;

    [Header("Confused Settings")]
    [SerializeField]
    float confuseTime;

    [Header("Down Settings")]
    [SerializeField]
    float downTime;
    [SerializeField]
    Sprite downSprite;

    protected override void Start()
    {
        base.Start();
        nextLookTime = Time.time + nextLookTime;
        StartPatrol();
    }

    protected override void Update()
    {
        switch (state)
        {
            case ClerkState.PATROL:
                if (Time.time >= nextLookTime)
                    Look();
                if (path != null)
                {
                    Patrol();
                }
                break;
            case ClerkState.ALERT:
                if (path != null)
                {
                    Chase();
                }
                else
                {
                    moveVector = Vector2.zero;
                }
                break;
            case ClerkState.CONFUSED:
                moveVector = Vector2.zero;
                direction = Direction.SOUTH;
                if (Time.time > nextStateChangeTime)
                    SetState(ClerkState.PATROL);
                break;
            case ClerkState.DOWN:
                moveVector = Vector2.zero;
                if (Time.time > nextStateChangeTime)
                {
                    foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
                    {
                        c.enabled = true;
                    }
                    updateSprites = true;
                    SetState(ClerkState.PATROL);
                }
                break;
            default:
                SetState(ClerkState.PATROL);
                break;
        }

        alertRenderer.sprite = alertSprite[(int)state];

        base.Update();
    }

    private void Chase()
    {
        bool reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    reachedEndOfPath = true;
                    path = null; // get rid of the path to be sure
                    break;
                }
            }
            else
            {
                break;
            }
        }
        if (!reachedEndOfPath)
        {
            moveVector = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
        }
        else
        {
            // do a raycast to see if we can still see player
            Vector2 lookDirection = ((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position
                                        + new Vector2(0f, 0.75f)   // ugly offset to look at player's CoM
                                        - (Vector2)lookSource.position).normalized;
            Debug.DrawRay(lookSource.position, lookDirection * lookDistance / 2, Color.white, 0.5f);
            List<RaycastHit2D> results = new List<RaycastHit2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(lookLayerMask);
            bool foundPlayer = false;
            // look directly at the player, at half of the normal look distance
            int resultCount = Physics2D.Raycast(lookSource.position, lookDirection, filter, results, lookDistance / 2);
            if (resultCount > 0)
            {
                for (int i = 0; i < resultCount; i++)
                {
                    if (results[i].collider.gameObject.layer == 8                 // on the RaycastTarget layer...
                        && results[i].collider.transform.tag == "Player")         // and is the player...
                    {

                        SetState(ClerkState.ALERT);
                        Seeker seeker = GetComponent<Seeker>();
                        seeker.StartPath(transform.position, results[i].collider.transform.position, OnPathComplete);
                        foundPlayer = true;
                        break;
                    }
                    else if (results[i].collider.gameObject.layer == 9)
                    {
                        break; // stop on terrain
                    }
                }
            }
            if (!foundPlayer)
            {
                SetState(ClerkState.CONFUSED);
            }
        }
    }

    private void SetState(ClerkState newState)
    {
        moveVector = Vector2.zero;
        // put logic to set up states in here
        switch (newState)
        {
            case ClerkState.PATROL:
                moveSpeed = stateMoveSpeeds[(int)ClerkState.PATROL];
                StartPatrol();
                state = ClerkState.PATROL;
                break;
            case ClerkState.ALERT:
                moveSpeed = stateMoveSpeeds[(int)ClerkState.ALERT];
                state = ClerkState.ALERT;
                break;
            case ClerkState.CONFUSED:
                nextStateChangeTime = Time.time + confuseTime;
                moveSpeed = stateMoveSpeeds[(int)ClerkState.CONFUSED];
                state = ClerkState.CONFUSED;
                break;
            case ClerkState.DOWN:
                updateSprites = false;
                spriteRenderer.sprite = downSprite;
                nextStateChangeTime = Time.time + downTime;
                moveSpeed = stateMoveSpeeds[(int)ClerkState.DOWN];
                foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
                {
                    c.enabled = false;
                }
                state = ClerkState.DOWN;
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
        rotatedVector = rotatedVector.normalized;

        Debug.DrawLine(lookSource.position, lookSource.position + ((Vector3)rotatedVector * lookDistance), Color.yellow, 0.5f);

        lookCurrentAngle += lookStep;
        if (Mathf.Abs(lookCurrentAngle) >= lookFOV)
        {
            lookStep *= -1;
        }
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(lookLayerMask);
        int resultCount = Physics2D.Raycast(lookSource.position, rotatedVector, filter, results, lookDistance);
        if (resultCount > 0)
        {
            for (int i = 0; i < resultCount; i++)
            {
                if (results[i].collider.gameObject.layer == 8                 // on the RaycastTarget layer...
                    && results[i].collider.transform.tag == "Player")         // and is the player...
                {
                    SetState(ClerkState.ALERT);
                    Seeker seeker = GetComponent<Seeker>();
                    seeker.StartPath(transform.position, results[i].collider.transform.position, OnPathComplete);
                    break;
                }
                else if (results[i].collider.gameObject.layer == 9)
                {
                    break; // stop on terrain
                }
            }
        }
        nextLookTime = Time.time + lookTime;
    }

    private void StartPatrol()
    {
        Seeker seeker = GetComponent<Seeker>();
        seeker.StartPath(transform.position, patrolPoints[currentPatrolPoint].position, OnPathComplete);
    }

    private void Patrol()
    {
        bool reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    SetState(ClerkState.PATROL);
                    reachedEndOfPath = true;
                    path = null; // get rid of the path to be sure
                    break;
                }
            }
            else
            {
                break;
            }
        }
        if (!reachedEndOfPath)
        {
            moveVector = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
        }
        else
        {
            // increment patrol point
            currentPatrolPoint++;
            currentPatrolPoint %= patrolPoints.Length;
            // start seeking new path
            StartPatrol();
        }
    }

    public override void Hit(int damage)
    {
        SetState(ClerkState.DOWN);
    }
}
