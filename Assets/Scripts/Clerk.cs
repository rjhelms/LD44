using Pathfinding;
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
    private Sprite[] alertSprite;

    [Header("State Machine Settings")]
    [SerializeField]
    private ClerkState state;
    [SerializeField]
    private SpriteRenderer alertRenderer;
    [SerializeField]
    private float[] stateMoveSpeeds;

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
    LayerMask lookLayerMask;
    [SerializeField]
    Transform lookSource;

    [Header("Confused Settings")]
    [SerializeField]
    float ConfuseTime;

    private float nextStateChangeTime;
    private float nextLookTime;

    [SerializeField]
    private float nextWaypointDistance = 0.5f;

    private Path path;
    private int currentWaypoint = 0;

    protected override void Start()
    {
        nextLookTime = Time.time + nextLookTime;
        base.Start();
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
            // do a raycast to see if we can still see player
            Vector2 lookDirection = ((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position
                                        + new Vector2(0.5f, 1.0f)   // ugly offset to look at player's CoM
                                        - (Vector2)lookSource.position).normalized;
            Debug.DrawRay(lookSource.position, lookDirection, Color.white, 1f);
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
                        && results[i].collider.transform.parent.tag == "Player")  // and is the player...
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
                nextStateChangeTime = Time.time + ConfuseTime;
                moveSpeed = stateMoveSpeeds[(int)ClerkState.CONFUSED];
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
        rotatedVector = rotatedVector.normalized;

        Debug.DrawLine(lookSource.position, lookSource.position + (Vector3)rotatedVector);

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
                    && results[i].collider.transform.parent.tag == "Player")  // and is the player...
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

    private void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        path = p;
        currentWaypoint = 0;
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
}
