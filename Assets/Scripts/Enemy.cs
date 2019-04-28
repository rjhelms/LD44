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
}
