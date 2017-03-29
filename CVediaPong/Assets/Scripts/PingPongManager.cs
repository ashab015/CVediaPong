using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class is going to manage all the behavior of the actual pong ball movement
// like bouncing against the walls and the paddles of the wall / other player.
public class PingPongManager : MonoBehaviour {

    // If checked the other players paddle will be controlled by the computer
    public bool PlayAgainstComputer = false;
    // Speed of the computer paddle
    public float ComputerMovementSpeed = 2.0f;
    // Movement of the paddle when the player pressed the right or left key.
    public float PaddleMovementSpeed = 1.0f;
    // Velocity of the ball when you give the initial random vector velocity.
    public float VelocityMultiplier = 1.0f;
    public UILabel ScoreCounter;
    private int Score = 0;
    public NGUIPanelFade WinLooseWidget;
    public UILabel WinLooseMessage;
    public GameObject Paddle1;
    public GameObject Paddle2;
    public GameObject Ball;
    // Mode of the game -1 = Offline, 1 = Client, 2 = Server
    public int GameMode = -1;

    void Start()
    {
        
        // Apply a random velocity to the ball at start so the ball flys in a random direction.
        // For example were just going to send it in a predicted location so when you open the app
        // You can actually know where is going to go and hit it.
        if (GameMode != 1)
            Ball.GetComponent<Rigidbody>().velocity = new Vector3(10,10,0);

    }

    // Handles the player paddle movement by the Left and Right Arrow keys.
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

    // Creates a random vector that can be scaled by the multiplied value.
    // Using Unity's default Random class generates zeros sometimes so were goint to use the System.Random
    Vector3 RandomVector3(float Multiplier)
    {
        return new Vector3(GetRandomNumber(-1f, 1f) * Multiplier, GetRandomNumber(-1, 1) * Multiplier, GetRandomNumber(-1, 1) * Multiplier);
    }
    // Since System.Random doesn't have the actual random inbetween float we inplement on ourself.
    public float GetRandomNumber(double minimum, double maximum)
    {
        System.Random random = new System.Random();
        return (float)(random.NextDouble() * (maximum - minimum) + minimum);
    }


    // If a collision between the paddle and the ball happens step the score of the player up one.
    // This function is called by the ModularCollsionEvent component on the ball gameobject.
    public void PingPongPaddleCollision()
    {
        Score += 1;
        ScoreCounter.text = "Score: " + Score.ToString();
    }

    // If the player fails to block the ping pong ball 
    // we set the UILabel and then fade in the UI element.
    public void PlayerLoose()
    {
        WinLooseWidget.FadeIn();
        WinLooseMessage.text = "You Loose!";
        // Stop the ball
        Ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
    public void PlayerWin()
    {
        WinLooseWidget.FadeIn();
        WinLooseMessage.text = "You Win!";
        // Stop the ball
        Ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }


}
