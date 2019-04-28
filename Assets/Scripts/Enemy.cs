using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : BaseActor
{
    [Header("Pathfinding Settings")]
    [SerializeField]
    protected float nextWaypointDistance = 0.5f;

    protected Path path;
    protected int currentWaypoint = 0;

    protected void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        path = p;
        currentWaypoint = 0;
    }

    protected void Patrol()
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
            PatrolDone();
        }
    }

    protected virtual void PatrolDone()
    { }
}
