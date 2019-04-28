using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

enum CapitalistState
{
    PATROL,
    WANDER,
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

    [Header("Wander Settings")]
    [SerializeField]
    private float wanderAmount;

    [Header("Projectile Settings")]
    [SerializeField]
    private GameObject ProjectilePrefab;
    [SerializeField]
    private Transform ProjectileSource;
    [SerializeField]
    private float fireChance;
    [SerializeField]
    private float fireTime;
    [SerializeField]
    private float projectileVectorFudge = 0.1f;

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
                    moveVector = Vector2.zero;
                } else if (path != null)
                {
                    Patrol();
                }
                if (Time.time > nextFireTime)
                {
                    nextFireTime = Time.time + fireTime;
                    if (Random.value <= fireChance)
                    {
                        FireProjectile();
                    }
                }
                break;
            case CapitalistState.WANDER:
                if (path != null)
                {
                    Patrol();
                }
                if (Time.time > nextFireTime)
                {
                    nextFireTime = Time.time + fireTime;
                    if (Random.value <= fireChance)
                    {
                        FireProjectile();
                    }
                }
                break;
        }
        base.Update();
    }

    protected override void PatrolDone()
    {
        base.PatrolDone();
        if (state == CapitalistState.PATROL)
        {
            Wander();
        } else
        {
            state = CapitalistState.PATROL;
            targetCereal = null;
        }
    }

    private void FireProjectile()
    {
        GameObject objProjectile = Instantiate(ProjectilePrefab, ProjectileSource.position, Quaternion.identity);
        Projectile Projectile = objProjectile.GetComponent<Projectile>();

        Projectile.parent = gameObject;

        if (moveVector.magnitude > 0)
        {
            Projectile.moveVector = moveVector.normalized;
        }
        else
        {
            switch (direction)
            {
                case Direction.NORTH:
                    Projectile.moveVector = Vector2.up;
                    break;
                case Direction.EAST:
                    Projectile.moveVector = Vector2.right;
                    break;
                case Direction.SOUTH:
                    Projectile.moveVector = Vector2.down;
                    break;
                case Direction.WEST:
                    Projectile.moveVector = Vector2.left;
                    break;
            }
        }

        // add some randomness to projectile vector
        Projectile.moveVector += new Vector2(Random.Range(-projectileVectorFudge, projectileVectorFudge),
                                             Random.Range(-projectileVectorFudge, projectileVectorFudge));
        Projectile.moveVector.Normalize();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if we hit another enemy, do a wander
        if (collision.gameObject.tag == "Enemy")
        {
            Wander();
        }
    }

    private void Wander()
    {
        state = CapitalistState.WANDER;
        Seeker seeker = GetComponent<Seeker>();
        Vector2 wanderVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        wanderVector *= wanderAmount;
        seeker.StartPath(transform.position, transform.position + (Vector3)wanderVector, OnPathComplete);
    }
}

