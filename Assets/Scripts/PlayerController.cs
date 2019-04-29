using UnityEngine;

public class PlayerController : BaseActor
{
    [Header("Projectile Settings")]
    [SerializeField]
    private GameObject ProjectilePrefab;
    [SerializeField]
    private Transform ProjectileSource;
    [SerializeField]
    private int ProjectileLifeCost = 1;
    [SerializeField]
    private int ProjectileLifeMin = 5;

    [Header("Hit Settings")]
    [SerializeField]
    public bool isInvulnerable;
    [SerializeField]
    private float flashTime;
    [SerializeField]
    private float invulnerableTime;

    private float invulnurableEndTime;
    private float nextFlashTime;

    private GameController controller;
    // Start is called before the first frame update
    protected override void Start()
    {
        controller = FindObjectOfType<GameController>();
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (controller.State == GameState.RUNNING)
        {
            ProcessInput();
            if (isInvulnerable)
            {
                if (Time.time > nextFlashTime)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                    nextFlashTime += flashTime;
                }
                if (Time.time > invulnurableEndTime)
                {
                    isInvulnerable = false;
                    spriteRenderer.enabled = true;
                }
            }
        }
        base.Update();
    }

    void ProcessInput()
    {
        float x_move = Input.GetAxisRaw("Horizontal");
        float y_move = Input.GetAxisRaw("Vertical");
        moveVector = new Vector2(x_move, y_move);
        moveVector.Normalize();
        if (Input.GetButtonDown("Fire"))
        {
            FireProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ScreenBounds")
        {
            Vector2 shift = new Vector2(0, 0);
            if (collision.gameObject.name == "LeftScreenEdge")
            { shift.x = -1; }
            else if (collision.gameObject.name == "RightScreenEdge")
            { shift.x = 1; }
            else if (collision.gameObject.name == "TopScreenEdge")
            {
                shift.y = 1;
            }
            else if (collision.gameObject.name == "BottomScreenEdge")
            {
                shift.y = -1;
            }
            FindObjectOfType<CameraController>().ShiftCamera(shift);
        }
    }

    private void FireProjectile()
    {
        // don't fire projectile if it would go below min life - you can't die from shooting cereal
        if (ScoreManager.Instance.Life < ProjectileLifeMin)
        {
            return;
        }
        GameController controller = GameObject.Find("GameController").GetComponent<GameController>();
        controller.PlaySound(controller.ShootSound);
        ScoreManager.Instance.Life -= ProjectileLifeCost;

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
    }

    public override void Hit(int damage)
    {
        if (!isInvulnerable)
        {
            isInvulnerable = true;
            invulnurableEndTime = Time.time + invulnerableTime;
            nextFlashTime = Time.time + flashTime;
            spriteRenderer.enabled = !spriteRenderer.enabled;
            ScoreManager.Instance.Life -= damage;
            if (ScoreManager.Instance.Life > 0)
            {
                GameController controller = GameObject.Find("GameController").GetComponent<GameController>();
                controller.PlaySound(controller.PlayerHitSound);
            }
            else
            {
                GameController controller = GameObject.Find("GameController").GetComponent<GameController>();
                controller.PlaySound(controller.LevelLoseSound);
            }
        }

        base.Hit(damage);
    }
}