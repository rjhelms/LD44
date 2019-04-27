using UnityEngine;

public class PlayerController : BaseActor
{

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
}