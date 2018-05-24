using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallComponent : MonoBehaviour {

    public enum Status
    {
        Moving,
        Attached
    }

    // How fast the paddle moves
    public float Speed = 5f;

    // The movement direction of the ball
    public Vector2 Movement = new Vector2(0f, 0f);

    // Cache the game controller component
    public GameController GameControllerRef;

    // The status of the ball
    public Status CurrentStatus { get; set; }

    public GameObject AttachedPaddle;

    private Vector4 Boundary;

    private Rigidbody2D BallBody;

    public bool isOnFire = false;

    // Check boundary to keep the ball in the play area
    public void CheckBoundary()
    {
        if (BallBody.transform.localPosition.x <= Boundary.x || BallBody.transform.localPosition.x >= Boundary.y)
            Movement.x *= -1;
        else if (BallBody.transform.localPosition.y <= Boundary.z || BallBody.transform.localPosition.y >= Boundary.w)
            Movement.y *= -1;
    }

    // Function to clamp the vector to make sure we don't leave the play area
    public Vector2 ClampVector(float X, float Y, ref Vector4 Limits)
    {
        return new Vector2(Mathf.Clamp(X, Limits.x, Limits.y), Mathf.Clamp(Y, Limits.z, Limits.w));
    }
    
    // Use this for initialization
    void Start ()
    {
        // Cache the game controller component
        GameControllerRef = GameObject.Find("GameController").GetComponent<GameController>();

        // By default the ball is attached to the paddle
        CurrentStatus = Status.Attached;

        // Camera boundary (game area boundary)
        float CameraBoundaryX = Camera.main.scaledPixelWidth / 2f;
        float CameraBoundaryY = Camera.main.scaledPixelHeight / 2f;

        // Ball radius
        float BallRadius = 6f;

        // Set the boundary
        Boundary = new Vector4(-CameraBoundaryX + BallRadius, CameraBoundaryX - +BallRadius, -CameraBoundaryY + BallRadius, CameraBoundaryY - BallRadius);

        // Get the ball body
        BallBody = GetComponentInParent<Rigidbody2D>();
    }

    // Reset the ball 
    public void ResetBall()
    {
        // Reset the status
        CurrentStatus = Status.Attached;

        // Keep the ball attached to the paddle
        BallBody.transform.localPosition = new Vector2(AttachedPaddle.transform.localPosition.x, AttachedPaddle.transform.localPosition.y + 11f);

        // Reset the RigidBody velocity
        BallBody.velocity = new Vector2(0f, 0f);

        // Reset the powerup modification
        GetComponent<ColorModifier>().ModifierMode = ColorModifier.ColorMode.TIME_BASED;
        isOnFire = false;
    }

    // Update is called once per frame
    void Update ()
    {       
        // If the ball is attached to the paddle
        if (CurrentStatus == Status.Attached)
        {
            // Keep the ball attached to the paddle
            BallBody.transform.localPosition = new Vector2(AttachedPaddle.transform.localPosition.x, AttachedPaddle.transform.localPosition.y + 11f);

            // Detach the ball if we press space
            if (Input.GetKeyUp(KeyCode.Space) && GameControllerRef.CurrentGameStatus == GameController.State.GAME_RUNNING)
                CurrentStatus = Status.Moving;
        }
        else if (CurrentStatus == Status.Moving)
        {
            // Make sure that if we hit the boundaries it flips
            CheckBoundary();

            // Update the ball velocity and makes sure it stays within the game area
            BallBody.velocity = Movement * Speed;
            BallBody.transform.localPosition = ClampVector(BallBody.transform.localPosition.x, BallBody.transform.localPosition.y, ref Boundary);
        }

        if (BallBody.transform.localPosition.y <= Boundary.z)
            GameControllerRef.OnPlayerLostLife();
    }
}
