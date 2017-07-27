using UnityEngine;
using System.Collections;

public class FPMovement1 : MonoBehaviour {

	[Header("Base Variables")]
	public CharacterController characterController;
	private bool canMove = true;

	[Space(10)]

	[Header("Speed Variables")]
	public float runSpeed = 7;
	public float walkSpeed = 4;
	public float crouchSpeed = 3;
	private float speed;
	private bool jumpedFromStand = false;

	[Space(10)]

	[Header("Sliding Variables")]
	// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
	public bool slideWhenOverSlopeLimit = false;
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

	private Vector3 xzDirection;
	private Vector3 moveDirection;


	[Space(10)]

	[Header("Crouching Variables")]
	//These are the crouching values that can be changed around (just make sure that crouchHeight - springOffset is not below 1)
	public float crouchHeight = 1.4f;
	public float springOffset = 0.2f;
	public float crouchingSpeed = 5f;
	public float springMod = 4;
	private float standHeight;              //You can edit this one by just changing the height of the controller in the inspector
	
	//working variables for crouching mechanism
	private bool recoilingFromLand = false;
	private float crouchingSpeedMod = 1;
	private float targetHeight;
    private float targetHeightPrevious;

    private bool crouchAux = false;


	[Space(10)]


	[Header("Changing Body During Crouch Variables")]
	public Transform meshTransform;
	private Vector3 meshTransformOriginalScale;

	public Transform[] headObjects;
	public Transform[] footObjects;



	private RaycastHit hit;
	private Vector3 contactPoint;
	private float rayDistance;
	
	
	private bool jumpButtonUp;

	private bool wallHang = false;
	private float stepOffsetInitial;
	private bool onSlope;

	private bool dontBounce = false;
	private bool dontBounceAssist = false;


	// Use this for initialization
	void Start () {

		stepOffsetInitial = characterController.stepOffset;

		slideLimit = characterController.slopeLimit - .1f;
		
		standHeight = characterController.height;
		
		characterController = GetComponent<CharacterController>();

		xzDirection = Vector3.zero;
		

		meshTransformOriginalScale = meshTransform.localScale;

        //LeanTween.value(gameObject, updateValueExampleCallback, characterController.height, crouchHeight, 1f).setEase(LeanTweenType.easeOutElastic);
	}
	
	void Update()
	{

        targetHeightPrevious = targetHeight;

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

			JumpMechanism();
			CrouchMechanism();
			MoveMechanism();

		}
		
		wallHang = false;
		jumpButtonUp = false;
		
	}

	void MoveMechanism()
	{
		if (grounded)
		{
			fellOffLedge = false;
		}

		slideDirectionMod = Mathf.Lerp(slideDirectionMod, 1, 0.1f);

		if (canMove)
		{
			xzDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		}
		else
		{
			xzDirection = Vector3.zero;
		}
		

		if (xzDirection.magnitude > 1)
		{
			xzDirection = xzDirection.normalized;
		}

		xzDirection *= speed;

		moveDirection.x = xzDirection.x;
		moveDirection.z = xzDirection.z;
		

		if (!wallHang)
		{

			moveDirection.y -= gravity * Time.deltaTime;
			characterController.stepOffset = stepOffsetInitial;
		}
		else
		{
			moveDirection.y = 0;
			characterController.stepOffset = 0.01f;
		}

		if (((characterController.collisionFlags & CollisionFlags.Above) != 0) && hitCeiling == false)
		{
			moveDirection.y = -0.4f;
			hitCeiling = true;
		}
		

		moveDirection = transform.TransformDirection(moveDirection);


		// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
		if (grounded && (((slideCounter > 2) && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide")))
		{
			Vector3 slideMoveDirection = moveDirection;
			Vector3 hitNormal = hit.normal;
			slideMoveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
			Vector3.OrthoNormalize(ref hitNormal, ref slideMoveDirection);
			slideMoveDirection *= slideSpeed;

			//slideMoveDirection.y *= 4;

			moveDirection *= 0.2f;
			moveDirection += slideMoveDirection * 0.8f * slideDirectionMod ;

			if (jumpButtonUp)
			{
				moveDirection.y = jumpSpeed*0.7f;
			}

		}
		

		
		grounded = (characterController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

		if (grounded)
		{
			hitCeiling = false;
			moveDirection.y = -antiBumpFactor;
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
				moveDirection.y = -1;
				fellOffLedge = false;
			}
		}
		
		
	}

	void JumpMechanism()
	{
		if (grounded)
		{

			jumpedFromStand = false;

			if (falling && !(slideCounter > 2) && !dontBounceAssist)
			{
				recoilingFromLand = true;
				falling = false;

				if (dontBounce)
				{
					dontBounceAssist = true;
				}
			}
				

			if (jumpButtonUp)
			{
				moveDirection.y = jumpSpeed;

				grounded = false;
				dontBounceAssist = false;

				if(!Input.GetButton("Crouch"))
				{
					jumpedFromStand = true;
				}
			}
			

		}
	}
	
	void CrouchMechanism()
	{

		//record the previous height to scale the mesh, move the head, and change the y value of the transform
		float previousHeight = characterController.height;
		
		//set the target height for the crouch command
		if (canMove&& Input.GetButton("Crouch"))
		{
			//If you are near the crouching spring height, than stop recoiling from a landing
			if(characterController.height - (crouchHeight - springOffset) < 0.05f)
			{
				recoilingFromLand = false;
			}
			
			//if you are crouching, then go to the crouch height
			targetHeight = crouchHeight;

			//if you are jumping or recoiling from a landing, go to the crouch spring height
			if (canMove && Input.GetButton("Jump") || recoilingFromLand)
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
			if (canMove && Input.GetButton("Jump") || recoilingFromLand)
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
		if (Physics.Raycast(transform.position, Vector3.up, out hit, characterController.height * 0.5f + 0.1f) && targetHeight == standHeight)
		{
			targetHeight = characterController.height;
		}




        //Lerp the controller height to the target height
        //characterController.height = iTween.FloatUpdate(characterController.height, targetHeight, crouchingSpeed * crouchingSpeedMod);


        /*
        if(targetHeightPrevious != targetHeight){

            print("O:");
            LeanTween.value(gameObject, characterController.height, targetHeight, 1);

        }
        */

        characterController.height = targetHeight;

		
		

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
		foreach(Transform t in headObjects)
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
	}

	void updateValueExampleCallback(float val, float ratio)
	{
        Debug.Log("tweened value:" + val + " percent complete:" + ratio * 100);
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		contactPoint = hit.point;
		if (onSlope && xzDirection.magnitude > 0.05f)
		{
			dontBounce = true;

		}
		else
		{
			dontBounce = false;

		}
	}

	void OnTriggerStay(Collider other)
	{

		if (other.gameObject.tag == "Level" && !grounded)
		{

			if (canMove && Input.GetButton("Wall Hang"))
			{
				wallHang = true;

			}
		}
	}

	




}
