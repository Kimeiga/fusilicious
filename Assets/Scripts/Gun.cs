using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    //references
    private Item itemScript;
    private AudioSource fireAudioSource;
    public Transform fireTransform;
    public Transform bodyTransform;
    public MouseRotate fireTransformRotate;
    public MouseRotate bodyTransformRotate;


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

    public float xKick = 10;
    public float yKick = 10;

    public float xOffset;
    public float yOffset;

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
						//fire
						if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, range, fireMask))
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

                        //xOffset += xKick;
                        //yOffset += yKick;

                        xOffset += xRecoil.Evaluate(currentRecoil) * recoilModX;
                        yOffset += yRecoil.Evaluate(currentRecoil) * recoilModY;

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

				LeanTween.value(gameObject, xOffset, 0, 0.9f).setEase(LeanTweenType.easeOutQuint).setOnUpdate((float val) => { xOffset = val; });
				LeanTween.value(gameObject, yOffset, 0, 0.9f).setEase(LeanTweenType.easeOutQuint).setOnUpdate((float val) => { yOffset = val; });

                firingAux = false;
            }






			bodyTransformRotate.xOffset = xOffset;
			fireTransformRotate.yOffset = yOffset;






        }


    }



}
