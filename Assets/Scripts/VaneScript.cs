using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaneScript : MonoBehaviour {


    public FPMovement2 fpmScript;
    private Vector3 flatDisplacement;
    public GameObject vane;

	// Use this for initialization
	void Start()
	{
        fpmScript = GetComponent<FPMovement2>();

	}

	// Update is called once per frame
	void FixedUpdate()
	{
        //get 3d displacement from fpm script
        flatDisplacement = fpmScript.measuredDisplacement;

        //flatify it
        flatDisplacement = new Vector3(flatDisplacement.x, 0, flatDisplacement.z);

        //if it's not zero
        if(flatDisplacement != Vector3.zero){

            //make the vane appear and rotate it correctly
            vane.SetActive(true);
			Quaternion rot = Quaternion.LookRotation(flatDisplacement, Vector3.up);
			vane.transform.rotation = rot;
			vane.transform.rotation = Quaternion.Euler(0, vane.transform.rotation.eulerAngles.y, 0);
        }
        else{

            //if it's zero just make it disappear
            vane.SetActive(false);
        }

	}


}
