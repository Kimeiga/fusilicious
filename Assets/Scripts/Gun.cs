using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    //references
    private Item itemScript;
    private AudioSource fireAudioSource;
    public Transform fireTransform;


    //gun stats
    public int maxAmmo;
    public int ammo;

    public bool automatic;
    private bool fireCommand;

    public float fireRate = 0.02f;
    private float nextFire;

    public float range = 1000;

    public float damage = 20;


    //firing variables
    private RaycastHit hit;
    public LayerMask fireMask;
    public GameObject bullet;




    // Use this for initialization
    void Start()
    {
        //initialize references
        itemScript = GetComponent<Item>();
        fireAudioSource = GetComponent<AudioSource>();


        //initialize numbers
        nextFire = 0;
        ammo = maxAmmo;


    }

    // Update is called once per frame
    void Update()
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



        if (itemScript.active)
        {

            if (fireCommand)
            {
                if (Time.time > nextFire && ammo > 0)
                {



					//fire
					if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, range, fireMask))
                    {


	                    Vector3 incomingVec = hit.point - transform.position;
	                    Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
	                    Debug.DrawLine(transform.position, hit.point, Color.red);
	                    Debug.DrawRay(hit.point, reflectVec, Color.green);

                        Quaternion rot = Quaternion.LookRotation(hit.normal);

                        GameObject bul = (GameObject)Instantiate(bullet, hit.point, rot);
                        bul.transform.parent = hit.transform;

                    }
                    nextFire = Time.time + fireRate;

                    //fireAudioSource.PlayOneShot(fireAudioSource.clip);

                    ammo--;

                    print("Fired");
                }
            }



        }


    }



}
