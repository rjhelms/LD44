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

    protected override void PatrolDone()
    {
        base.PatrolDone();
        targetCereal = null;    // clear target cereal
    }
}

