using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaneScript : MonoBehaviour {




	public Vector3 displacement;

	public Vector3 lastPosition = Vector3.zero;


    public GameObject vane;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{


        displacement = transform.position - lastPosition;

        displacement = new Vector3(displacement.x, 0, displacement.z);

        if(displacement != Vector3.zero){

            vane.SetActive(true);

			Quaternion rot = Quaternion.LookRotation(displacement, Vector3.up);

			vane.transform.rotation = rot;

			vane.transform.rotation = Quaternion.Euler(0, vane.transform.rotation.eulerAngles.y, 0);
        }
        else{
            vane.SetActive(false);
        }



		lastPosition = transform.position;



	}

	void Update()
	{
	}


}
