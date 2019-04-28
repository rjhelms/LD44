using UnityEngine;

public class PlayerController : BaseActor
{
    [Header("Player Settings")]
    [SerializeField]
    private GameObject ProjectilePrefab;
    [SerializeField]
    private Transform ProjectileSource;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        ProcessInput();
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
        GameObject objProjectile = Instantiate(ProjectilePrefab, ProjectileSource.position, Quaternion.identity);
        Projectile Projectile = objProjectile.GetComponent<Projectile>();
        Projectile.parent = gameObject;
        if (moveVector.magnitude > 0)
        {
            Projectile.moveVector = moveVector.normalized;
        } else
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
}