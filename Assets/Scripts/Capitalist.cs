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
    [SerializeField]
    private bool foundCereal;

    [Header("Wander Settings")]
    [SerializeField]
    private float wanderAmount;
    [SerializeField]
    private float maxWanderWaitTime;

    [Header("Down Settings")]
    [SerializeField]
    float downTime;
    [SerializeField]
    Sprite downSprite;

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
    private float nextStateChangeTime;
    private float wanderWait;

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
                if (!foundCereal)
                {
                    GameObject[] candidateCereals = GameObject.FindGameObjectsWithTag("Finish");
                    if (candidateCereals.Length > 0)
                    {
                        int idxChosenCereal = Random.Range(0, candidateCereals.Length);
                        targetCereal = candidateCereals[idxChosenCereal];
                        foundCereal = true;
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
            case CapitalistState.DOWN:
                moveVector = Vector2.zero;
                if (Time.time > nextStateChangeTime)
                {
                    foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
                    {
                        c.enabled = true;
                    }
                    updateSprites = true;
                    state = CapitalistState.WANDER;
                    Wander();
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
            foundCereal = false;
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
        if (collision.gameObject.tag == "Enemy" && state != CapitalistState.DOWN)
        {
            Wander();
            wanderWait = Time.time + Random.Range(0, maxWanderWaitTime);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // if we're still hitting another enemy, and we got a path back, try again
        if (collision.gameObject.tag == "Enemy" && Time.time > wanderWait && path != null && state != CapitalistState.DOWN)
        {
            Wander();
            wanderWait = Time.time + Random.Range(0, maxWanderWaitTime);
        }
    }

    private void Wander()
    {
        if (state != CapitalistState.DOWN)
        {
            state = CapitalistState.WANDER;
            Seeker seeker = GetComponent<Seeker>();
            Vector2 wanderVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            wanderVector *= wanderAmount;
            seeker.StartPath(transform.position, transform.position + (Vector3)wanderVector, OnPathComplete);
        }
    }

    public override void Hit(int damage)
    {
        state = CapitalistState.DOWN;
        foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
        {
            c.enabled = false;
        }
        nextStateChangeTime = Time.time + downTime;
        updateSprites = false;
        spriteRenderer.sprite = downSprite;
    }
}

