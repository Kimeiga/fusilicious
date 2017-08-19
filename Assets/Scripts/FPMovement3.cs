using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FPMovement3 : MonoBehaviour {

    public bool gliding = false;
    float glideSpeed;
    public float driftKeyDownTime;
    public float driftTimeThresh;
    public float driftBonus = 0.02f;
    public float driftCap = 1.5f;
    public float driftDecay = 0.01f;
    public float driftRadius = 2;


    public MouseRotate bodyMouseRotate;
    public MouseRotate fireMouseRotate;
    public float turnSpeed = 2;
    Quaternion originalRotation;


    float glideSpeedNew;
    float turnSpeedNew;

	[Header("Base Variables")]
	public CharacterController characterController;
	private bool canMove = true;

	[Space(10)]

	[Header("Speed Variables")]
	public float runSpeed = 5;
	public float walkSpeed = 4;
	public float crouchSpeed = 3;
	public float speed;
	public bool jumpedFromStand = false;

	[Space(10)]

	[Header("Sliding Variables")]
	// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = true;
	// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
	public bool slideOnTaggedObjects = false;
	public float slideSpeed = 6f;
	private float slideLimit;
	private bool sliding = false;
	public int slideCounter = 0;

	private float slideDirectionMod = 1;


	[Space(10)]


	[Header("Jumping Variables")]
	public float jumpSpeed = 8;
	private bool falling = false;
	
	
	private bool hitCeiling = false;
	public float gravity = 20;
	private float antiBumpFactor = .75f;
	private bool fellOffLedge = false;

	private bool grounded = false;

	private Vector3 wishDir;

	private Vector3 moveDir;


	[Space(10)]

	[Header("Crouching Variables")]
	//These are the crouching values that can be changed around (just make sure that crouchHeight - springOffset is not below 1)
	public float crouchHeight = 1.4f;
	public float springOffset = 0.2f;
	public float crouchingSpeed = 0.2f;
	public float springMod = 2;
	private float standHeight; //You can edit this one by just changing the height of the controller in the inspector
	
	//working variables for crouching mechanism
	private bool recoilingFromLand = false;
	private float crouchingSpeedMod = 1;
	private float targetHeight;


	[Space(10)]


	[Header("Changing Body During Crouch Variables")]
	public Transform meshTransform;
	private Vector3 meshTransformOriginalScale;

	public Transform[] headObjects;
	public Transform[] footObjects;



	private RaycastHit hit;
	private Vector3 contactPoint;
	private float rayDistance;
    public LayerMask notBodyPartAlive;
	
	
	private bool jumpButtonUp;

	private bool wallHang = false;
	private float stepOffsetInitial;
	private bool onSlope;

	private bool dontBounce = false;
	private bool dontBounceAssist = false;


	[Space(10)]

	[Header("Measurements")]
	public Vector3 measuredDisplacement;
	public float measuredSpeed;
	private Vector3 lastPosition;


	// Use this for initialization
	void Start () {


        //initialize last position
        lastPosition = Vector3.zero;

        characterController = GetComponent<CharacterController>();

		stepOffsetInitial = characterController.stepOffset;

		slideLimit = characterController.slopeLimit - .1f;
		
		standHeight = characterController.height;
		
		

		wishDir = Vector3.zero;
		

		meshTransformOriginalScale = meshTransform.localScale;

	}

    void Update()
    {

        if (Input.GetButtonDown("Glide")) {
            if (gliding == false) {

                gliding = true;

                //bodyMouseRotate.canRotate = false;
                bodyMouseRotate.enabled = false;

                fireMouseRotate.axes = MouseRotate.RotationAxes.MouseXAndY;


                originalRotation = transform.localRotation;

                glideSpeed = speed;

            }
            else {

                gliding = false;
                //bodyMouseRotate.canRotate = true;
                bodyMouseRotate.enabled = true;
                bodyMouseRotate.originalRotation = transform.localRotation;
                bodyMouseRotate.rotationX = fireMouseRotate.rotationX + fireMouseRotate.turnOffset;

                fireMouseRotate.axes = MouseRotate.RotationAxes.MouseY;
                fireMouseRotate.rotationX = 0;
                fireMouseRotate.turnOffset = 0;
            }

        }

        if (gliding)
        {
            if (Input.GetButtonDown("Left"))
            {
                driftKeyDownTime = Time.time;
            }

            if (Input.GetButtonDown("Right"))
            {
                driftKeyDownTime = Time.time;
            }

            if (Input.GetButtonUp("Left"))
            {

                if (!Input.GetButton("Right"))
                {
                    driftKeyDownTime = -1; //this is an impossible value for Time.time
                    //I am using this to save on variables; this way driftkeydowntime can be a boolean and a float hahahah
                }
            }

            if (Input.GetButtonUp("Right"))
            {

                if (!Input.GetButton("Left"))
                {
                    driftKeyDownTime = -1; //this is an impossible value for Time.time
                    //I am using this to save on variables; this way driftkeydowntime can be a boolean and a float hahahah
                }
            }
        }

        if(grounded){
            jumpedFromStand = false;
        }


        if ((canMove && Input.GetButton("Crouch") && !jumpedFromStand) 
            || (characterController.collisionFlags & CollisionFlags.Above) != 0 && characterController.height < standHeight)
		{
			speed = crouchSpeed;
		}
		else if (canMove && Input.GetButton("Walk"))
		{
			speed = walkSpeed;
		}
		else
		{
			speed = runSpeed;
		}


		rayDistance = characterController.height * 0.5f + characterController.radius;

		if (canMove && Input.GetButtonUp("Jump"))
		{
			jumpButtonUp = true;
		}
	}

	void FixedUpdate() {

        //sliding mechanic
		if (grounded)
		{
			sliding = false;
			// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
			// because that interferes with step climbing amongst other annoyances
			if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance))
			{
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
				{
					sliding = true;
					grounded = true;
				}

				if (Vector3.Angle(hit.normal, Vector3.up) > 0.1f)
				{
					onSlope = true;
				}
				else
				{
					onSlope = false;
				}

			}
			// However, just raycasting straight down from the center can fail when on steep slopes
			// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
			else
			{
				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
				{
					sliding = true;
					grounded = true;
				}
			}
		}

		if (sliding)
		{
			slideCounter++;

		}
		else
		{
			slideCounter = 0;
		}


		if (canMove)
		{

            if(grounded){
				if (falling && !(slideCounter > 2) && !dontBounceAssist)
				{
					recoilingFromLand = true;
					falling = false;

					if (dontBounce)
					{
						dontBounceAssist = true;
					}
				}
            }
			

			//record the previous height to scale the mesh, move the head, and change the y value of the transform
			float previousHeight = characterController.height;

			//set the target height for the crouch command
			if (Input.GetButton("Crouch"))
			{
				//If you are near the crouching spring height, than stop recoiling from a landing
				if (characterController.height - (crouchHeight - springOffset) < 0.05f)
				{
					recoilingFromLand = false;
				}

				//if you are crouching, then go to the crouch height
				targetHeight = crouchHeight;

				//if you are jumping or recoiling from a landing, go to the crouch spring height
				if (Input.GetButton("Jump") || recoilingFromLand)
				{
					targetHeight = crouchHeight - springOffset;
				}
			}
			else
			{
				//If you are near the spring height, than stop recoiling from a landing
				if (characterController.height - (standHeight - springOffset) < 0.05f)
				{
					recoilingFromLand = false;
				}

				//if you are just standing, then go to the stand height
				targetHeight = standHeight;

				//if you are jumping or recoiling from a landing, go to the spring height
				if (Input.GetButton("Jump") || recoilingFromLand)
				{
					targetHeight = standHeight - springOffset;
				}
			}


			//If you are recoiling from a landing, than crouch faster
			if (recoilingFromLand)
			{
				crouchingSpeedMod = springMod;
			}
			else
			{
				crouchingSpeedMod = 1;
			}


			//prevent increasing in height if there is something over my head
			if ((characterController.collisionFlags & CollisionFlags.Above) != 0 && targetHeight == standHeight)
			{
				targetHeight = characterController.height;
			}
			//prevent increasing in height if there is something over my head
			if (Physics.Raycast(transform.position, Vector3.up, out hit, characterController.height * 0.5f + 0.1f, notBodyPartAlive) && targetHeight == standHeight)
			{
				targetHeight = characterController.height;
			}





			//Lerp the controller height to the target height

			if (Mathf.Abs(characterController.height - targetHeight) > 0.0001f)
			{

				characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchingSpeed * crouchingSpeedMod);
			}





			//Change Y Value of Transform with change in controller height
			float changeInHeightHalfed = (characterController.height - previousHeight) * 0.5f; //these two variables will be used for the next three sections
			Vector3 temp = transform.position;
			if (grounded)
			{
				temp.y += changeInHeightHalfed;
			}
			else
			{
				temp.y -= changeInHeightHalfed;
			}
			if (!float.IsNaN(temp.y))
			{
				transform.position = temp;
			}



			//Change position of each "head object" with change in controller height such that it stays by the "head"
			foreach (Transform t in headObjects)
			{
				temp = t.position;
				temp.y += changeInHeightHalfed;
				t.position = temp;
			}

			//Change position of each "foot object" with change in controller height such that it stays by the "foot"
			foreach (Transform t in footObjects)
			{
				temp = t.position;
				temp.y -= changeInHeightHalfed;
				t.position = temp;
			}


			//Change scale of character mesh with change in controller height
			temp = new Vector3(meshTransformOriginalScale.x, meshTransformOriginalScale.y * characterController.height / standHeight, meshTransformOriginalScale.z);
			if (!float.IsNaN(meshTransformOriginalScale.y * characterController.height / standHeight))
			{
				meshTransform.localScale = temp;
			}









			if (grounded)
			{
				fellOffLedge = false;
			}

			slideDirectionMod = Mathf.Lerp(slideDirectionMod, 1, 0.1f);

            if (!gliding)
            {
                //get wish direction of movement from inputs
                wishDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

				if (wishDir.magnitude > 1)
				{
					wishDir = wishDir.normalized;
				}



				moveDir.x = wishDir.x * speed;
				moveDir.z = wishDir.z * speed;


			}
            else{

                //rotate wishdir of movement with horizontal keys





				turnSpeedNew = turnSpeed;

                float driftRadiusMod = 1;

				if (Input.GetButton("Tuck"))
				{
					driftRadiusMod = 1.3f;
				}
				else if (Input.GetButton("Carve"))
				{
                    driftRadiusMod = 0.7f;
				}

                if(driftKeyDownTime != -1 && Time.time > driftKeyDownTime + driftTimeThresh)
                {
                    glideSpeed += driftBonus * Mathf.Abs(Input.GetAxis("Horizontal")) / driftRadiusMod;
                }
                


                if(Input.GetAxis("Horizontal") == 0){
                    glideSpeed -= driftDecay;
                }


                glideSpeed = Mathf.Clamp(glideSpeed, runSpeed, driftCap * runSpeed);

                float turnMax = glideSpeed / (driftRadius * driftRadiusMod);


                float turn = Input.GetAxis("Horizontal") * turnMax;

                float turnDeg = turn;

                print(glideSpeed);

				

                wishDir = Quaternion.AngleAxis(turnDeg, Vector3.up) * wishDir;





                transform.localRotation = Quaternion.AngleAxis(turnDeg, Vector3.up) * transform.localRotation;

                fireMouseRotate.turnOffset -= turnDeg;

                //Quaternion lookAtDir = Quaternion.LookRotation(transform.TransformDirection(wishDir));

                //transform.rotation = lookAtDir;


                //bodyMouseRotate.glideTurn += turn;

				



                moveDir.x = wishDir.x * glideSpeed;
                moveDir.z = wishDir.z * glideSpeed;

			}







			if (!wallHang)
			{
				moveDir.y -= gravity * Time.fixedDeltaTime;
				characterController.stepOffset = stepOffsetInitial;
			}
			else
			{
				moveDir.y = 0;
				characterController.stepOffset = 0.01f;
			}

			if (grounded)
			{
				if (jumpButtonUp)
				{
					moveDir.y = jumpSpeed;
					grounded = false;
					dontBounceAssist = false;
                    jumpedFromStand = !Input.GetButton("Crouch");
				}
			}


			if (((characterController.collisionFlags & CollisionFlags.Above) != 0) && hitCeiling == false)
			{
				moveDir.y = -0.4f;
				hitCeiling = true;
			}


			moveDir = transform.TransformDirection(moveDir);


			// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
			if (grounded && (((slideCounter > 2) && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide")))
			{
				Vector3 slideMoveDirection = moveDir;
				Vector3 hitNormal = hit.normal;
				slideMoveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
				Vector3.OrthoNormalize(ref hitNormal, ref slideMoveDirection);
				slideMoveDirection *= slideSpeed;


				moveDir *= 0.2f;
				moveDir += slideMoveDirection * 0.8f * slideDirectionMod;

				if (jumpButtonUp)
				{
					moveDir.y = jumpSpeed * 0.7f;
				}

			}



			grounded = (characterController.Move(moveDir * Time.fixedDeltaTime) & CollisionFlags.Below) != 0;

			if (grounded)
			{
				hitCeiling = false;
				moveDir.y = -antiBumpFactor;
				if (falling && !sliding)
				{
					slideDirectionMod = 0.5f;
				}
			}
			else
			{
				falling = true;
				if (fellOffLedge && !falling && !(slideCounter > 2))
				{
					moveDir.y = -1;
					fellOffLedge = false;
				}
			}

		}
		
		wallHang = false;
		jumpButtonUp = false;


		measuredDisplacement = transform.position - lastPosition;
		measuredSpeed = measuredDisplacement.magnitude;

        //set lastposition for displacement/speed calculation
        lastPosition = transform.position;
		
	}


	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		contactPoint = hit.point;
		if (onSlope && wishDir.magnitude > 0.05f)
		{
			dontBounce = true;
		}
		else
		{
			dontBounce = false;

		}


        if (gliding && (characterController.collisionFlags & CollisionFlags.Sides) != 0 && hit.gameObject.tag == "Level")
        {
            glideSpeed = runSpeed;
        }

    }

	void OnTriggerStay(Collider other)
	{

		if (other.gameObject.tag == "Level")
		{

            if (!grounded && canMove && Input.GetButton("Wall Hang")){
                wallHang = true;

			}


        }
	}



}
