using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour {

    // How fast the paddle moves
    public float Speed = 5f;

    private Vector2 Boundary;

    private Rigidbody2D PaddleBody;

    public bool HasPowerUp = false;
    public bool IsMagnetic = false;

    private void CheckInput()
    {
        float MoveHorizontal = Input.GetAxis("Horizontal");

        Vector2 Movement = new Vector2(MoveHorizontal, 0f);

        Rigidbody2D PaddleBody = GetComponentInParent<Rigidbody2D>();

        // Update the paddle velocity and makes sure it stays within the game area
        PaddleBody.velocity = Movement * Speed;
        PaddleBody.transform.localPosition = new Vector2(Mathf.Clamp(PaddleBody.transform.localPosition.x, Boundary.x, Boundary.y), PaddleBody.transform.localPosition.y);
    }
    

    // Use this for initialization
    void Start ()
    {
        // Camera boundary (game area boundary)
        float CameraBoundary = Camera.main.scaledPixelWidth / 2f;

        // Paddle width
        float PaddleWidth = 40f;

        // Set the boundary
        Boundary = new Vector2(-CameraBoundary + PaddleWidth / 2, CameraBoundary - PaddleWidth / 2);

        // Get the instance of the paddle body
        PaddleBody = GetComponentInParent<Rigidbody2D>();
    }

    // Reset the paddle 
    public void ResetPaddle()
    {
        // Reset the paddle X position
        Vector2 CurrentPosition = transform.localPosition;
        CurrentPosition.x = 0f;
        transform.localPosition = CurrentPosition;

        // Reset the RigidBody velocity
        PaddleBody.velocity = new Vector2(0f, 0f);

        // Reset the powerup modification
        GetComponent<ColorModifier>().ModifierMode = ColorModifier.ColorMode.TIME_BASED;
        HasPowerUp = false;
    }

    // Update is called once per frame
    void Update () {
        CheckInput();
    }

    void OnTriggerEnter2D(Collider2D Other)
    {
        switch (Other.name)
        {
            case "Ball":
                Other.GetComponent<BallComponent>().Movement.y = 1f;

                if (IsMagnetic)
                    Other.GetComponent<BallComponent>().CurrentStatus = BallComponent.Status.Attached;
                break;
            case "PowerUp":
                break;
        }
        
    }
}
