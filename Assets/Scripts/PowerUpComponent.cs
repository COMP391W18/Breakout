using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpComponent : MonoBehaviour {

    // Type of powerup
    public enum Type { LARGE_PADDLE, MAGNETIC_PADDLE, STRONG_BALL };

    // The current power up
    public Type PowerUpType = Type.STRONG_BALL;

    // How much point we get
    public int PowerLevel = 200;

    // How long the powerup last
    public float PowerUpTime = 5f;

    // Ref to the game controller
    GameController GameControllerRef;

    // Use this for initialization
    void Start () {
        // Cache the game controller component
        GameControllerRef = GameObject.Find("GameController").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        // Notify the game controller we destroyed a powerup
        GameControllerRef.AddScore(PowerLevel);
    }

    public void Reset()
    {
        StopAllCoroutines();
    }

    public IEnumerator ApplyPowerUp(GameObject Other)
    {
        PaddleController Paddle = Other.GetComponent<PaddleController>();

        // If the other component alread has a power up destroy the power up now
        if (Paddle.HasPowerUp)
        {
            Destroy(gameObject);
            yield return null;
        }

        //
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        switch (PowerUpType)
        {
            case Type.LARGE_PADDLE:
                Paddle.HasPowerUp = true;
                Other.transform.localScale = new Vector3(100f, 200f);
                yield return new WaitForSeconds(PowerUpTime);
                Paddle.HasPowerUp = false;
                Other.transform.localScale = new Vector3(100f, 100f);
                break;
            case Type.MAGNETIC_PADDLE:
                Paddle.IsMagnetic = true;
                Paddle.HasPowerUp = true;
                Other.GetComponent<ColorModifier>().ModifierMode = ColorModifier.ColorMode.FIXED;
                Other.GetComponent<ColorModifier>().SetColor(new Color(1, 1, 1));
                yield return new WaitForSeconds(PowerUpTime);
                Paddle.IsMagnetic = false;
                Paddle.HasPowerUp = false;
                GameObject.Find("Ball").GetComponent<BallComponent>().CurrentStatus = BallComponent.Status.Moving;
                Other.GetComponent<ColorModifier>().ModifierMode = ColorModifier.ColorMode.TIME_BASED;
                break;
            case Type.STRONG_BALL:
                BallComponent Ball = GameObject.Find("Ball").GetComponent<BallComponent>();
                ColorModifier BallColor = GameObject.Find("Ball").GetComponent<ColorModifier>();

                Paddle.HasPowerUp = true;
                Ball.isOnFire = true;
                BallColor.ModifierMode = ColorModifier.ColorMode.FIXED;
                BallColor.SetColor(new Color(1f, 0f, 0f));
                yield return new WaitForSeconds(PowerUpTime);
                Paddle.HasPowerUp = false;
                BallColor.ModifierMode = ColorModifier.ColorMode.TIME_BASED;
                Ball.isOnFire = false;
                break;
        }

        // Destroy the object
        Destroy(gameObject);
    }

    // When our power up connects with a paddle
    void OnTriggerEnter2D(Collider2D Other)
    {
        if (Other.name == "Paddle")
        {
            StartCoroutine(ApplyPowerUp(Other.gameObject));
            
        }
    }
}
