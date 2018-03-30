using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is going to manage all the behavior of the actual pong ball movement
/// like bouncing against the walls and the paddles of the wall / other player.
/// </summary>
public class PingPongManager : MonoBehaviour {

    /// <summary> If checked the other players paddle will be controlled by the computer. </summary>
    public bool PlayAgainstComputer = false;
    /// <summary> Speed of the computer paddle. </summary>
    public float ComputerMovementSpeed = 2.0f;
    /// <summary> Movement of the paddle when the player pressed the right or left key. </summary>
    public float PaddleMovementSpeed = 1.0f;
    /// <summary> Velocity of the ball when you give the initial random vector velocity. </summary>
    public float VelocityMultiplier = 1.0f;
    /// <summary> The score counter. </summary>
    public UILabel ScoreCounter;
    /// <summary> The score. </summary>
    private int Score = 0;
    /// <summary> The window loose widget. </summary>
    public NGUIPanelFade WinLooseWidget;
    /// <summary> Message describing the window loose. </summary>
    public UILabel WinLooseMessage;
    /// <summary> The first paddle. </summary>
    public GameObject Paddle1;
    /// <summary> The second paddle. </summary>
    public GameObject Paddle2;
    /// <summary> The ball. </summary>
    public GameObject Ball;
    /// <summary> Mode of the game -1 = Offline, 1 = Client, 2 = Server. </summary>
    public int GameMode = -1;

    /// <summary> Starts this object. </summary>
    void Start()
    {
        
        // Apply a random velocity to the ball at start so the ball flys in a random direction.
        // For example were just going to send it in a predicted location so when you open the app
        // You can actually know where is going to go and hit it.
        if (GameMode != 1)
            Ball.GetComponent<Rigidbody>().velocity = new Vector3(10,10,0);

    }

    /// <summary> Handles the player paddle movement by the Left and Right Arrow keys. </summary>
    void Update ()
    {
	
        // Handles the arrow key paddle movement
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // If the gamemode is 1 then this is the client player then switch the paddles
            if (GameMode != 1)
            {
                Paddle1.GetComponent<Rigidbody>().velocity += new Vector3(-1, 0, 0) * PaddleMovementSpeed;
            }
            else
            {
                // Create a rigidbody on paddle 2
                if (Paddle2.GetComponent<Rigidbody>() == null)
                {
                    Rigidbody rb = Paddle2.AddComponent<Rigidbody>();
                    rb.drag = 6;
                    rb.useGravity = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    rb.freezeRotation = true;
                }
                Paddle2.GetComponent<Rigidbody>().velocity += new Vector3(-1, 0, 0) * PaddleMovementSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // If the gamemode is 1 then this is the client player then switch the paddles
            if (GameMode != 1)
            {
                Paddle1.GetComponent<Rigidbody>().velocity += new Vector3(1, 0, 0) * PaddleMovementSpeed;
            }
            else
            {
                // Create a rigidbody on paddle 2
                if (Paddle2.GetComponent<Rigidbody>() == null)
                {
                    Rigidbody rb = Paddle2.AddComponent<Rigidbody>();
                    rb.drag = 6;
                    rb.useGravity = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    rb.freezeRotation = true;
                }
                Paddle2.GetComponent<Rigidbody>().velocity += new Vector3(1, 0, 0) * PaddleMovementSpeed;
            }            
        }

        // If the PlayAgainstComputer is checked the computer just lerp the x position
        if (PlayAgainstComputer == true && GameMode != 1)
        {
            // If the ball is past 0.0f on the on Y axis that means its getting close to hitting the computers side.
            if (Ball.transform.position.y > 0.0f)
            {
                Paddle2.transform.position = Vector3.Lerp(Paddle2.transform.position, new Vector3(Ball.transform.position.x, Paddle2.transform.position.y, Paddle2.transform.position.z), Time.deltaTime * ComputerMovementSpeed);
            }
            // Make sure the computer paddle doesnt go beyond the screen
            if (Paddle2.transform.position.x <= -53f)
                Paddle2.transform.position = new Vector3(-53f, Paddle2.transform.position.y, Paddle2.transform.position.z);
            if (Paddle2.transform.position.x >= 53f)
                Paddle2.transform.position = new Vector3(53f, Paddle2.transform.position.y, Paddle2.transform.position.z);
        }          
    }

    /// <summary>
    /// Creates a random vector that can be scaled by the multiplied value.
    /// Using Unity's default Random class generates zeros sometimes so were goint to use the System.Random.
    /// </summary>
    ///
    /// <param name="Multiplier">   The multiplier. </param>
    ///
    /// <returns> A Vector3. </returns>
    Vector3 RandomVector3(float Multiplier)
    {
        return new Vector3(GetRandomNumber(-1f, 1f) * Multiplier, GetRandomNumber(-1, 1) * Multiplier, GetRandomNumber(-1, 1) * Multiplier);
    }
    /// <summary> Since System.Random doesn't have the actual random inbetween float we inplement on ourself. </summary>
    ///
    /// <param name="minimum">  The minimum. </param>
    /// <param name="maximum">  The maximum. </param>
    ///
    /// <returns> The random number. </returns>
    public float GetRandomNumber(double minimum, double maximum)
    {
        System.Random random = new System.Random();
        return (float)(random.NextDouble() * (maximum - minimum) + minimum);
    }


    /// <summary>
    /// If a collision between the paddle and the ball happens step the score of the player up one.
    /// This function is called by the ModularCollsionEvent component on the ball gameobject.
    /// </summary>
    public void PingPongPaddleCollision()
    {
        Score += 1;
        ScoreCounter.text = "Score: " + Score.ToString();
    }

    /// <summary>
    /// If the player fails to block the ping pong ball
    /// we set the UILabel and then fade in the UI element.
    /// </summary>
    public void PlayerLoose()
    {
        WinLooseWidget.FadeIn();
        WinLooseMessage.text = "You Loose!";
        // Stop the ball
        Ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
    /// <summary> Player window. </summary>
    public void PlayerWin()
    {
        WinLooseWidget.FadeIn();
        WinLooseMessage.text = "You Win!";
        // Stop the ball
        Ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }


}
