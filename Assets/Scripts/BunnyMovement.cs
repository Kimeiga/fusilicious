using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyMovement : MonoBehaviour {

    float max_velocity_ground = 4;

    float max_velocity_air = 2;

    float air_accelerate = 50;

    float ground_accelerate = 50;

    float friction = 8;

    float xInput;
    float yInput;

    public CharacterController controller;
    public Rigidbody rigid;

    public float jumpSpeed;
    public float moveSpeed;

    public bool grounded;

    public Vector3 velocity = Vector3.zero;
    Vector3 lastPosition;


	// Use this for initialization
	void Start () {

        rigid = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();

        lastPosition = transform.position;

	}
	
	// Update is called once per frame
	void Update () {


	}

    private void FixedUpdate()
    {


        print(velocity.magnitude);

		//velocity = rigid.velocity;


		velocity = transform.position - lastPosition;


		lastPosition = transform.position;




		float input_y = Input.GetAxis("Vertical");
		float input_x = Input.GetAxis("Horizontal");

		Vector3 desiredMove = transform.forward * input_y + transform.right * input_x;

		// get a normal for the surface that is being touched to move along it
		RaycastHit hitInfo;
        Physics.SphereCast(transform.position, controller.radius, Vector3.down, out hitInfo,
                           controller.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        

        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;


        Vector3 moveDirection = desiredMove * moveSpeed;

        //print(moveDirection);


        if(controller.isGrounded){

			if (Input.GetButton("Jump"))
			{

				moveDirection.y = jumpSpeed;

			}

            controller.Move(MoveGround(moveDirection, velocity));

            //print("ground");



        }
        else{

            //print("air");

            moveDirection += Physics.gravity * Time.fixedDeltaTime * 0.025f;



            controller.Move(MoveAir(moveDirection, velocity));



        }






    }

    // accelDir: normalized direction that the player has requested to move (taking into account the movement keys and look direction)
    // prevVelocity: The current velocity of the player, before any additional calculations
    // accelerate: The server-defined player acceleration value
    // max_velocity: The server-defined maximum player velocity (this is not strictly adhered to due to strafejumping)
    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
	{
		float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.

		float accelVel = accelerate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment

		// If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
		if (projVel + accelVel > max_velocity)
			accelVel = max_velocity - projVel;
        
		return prevVelocity + accelDir * accelVel;
	}

	private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
	{

        //print("prevVelocity before " + prevVelocity);

		// Apply Friction
		float speed = prevVelocity.magnitude;
		if (speed != 0) // To avoid divide by zero errors
		{


			float drop = speed * friction * Time.fixedDeltaTime;
			prevVelocity *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction.
		}

        //print("prevVelocity after " + prevVelocity);

		// ground_accelerate and max_velocity_ground are server-defined movement variables
		return Accelerate(accelDir, prevVelocity, ground_accelerate, max_velocity_ground);
	}

	private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
	{
		// air_accelerate and max_velocity_air are server-defined movement variables
		return Accelerate(accelDir, prevVelocity, air_accelerate, max_velocity_air);
	}
}
