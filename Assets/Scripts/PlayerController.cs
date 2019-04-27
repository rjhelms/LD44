using System;
using System.Collections;
using System.Collections.Generic;
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
}
