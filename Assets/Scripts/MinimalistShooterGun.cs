using UnityEngine;
using System.Collections;

public class MinimalistShooterGun : MonoBehaviour
{

	//this is attached to every gun and has its statistics altered between guns.



	//* Gun Statistics and Settings *

	//for the weapon sway script
	public Vector3 originalPosition;

	//is the gun fully automatic or semi automatic?
	public bool automatic;

	//to switch between auto and semi
	public bool fireCommand;

	//can the gun fire?
	public bool canShoot = true;

	//size of the magazine
	public int startingAmmo;

	//number of rounds left
	public int ammo;

	//damage inflicted per shot
	public float damage;

	//fire rate of weapon
	public float fireRate = 0.1f;

	//temporary variable for firing
	private float nextfire;

	//speed of character when using this gun
	public float mobility;


	//* Shooting Mechanics *

	//time when a bullet was shot (to lerp accuracyModifierShoot back to 0)
	private float timeOfShot;

	//range of the raycast
	public float range = 1000;

	private Ray shot;
	private RaycastHit hit;

	//Accuracy Randomization
	private Vector3 randomSphere;
	private Vector2 randomCircle;
	private Vector3 shotDirection;

	//actual accuracy and its global minimum. Maybe you don't need maximum if the others have maximums
	public float accuracy;
	public float accuracyMinimum;

	//amount accuracy increases per shot
	public float accuracyIncrement;
	public float accuracyRecoveryModifier;

	//part of accuracy that is affected by shooting
	public float accuracyComponentShoot;
	public float accuracyComponentShootModifier = 1;
	public float accuracyComponentShootMaximum;

	//part of accuracy that is affected by player's speed
	public float accuracyComponentMove = 0;
	public float accuracyComponentMoveModifier = 1;
	public float accuracyComponentMoveMaximum;
	//public SpeedCalculator speedCalculatorScript;

	//to debug recoil patterns
	public bool perfectAccuracy;




	//* Recoil Mechanics *

	public AnimationCurve horizontalRecoil;
	public AnimationCurve verticalRecoil;
	public float horizontalRecoilModifier;
	public float verticalRecoilModifier;
	public float currentRecoil;

	public float currentRecoilIncrement;
	public Vector3 recoilRotation;
	public Vector3 startingRecoil;

	//for semi auto recoil
	public bool fire;
	public float semiAutomaticRecoilX;
	public float semiAutomaticRecoilY;
	public float recoilRecoveryModifier;


	//for mouselooks
	public float recoilXLast;
	public float recoilXChange;
	public float recoilYLast;
	public float recoilYChange;


	//to tell when firing is in progress and you must advance the recoil pattern
	public bool firing;

	//aim kick mechanic
	public float aimKickModifier = 0.1f;



	//* Transforms and Prefabs *

	//this is for animation stuff:
	public Transform geometryTransform;

	//how far the gun goes back when you fire
	public float kickbackAmount;

	//this is a messenger variable to transfer the animation to the transform
	public Vector3 gunOffset;

	//this is the t transform that will cast the bullets and get affected by recoil
	//in my game, this is just going to be the head rotation;
	public Transform shootTransform;


	//used to calculate the the crosshair size
	public Camera playerCamera;

	//temporary bullet placeholder for marking shots
	public GameObject bullet;


	//* Ammo Plate and Health Plate


	public GameObject[] numeral1Numbers;
	public GameObject[] numeral2Numbers;

	public int tensDigit;
	public int unitsDigit;
	public int lastTensDigit;
	public int lastUnitsDigit;


	//* Temporary Variables *
	private Vector3 v1;
	private Vector3 v2;
	public LayerMask shotLayerMask;
	public float crosshairRadius;


	// Use this for initialization
	void Start()
	{

		ammo = startingAmmo;

		//set gunOffset
		gunOffset = Vector3.zero;


		if (automatic)
		{

			startingRecoil = new Vector3(horizontalRecoil.Evaluate(0), verticalRecoil.Evaluate(0), 0);
		}
		else
		{
			startingRecoil = Vector3.zero;
		}

		recoilRotation = startingRecoil;


		//Calculate current recoil increment
		currentRecoilIncrement = (1.0f / startingAmmo);


		//Calculate crosshair radius
		Vector3 dir0 = shootTransform.forward + shootTransform.right * accuracyMinimum;

		RaycastHit hit0;

		if (Physics.Raycast(shootTransform.position, dir0, out hit0, 1000, shotLayerMask))
		{
			v1 = hit0.point;
		}

		RaycastHit hit1;
		if (Physics.Raycast(shootTransform.position, shootTransform.forward, out hit1, 1000, shotLayerMask))
		{
			v2 = hit1.point;
		}


		v1 = playerCamera.WorldToScreenPoint(v1);
		v2 = playerCamera.WorldToScreenPoint(v2);
		crosshairRadius = Vector3.Distance(v1, v2);

	}

	void Update()
	{

		//may need to use a time.deltatime in some of these lerps!!!!!!!!

		//set last digits for ammo counter


		lastTensDigit = tensDigit;
		lastUnitsDigit = unitsDigit;


		tensDigit = ammo / 10;
		unitsDigit = ammo % 10;



		recoilXLast = recoilRotation.x;
		recoilYLast = recoilRotation.y;


		if (!perfectAccuracy)
		{

			//Clamp accuracy components
			accuracyComponentMove = Mathf.Clamp(accuracyComponentMove, 0, accuracyComponentMoveMaximum);
			accuracyComponentShoot = Mathf.Clamp(accuracyComponentShoot, 0, accuracyComponentShootMaximum);




			//bringing in the accuracy Move component
			//accuracyComponentMove = speedCalculatorScript.measuredSpeed;

			//Scale accuracy components
			accuracyComponentMove *= accuracyComponentMoveModifier;
			accuracyComponentShoot *= accuracyComponentShootModifier;

			//Gradually lerp accuracyComponentShoot back to 0
			accuracyComponentShoot = Mathf.Lerp(accuracyComponentShoot, 0, (Time.time - timeOfShot) * accuracyRecoveryModifier);

			//ACCURACY CORE
			accuracy = accuracyMinimum + accuracyComponentShoot + accuracyComponentMove;

		}
		else
		{
			accuracy = 0.0f;
		}


		//Animation stuff for shooting
		gunOffset.z = Mathf.Lerp(gunOffset.z, 0, (Time.time - timeOfShot) * accuracyRecoveryModifier);
		geometryTransform.localPosition = gunOffset;



		//Clamp recoil pattern
		currentRecoil = Mathf.Clamp(currentRecoil, 0, 1);

		if (automatic)
		{
			fireCommand = Input.GetButton("Fire");


			//let me try recoil patterns for automatic fire first
			if (firing)
			{



				//attempt 2
				recoilRotation += new Vector3(horizontalRecoil.Evaluate(currentRecoil) * horizontalRecoilModifier,
				  verticalRecoil.Evaluate(currentRecoil) * verticalRecoilModifier, 0);

				//recoil reset in fixed update

			}
			else
			{


				currentRecoil = Mathf.Lerp(currentRecoil, 0, (Time.time - timeOfShot) * accuracyRecoveryModifier);
				recoilRotation = Vector3.Lerp(recoilRotation, startingRecoil, (Time.time - timeOfShot) * accuracyRecoveryModifier);


			}






		}
		else
		{

			fireCommand = Input.GetButtonDown("Fire");

			//for semi automatic weapons, there will be one recoil that is used for every bullet. so no need for currentRecoil
			//the difference is also that you wouldnt use "firing" but rather, "fire"

			if (fire)
			{

				recoilRotation += new Vector3(semiAutomaticRecoilX, semiAutomaticRecoilY, 0);



			}
			else
			{

				recoilRotation = Vector3.Lerp(recoilRotation, startingRecoil, (Time.time - timeOfShot) * recoilRecoveryModifier);

				//recoil reset in fixedupdate

			}

		}



		playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, Quaternion.identity, (Time.time - timeOfShot) / fireRate);



		if (Input.GetButton("Fire") && ammo > 0 && canShoot == true)
		{
			firing = true;
		}
		else
		{
			firing = false;
		}

		//to rotate the head
		recoilXChange = (recoilRotation.x - recoilXLast);
		recoilYChange = (recoilRotation.y - recoilYLast);

		//Firing mechanic
		if (fireCommand && ammo > 0 && canShoot == true && Time.time > nextfire)
		{


			//kickback gun (animation)
			gunOffset.z -= kickbackAmount;



			//set fire
			fire = true;

			//set timeOfShot
			timeOfShot = Time.time;

			//Calculate Inaccuracy
			randomSphere = Random.insideUnitSphere * accuracy;
			randomCircle = new Vector2(randomSphere.x, randomSphere.y);

			//Calculate final path of bullet with inaccuracy and recoil
			shotDirection = shootTransform.forward + shootTransform.right * randomCircle.x
				+ shootTransform.up * randomCircle.y;

			//Cast ray
			shot = new Ray(shootTransform.position, shotDirection);

			//Detect if ray hits something and place a bullet there for placeholder
			if (Physics.Raycast(shot, out hit, range))
			{

				//put a ball there!
				Instantiate(bullet, hit.point, Quaternion.identity);
			}

			//use a bullet
			ammo -= 1;

			//set nextFire
			nextfire = Time.time + fireRate;


			if (!perfectAccuracy)
			{

				//Increment accuracy
				accuracyComponentShoot += accuracyIncrement;
			}

			//Increment recoil wait lemme try a different way
			currentRecoil += currentRecoilIncrement;

			//aim kick mechanic




			Quaternion xQuaternion = Quaternion.AngleAxis(recoilRotation.normalized.x * aimKickModifier, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis(recoilRotation.normalized.y * aimKickModifier, -Vector3.right);

			playerCamera.transform.localRotation = Quaternion.identity * xQuaternion * yQuaternion;
		}
		else
		{

			fire = false;

		}



		//for time being, I will have reload
		if (Input.GetButtonDown("Reload"))
		{
			ammo = startingAmmo;
		}



		//AMMO COUNTER :D

		if (tensDigit != lastTensDigit)
		{

			//change the number!!
			numeral1Numbers[lastTensDigit].SetActive(false);
			numeral1Numbers[tensDigit].SetActive(true);


		}

		if (unitsDigit != lastUnitsDigit)
		{

			//change the number!!
			numeral2Numbers[lastUnitsDigit].SetActive(false);
			numeral2Numbers[unitsDigit].SetActive(true);


		}

	}


}