using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType { Melee, Gun, Gadget, Sundry }

public class Item : MonoBehaviour {


    public ItemType type;

    [System.NonSerialized]
	public bool active = false;

    public Transform rightHandHold;
    public Transform leftHandHold;

    public Vector3 localHoldPosition;
    public Vector3 localHoldRotation;


    public Collider col;


	// Use this for initialization
	void Start () {

        //Inspector derp failsafes

        //if I didn't set the collider
        if(col == null){

            //use the one on the root
            col = gameObject.GetComponent<Collider>();

            //if there isn't one
            if(col == null){

                //iterate through the kids
				foreach (Transform child in transform)
				{

                    //if the current kid has a collider
                    if(child.gameObject.GetComponent<Collider>() != null){

                        //just use that one
                        col = child.gameObject.GetComponent<Collider>();

                        //break: therefore the first kid with the collider wins
                        break;
                    }
				}
            }

        }

        //if I didn't set right hand hold
        if(rightHandHold == null){
            rightHandHold = transform.Find("Right Hand Hold");
        }

		//if I didn't set left hand hold
		if (leftHandHold == null)
		{
            leftHandHold = transform.Find("Right Hand Hold");
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
