using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class is going to manage all the behavior of the actual pong ball movement
// like bouncing against the walls and the paddles of the wall / other player.
public class PingPongManager : MonoBehaviour {

    
    // Movement of the paddle when the player pressed the right or left key.
    public float PaddleMovementSpeed = 1.0f;
    // Velocity of the ball when you give the initial random vector velocity.
    public float VelocityMultiplier = 1.0f;
    public UILabel ScoreCounter;
    private int Score = 0;
    public GameObject Paddle1;
    public GameObject Paddle2;
    public GameObject Ball;

    void Start()
    {
        
        // Apply a random velocity to the ball at start so the ball flys in a random direction.
        Ball.GetComponent<Rigidbody>().velocity = RandomVector3(VelocityMultiplier);

    }

    // Handles the player paddle movement by the Left and Right Arrow keys.
    void Update ()
    {
	
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Paddle1.GetComponent<Rigidbody>().velocity += new Vector3(-1, 0, 0) * PaddleMovementSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Paddle1.GetComponent<Rigidbody>().velocity += new Vector3(1, 0, 0) * PaddleMovementSpeed;
        }

    }

    // Creates a random vector that can be scaled by the multiplied value.
    Vector3 RandomVector3(float Multiplier)
    {
        return new Vector3(Random.RandomRange(-1, 1) * Multiplier, Random.RandomRange(-1, 1) * Multiplier, Random.RandomRange(-1, 1) * Multiplier);
    }


}
