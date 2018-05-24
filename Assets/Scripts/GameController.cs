using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    // Status of the game
    public enum State { GAME_RUNNING, GAME_PAUSED, GAME_ENDED }

    // Current player score
    int PlayerScore = 0;

    // Number of lives the player has
    public int PlayerLives = 3;

    // Current status of the game
    public State CurrentGameStatus = State.GAME_PAUSED;

    // GUI Object
    public GameObject GUI;

    // Parent for all the play entities
    public GameObject EntityParent;

    // Score text object
    public Text ScoreText;
    public Text LevelName;

    // Array of GameObjects for the player lives
    public GameObject HearthPrefab;
    GameObject[] LivesSprites;

    // Array of GameObjects for the bricks prefabs
    public GameObject BrickPrefab;
    BrickComponent[] Bricks;

    // Array with the background sprites datas
    public Sprite[] BackgroundSprites;

    // Array of Rigidbody2D components for the entities in the game
    List<Rigidbody2D> PhysicsBodies;

    // Current Level
    public int CurrentLevel = 1;
    
    // Time between level
    public float CountDownBetweenLevel = 3f;

    // Use this for initialization
    void Start()
    {
        // Create an array of sprites with the health object
        LivesSprites = new GameObject[PlayerLives];

        // Create an array of sprites with the health object
        Bricks = new BrickComponent[91];

        // Cache the GUI object
        GUI = GameObject.Find("GUI");

        // Cache the score text object and level name object
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        LevelName = GameObject.Find("LevelName").GetComponent<Text>();

        // Cache the EntityParent object
        EntityParent = GameObject.Find("GameParent");

        // Cache the PhysicsBodies
        PhysicsBodies = new List<Rigidbody2D>(EntityParent.GetComponentsInChildren<Rigidbody2D>());

        // Reset the game
        ResetGame(1);

        // Pause the game
        ChangeGameState(State.GAME_PAUSED);

        //Debug.Log("Width: " + Camera.main.pixelRect.width + ", Height:" + Camera.main.pixelRect.height);
    }

    // Clean the PhysicsBodies array
    private void CleanPhysicsBodiesArray(ref List<Rigidbody2D> Array)
    {
        Array.RemoveAll(Obj => Obj == null);
    }

    // Load the bricks
    private void LoadBricks()
    {
        // Compute how much we have to scale our objects
        Vector2 Scaler = new Vector2(Camera.main.pixelRect.width / 777.5f, Camera.main.pixelRect.height / 622f);

        // Reset the bricks
        for (System.UInt16 Index = 0; Index < 91; ++Index)
        {
            // Destroy the brick
            if (Bricks[Index] != null)
                DestroyImmediate(Bricks[Index].gameObject);

            int Col = (Index % 13);
            int Row = (Index / 13);

            // Instantiate a brick under the game parent object
            BrickComponent NewBrick = Bricks[Index] = Instantiate(BrickPrefab, new Vector3(), new Quaternion(), EntityParent.transform).GetComponent<BrickComponent>();

            // Set some property
            NewBrick.BrickStrength = 1;
            NewBrick.transform.localPosition = new Vector2((-300f + 50 * Col) * Scaler.x, (220f - 40 * Row) * Scaler.y);            
            NewBrick.name = "Brick_" + CurrentLevel + "_" + Index;

            //
            if (CurrentLevel == 1)
            {
                if (Col < 3 || Col > 9)
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(1, 0, 0));
                    NewBrick.BrickStrength = 1;
                }
                else
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(1, 1, 1));
                    NewBrick.BrickStrength = 2;
                }

                if (Index == 19 || Index == 32 || (Index > 42 && Index < 48) || (Index > 56 && Index < 60) || Index == 71 || Index == 84)
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(1, 0, 0));
                    NewBrick.BrickStrength = 3;
                }

            }
            else if (CurrentLevel == 2)
            {
                if (Col < 4)
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(0 / 255f, 146 / 255f, 70 / 255f));
                    NewBrick.BrickStrength = 1;
                }
                else if (Col < 9)
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(1, 1, 1));
                    NewBrick.BrickStrength = 2;
                }
                else
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(206 / 255f, 43 / 255f, 55 / 255f));
                    NewBrick.BrickStrength = 3;
                }
            }
            else if (CurrentLevel == 3)
            {
                // Yellow color
                if ((Index > 17 && Index < 21) || (Index > 28 && Index < 31) || (Index > 33 && Index < 36) || (Index > 39 && Index < 43) || (Index > 47 && Index < 51) || (Index > 54 && Index < 57) || (Index > 59 && Index < 62) || (Index > 69 && Index < 73))
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(254f / 255f, 254f / 255f, 0));
                    NewBrick.BrickStrength = 1;
                }
                // Blue color
                else if ((Index > 30 && Index < 34) || (Index > 42 && Index < 48) || (Index > 56 && Index < 60))
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(0f, 34 / 255f, 119 / 255f));
                    NewBrick.BrickStrength = 2;
                }
                // Green color
                else
                {
                    NewBrick.GetComponentInParent<ColorModifier>().SetColor(new Color(0f, 156 / 255f, 55 / 255f));
                    NewBrick.BrickStrength = 3;
                }
            }

            // Set how many points we get by destroying the brick
            NewBrick.PowerLevel = (7 - Row) * 50;

            // Sets whatever this brick will spawn a powerup
            NewBrick.HasPowerUp = Random.Range(0, 10) < 8 ? false : true;
        }
    }

    private void LoadLivesSprite()
    {
        GameObject LivesParent = GameObject.Find("Lives");
        
        for (int Index = 0; Index < PlayerLives; ++Index)
        {
            DestroyImmediate(LivesSprites[Index]);
            // Create a "hearth" prefab under the Lives game object
            LivesSprites[Index] = Instantiate(HearthPrefab, new Vector3(), new Quaternion(), LivesParent.transform);
            LivesSprites[Index].transform.localPosition = new Vector3(-20f - (25f * Index), -20f, 0);
        }
    }

    // Reset the game to default values
    void ResetGame(int NextLevel)
    {
        // Reset the player score and text
        PlayerScore = 0;
        ScoreText.text = "0";

        // Reset the level played
        CurrentLevel = NextLevel;
        LevelName.text = "Level " + System.Convert.ToString(CurrentLevel);

        // Change the BG sprite
        GameObject.Find("Game_BG").GetComponent<SpriteRenderer>().sprite = BackgroundSprites[CurrentLevel - 1];
        
        // Reset the number of player lives
        PlayerLives = 3;
        LoadLivesSprite();

        // Load the bricks
        LoadBricks();

        // Remove powerup

        // Reset the paddle and ball position
        GameObject.Find("Paddle").GetComponent<PaddleController>().ResetPaddle();
        GameObject.Find("Ball").GetComponent<BallComponent>().ResetBall();
        foreach (PowerUpComponent PowerUp in GameObject.Find("GameParent").GetComponentsInChildren<PowerUpComponent>())
            PowerUp.Reset();
    }
    
    // Call this function when we want to change the game state
    public void ChangeGameState(State NewState)
    {
        CurrentGameStatus = NewState;

        switch (NewState)
        {
            case State.GAME_RUNNING:
                OnGameRunning();
                break;

            case State.GAME_ENDED:
                OnGameEnded();
                break;

            case State.GAME_PAUSED:
                CleanPhysicsBodiesArray(ref PhysicsBodies);
                OnGamePaused();
                break;
        }
    }

    // Call this function to add score to the player
    public void AddScore(int NewScore)
    {
        PlayerScore += NewScore;

        ScoreText.text = System.Convert.ToString(PlayerScore);
    }

    // What happens when we lose a life
    public void OnPlayerLostLife()
    {
        // Decrease the number of lives
        --PlayerLives;

        // Destroy one of the lives
        Destroy(LivesSprites[PlayerLives]);

        // Reset the paddle and ball position
        GameObject.Find("Paddle").GetComponent<PaddleController>().ResetPaddle();
        GameObject.Find("Ball").GetComponent<BallComponent>().ResetBall();
    }

    // When we lose a game
    public void OnGameEnded()
    {
        // Show the UI
        GUI.SetActive(true);

        // Get the text component of the end message
        Text EndText = GameObject.Find("GameEndedMessage").GetComponent<Text>();

        // Display a message based on the result
        EndText.text = PlayerLives == 0 ? "You lost!" : "You won!";

        // Reset the game
        if (CurrentLevel < 3)
            ResetGame(PlayerLives == 0 ? 1 : ++CurrentLevel);
        else
            CurrentLevel = 0;

        // Hide some part of the GUI but the endtext
        Text[] GuiElements = GUI.GetComponentsInChildren<Text>();
        foreach (Text GuiText in GuiElements)
            GuiText.enabled = false;
        EndText.enabled = true;
    }

    // When we set the game to running
    public void OnGameRunning()
    {
        // Hide the gui
        GUI.SetActive(false);

        // Resume the game physics
        Time.timeScale = 1f;
    }

    // When we pause the game
    public void OnGamePaused()
    {
        // Show the gui
        GUI.SetActive(true);

        Text[] GuiElements = GUI.GetComponentsInChildren<Text>();
        foreach (Text GuiText in GuiElements)
            GuiText.enabled = true;
        GameObject.Find("GameEndedMessage").GetComponent<Text>().enabled = false;

        // Pause the game physics
        Time.timeScale = 0f;
    }

	
	// Update is called once per frame
	void Update () {

        switch (CurrentGameStatus)
        {
            // If the game is paused
            case State.GAME_PAUSED:
                // If we press "p" enables the game
                if (Input.GetKeyUp(KeyCode.P))
                {
                    ChangeGameState(State.GAME_RUNNING);
                }
                break;

            // If the game ended
            case State.GAME_ENDED:
                //CountDownBetweenLevel -= Time.deltaTime;

                if ((CountDownBetweenLevel -= Time.deltaTime) < 0f)
                {
                    // If we lost on the last 
                    if (CurrentLevel == 0 || CurrentLevel == 1)
                        SceneManager.LoadScene("MainScreen");

                    // Pause the game
                    ChangeGameState(State.GAME_PAUSED);

                    // Reset the countdown
                    CountDownBetweenLevel = 3f;
                }

                break;

            // If the game is running
            case State.GAME_RUNNING:

                // If we press "p" pauses the game
                if (Input.GetKeyUp(KeyCode.P))
                {
                    ChangeGameState(State.GAME_PAUSED);
                }
                // If we press "L" we instalose
                else if (Input.GetKeyUp(KeyCode.L))
                {
                    PlayerLives = 0;
                    ChangeGameState(State.GAME_ENDED);
                }
                // If we press "W" we instawin
                else if (Input.GetKeyUp(KeyCode.W))
                {
                    ChangeGameState(State.GAME_ENDED);
                }
                // if we press "U" all the brick have power up 
                else if (Input.GetKeyUp(KeyCode.U))
                {
                    foreach (BrickComponent Brick in Bricks)
                        if (Brick != null)
                            Brick.HasPowerUp = true;
                }
                // If we run out of lives or we destroyed all the bricks the game is over
                else if (PlayerLives == 0 || BrickComponent.BrickCount == 0)
                {
                    // We are on the player ended state
                    ChangeGameState(State.GAME_ENDED);
                }

                break;
        }
	}
}
