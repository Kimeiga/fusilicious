using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    //references
    private Item itemScript;
    private AudioSource fireAudioSource;
    public Transform fireTransform;
    public MouseRotate fireTransformRotate;
    public MouseRotate bodyTransformRotate;
    public MouseRotate lookTransformRotate;
    public Sway handsSway;


    //gun stats
    public int maxAmmo;
    public int ammo;

    public bool automatic;
    private bool fireCommand;
    public bool firing = false;
    public bool firingAux = false;

    public float fireRate = 0.02f;
    private float nextFire;

    public float range = 1000;

    public float damage = 20;


    public float scaleMod = 3;
    public float minScaleMod = 0.5f;
    public float maxScaleMod = 3;

    //firing variables
    private RaycastHit hit;
    public LayerMask fireMask;
    public GameObject bullet;


    //we're going to do automatic fire first ok?
    public AnimationCurve xRecoil;
    public AnimationCurve yRecoil;

    public float currentRecoil;
    public float currentRecoilIncrement;

    public float recoilModX = 3;
    public float recoilModY = 3;

    public float xKick;
    public float yKick;

    public float kickTween;

    public float kickMod = 2;

    public float xRecoilOffset; //smooth recoil pattern from the whole spray
    public float yRecoilOffset;

    public float recoilTween;

    public float xKickOffset; //oneshot kicks from each bullet
    public float yKickOffset;

    public AnimationCurve kickIntensity;

    public float kickback;
    public float kickbackAcc;


    // Use this for initialization
    void Start()
    {
        //initialize references
        itemScript = GetComponent<Item>();
        fireAudioSource = GetComponent<AudioSource>();


        //initialize numbers
        nextFire = 0;
        ammo = maxAmmo;


        currentRecoil = 0;

        //Calculate current recoil increment
        currentRecoilIncrement = (1.0f / maxAmmo);

    }

    // Update is called once per frame
    void Update()
    {

        if (itemScript.active)
        {

			//cue fire command depending on the the automatic fire bool
			if (automatic)
			{
				fireCommand = Input.GetButton("Fire");
			}
			if (!automatic)
			{
				fireCommand = Input.GetButtonDown("Fire");
			}


            currentRecoil = Mathf.Clamp(currentRecoil, 0, 1);


            if (fireCommand)
            {
                if(ammo > 0){

                    firing = true;
                    firingAux = true;

					if (Time.time > nextFire)
					{

						Vector3 recoilOffset = new Vector3(xRecoilOffset * 0.5f, yRecoilOffset * 0.5f, 0);
						

                        Quaternion xQuaternion = Quaternion.AngleAxis(recoilOffset.x, Vector3.up);
                        Quaternion yQuaternion = Quaternion.AngleAxis(recoilOffset.y, -Vector3.right);

                        Quaternion shotDirection = fireTransform.rotation * xQuaternion * yQuaternion;


						//fire
                        if (Physics.Raycast(fireTransform.position, shotDirection * Vector3.forward, out hit, range, fireMask))
						{
							//you can use this for reflecting guns soon! :D
							//Vector3 incomingVec = hit.point - transform.position;
							//Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
							//Debug.DrawLine(transform.position, hit.point, Color.red);
							//Debug.DrawRay(hit.point, reflectVec, Color.green);


							Quaternion rot = Quaternion.LookRotation(-fireTransform.forward);

							GameObject bul = (GameObject)Instantiate(bullet, hit.point, rot);

							//float bulletScaleMod = hit.distance * scaleMod;
							//bulletScaleMod = Mathf.Clamp(bulletScaleMod, minScaleMod, maxScaleMod);

							//bul.transform.localScale *= bulletScaleMod;
							////bul.transform.parent = hit.transform;




							Vector3 prevScale = bul.transform.lossyScale;
							Vector3 hitObjectScale = hit.transform.lossyScale;
							Vector3 newScale = new Vector3(prevScale.x / hitObjectScale.x, prevScale.y / hitObjectScale.y, prevScale.z / hitObjectScale.z);



							print(prevScale + "\n" + hitObjectScale + "\n" + newScale);
							//bul.transform.SetParent(hit.transform);
							//bul.transform.localScale = newScale;



							//bul.transform.rotation = rot;


						}
						nextFire = Time.time + fireRate;

						//fireAudioSource.PlayOneShot(fireAudioSource.clip);

						ammo--;

                        currentRecoil += currentRecoilIncrement;

                        LeanTween.cancelAll();

                        xRecoilOffset += xRecoil.Evaluate(currentRecoil) * recoilModX;
                        yRecoilOffset += yRecoil.Evaluate(currentRecoil) * recoilModY;

                        //Vector3 recoilDer = new Vector3(xRecoil.Evaluate(currentRecoil) * recoilModX, yRecoil.Evaluate(currentRecoil) * recoilModY, 0);

                        ////recoilDer = recoilDer.normalized; //the normalize instantaneous direction of the recoil at any time

                        //recoilDer *= kickMod;

                        //xKick = recoilDer.x;
                        //yKick = recoilDer.y;

                        Vector3 kickDir = new Vector3(xRecoilOffset, yRecoilOffset, 0);
                        kickDir = kickDir.normalized;
                        kickDir *= kickMod;
                        kickDir *= kickIntensity.Evaluate(currentRecoil);

                        xKick = kickDir.x;
                        yKick = kickDir.y;

                        kickbackAcc -= kickback;


					}

                }
                else{
                    //gun is empty
                    //play empty sound
                    firing = false;
                }

            }
            else{

                firing = false;
            }




            if (firing == false && firingAux == true){

                currentRecoil = 0;

				LeanTween.value(gameObject, yRecoilOffset, 0, recoilTween).setEase(LeanTweenType.easeOutQuint).setOnUpdate((float val) => { yRecoilOffset = val; });
                LeanTween.value(gameObject, xRecoilOffset, 0, recoilTween).setEase(LeanTweenType.easeOutQuint).setOnUpdate((float val) => { xRecoilOffset = val; });

                firingAux = false;
            }



            //LeanTween.value(gameObject, xKick, 0, 0.3f).setOnUpdate((float val) => { xKick = val; });
            //LeanTween.value(gameObject, yKick, 0, 0.3f).setOnUpdate((float val) => { yKick = val; });



            bodyTransformRotate.xOffset = xRecoilOffset / 2;
            fireTransformRotate.yOffset = yRecoilOffset / 2;

            lookTransformRotate.xOffset = xKick;
            lookTransformRotate.yOffset = yKick;


            handsSway.zOffset = kickbackAcc;

			//xKick = Mathf.Lerp(xKick, 0, kickLerp);
			//yKick = Mathf.Lerp(yKick, 0, kickLerp);

            LeanTween.value(gameObject, xKick, 0, kickTween).setOnUpdate((float val) => { xKick = val; });
            LeanTween.value(gameObject, yKick, 0, kickTween).setOnUpdate((float val) => { yKick = val; });

            LeanTween.value(gameObject, kickbackAcc, 0, kickTween).setOnUpdate((float val) => { kickbackAcc = val; });



            if(Input.GetButtonDown("Reload")){
                ammo = maxAmmo;
            }



        }


    }



}
