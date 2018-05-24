using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickComponent : MonoBehaviour {

    // The power level of the brick (how much you get when you destroy it)
    public int PowerLevel = 100;

    // How much it takes to destroy the brick
    public int BrickStrength = 1;
    int BrickHealth = 1;

    // Static int variable that stores the number of brick in the scene
    public static int BrickCount = 0;

    GameController GameControllerRef;

    // Keep track of the ColorModifier component attached to the parent object
    ColorModifier ColorComponent;

    // If this brick spawn a powerup
    public bool HasPowerUp = false;

    // Powerup prefrab
    public GameObject PowerUp;

    // Use this for initialization
    void Start ()
    {
        // Cache the game controller component
        GameControllerRef = GameObject.Find("GameController").GetComponent<GameController>();

        // Get the color modifier component
        ColorComponent = GetComponentInParent<ColorModifier>();

        // Set the color mode as fixed
        ColorComponent.ModifierMode = ColorModifier.ColorMode.FIXED;
        
        // The brick health is equal to the strength
        BrickHealth = BrickStrength;

        // Increment the amount of brick object in the level
        ++BrickCount;
    }

    // When the brick get destroyed
    void OnDestroy()
    {
        // decrease the number of brick in the scene
        --BrickCount;
    }

    void RegisterHit()
    {        
        // If health is < 0 we destroy the brick
        if (--BrickHealth == 0)
        {
            // Spawn a powerup if it has onw
            if (HasPowerUp)
            {
                GameObject NewPowerUp = Instantiate(PowerUp, transform.position, transform.rotation, GameObject.Find("GameParent").transform);
                NewPowerUp.name = "PowerUP_" + name;
                switch (GameControllerRef.CurrentLevel)
                {
                    case 1:
                        NewPowerUp.GetComponent<PowerUpComponent>().PowerUpType = PowerUpComponent.Type.LARGE_PADDLE;
                        break;
                    case 2:
                        NewPowerUp.GetComponent<PowerUpComponent>().PowerUpType = PowerUpComponent.Type.MAGNETIC_PADDLE;
                        break;
                    case 3:
                        NewPowerUp.GetComponent<PowerUpComponent>().PowerUpType = PowerUpComponent.Type.STRONG_BALL;
                        break;
                }
            }

            // Destroy the brick
            Destroy(gameObject);

            // Notify the game controller we destroyed a brick
            GameControllerRef.AddScore(PowerLevel);
        }
        // If it takes more then 1 hit to destroy the brick reduce the alpha value
        else if (BrickStrength > 1)
        {
            // Get the current color of the brick
            Color CurrentColor = ColorComponent.GetCurrentColor();

            // Adjust the alpha value
            CurrentColor.a = 1f * BrickHealth / BrickStrength;

            // Reassign the adjusted color to the brick
            ColorComponent.SetColor(CurrentColor);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnTriggerEnter2D(Collider2D Other)
    {
        if (Other.GetComponent<BallComponent>() == null)
            return;

        // Determine where to bounce the ball vertically
        float ThisY = GetComponentInParent<RectTransform>().localPosition.y;
        float BallY = Other.GetComponent<RectTransform>().localPosition.y;

        Other.GetComponent<BallComponent>().Movement.y = ThisY > BallY ? -1f : 1f;

        // If the ball is on fire we insta destroy the block
        if (Other.GetComponent<BallComponent>().isOnFire == true)
            BrickHealth = 1;

        // Notify the brick we have been hit
        RegisterHit();
    }
}
