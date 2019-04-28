using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

enum CapitalistState
{
    PATROL,
    DOWN,
    INVALID
}

public class Capitalist : Enemy
{
    [Header("State Machine Settings")]
    [SerializeField]
    private CapitalistState state;

    [Header("Patrol Settings")]
    [SerializeField]
    private GameObject targetCereal;
    [SerializeField]
    private float fireChance;
    [SerializeField]
    private float fireTime;
    private float nextFireTime;

    protected override void Start()
    {
        base.Start();
        nextFireTime = Time.time + fireTime;
    }

    protected override void Update()
    {
        switch (state)
        {
            case CapitalistState.PATROL:
                if (targetCereal == null)
                {
                    GameObject[] candidateCereals = GameObject.FindGameObjectsWithTag("Finish");
                    if (candidateCereals.Length > 0)
                    {
                        int idxChosenCereal = Random.Range(0, candidateCereals.Length);
                        targetCereal = candidateCereals[idxChosenCereal];
                        Seeker seeker = GetComponent<Seeker>();
                        seeker.StartPath(transform.position, targetCereal.transform.position, OnPathComplete);
                    }
                } else if (path != null)
                {
                    Patrol();
                }
                break;

        }
        base.Update();
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
            targetCereal = null;    // clear target cereal
        }
    }
}

