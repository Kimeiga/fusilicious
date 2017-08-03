﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DG.Tweening;

public class Inventory2 : MonoBehaviour
{

    //Poder Variables
    public bool stateChange;


    //Initial Grab Raycast Variables
    public Transform cameraTransform;
    public float grabRange = 4;
    private LayerMask grabLayerMask;

    private RaycastHit hit;


    //Grabbing Behaviour Variables
    private GameObject nextItem;
    private Item nextItemScript;

    private GameObject prevItem;
    private Item prevItemScript;

    //public float grabTime = 0.3f;
    private float grabTime = 1;
    public float grabTimeMod = 0.3f;
    public float grabTimeLower = 0.4f;
    public float grabTimeUpper = 0.7f;


    //Hands Variables
    public Transform leftHandTransform;
    private Transform[] leftHandChildren;
    private Vector3 leftHandOriginPos;
    private Quaternion leftHandOriginRot;

    public Transform rightHandTransform;
    private Transform[] rightHandChildren;
    private Vector3 rightHandOriginPos;
    private Quaternion rightHandOriginRot;

    public Transform handTransform;


    //Inventory Variables

    public FPMovement2 fpmScript;




    public GameObject[] inventory;
    public int maxInventorySize = 8;

    public int inventoryIndex;
    public int startingInventoryIndex = 1;



    public Transform[] holsters;

    //drop variables:
    public float dropTime = 0.3f;
    public float dropForce = 0.5f;

    public Collider playerCollider;


    //Throw Variables
    public float throwForce = 3;



    // Use this for initialization
    void Start()
    {

        fpmScript = GetComponent<FPMovement2>();

        inventory = new GameObject[maxInventorySize];
        inventoryIndex = startingInventoryIndex;

        //store right/left hand children to make the layer switching faster
        rightHandChildren = rightHandTransform.GetComponentsInChildren<Transform>();
        leftHandChildren = leftHandTransform.GetComponentsInChildren<Transform>();

        //set grab layer mask for the items:
        grabLayerMask = 1 << LayerMask.NameToLayer("Item");


        //set hand origins
        rightHandOriginPos = rightHandTransform.localPosition;
        rightHandOriginRot = rightHandTransform.localRotation;
        leftHandOriginPos = leftHandTransform.localPosition;
        leftHandOriginRot = leftHandTransform.localRotation;

        //set statechange to false
        stateChange = false;

        //get player collider
        playerCollider = GetComponent<Collider>();




        DOTween.Init();
    }

    // Update is called once per frame
    void Update()
    {

        
        

        if (stateChange == false)
        {

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, grabRange, grabLayerMask))
            {
                //you could make it glow or something in here to show the player that he/she can grab it right HERE



                //ONESHOT: player wants to grab something
                if (Input.GetButtonDown("Grab"))
                {
                    //this should be only called once, so I'm moving it here
                    if (hit.transform.gameObject.GetComponent<Item>() != null)
                    {

                        //set working variables
                        stateChange = true;
                        nextItem = hit.transform.gameObject;
                        nextItemScript = nextItem.GetComponent<Item>();

                        //got yer hands out
                        if (inventory[inventoryIndex] == null)
                        {
                                StartCoroutine(GrabItem(hit.transform.gameObject));

                        }
                        else
                        {


                            //declare workvar for free slot index
                            int freeIndex = -1;


                            for (int i = 0; i < inventory.Length; i++)
                            {
                                if (inventory[i] == null)
                                {
                                    freeIndex = i;
                                    break;
                                }
                            }

                            //if it's still null, then there are no free slots
                            if (freeIndex == -1)
                            {
                                //eventually write a function that drops the item in the last sundry slot and brings the new item directly to that holster
                            }


                            HolsterAndSwitchToItem(hit.transform.gameObject, freeIndex);


                        }

                    }

                }

            }


            if (inventory[inventoryIndex] != null)
            {

                if (Input.GetButtonDown("Drop"))
                {
                    DropCurrentItem(dropForce);
                }
                else if (Input.GetButtonDown("Throw"))
                {
                    DropCurrentItem(throwForce);
                }

            }



            if (Input.GetButtonDown("Slot 0") && inventoryIndex != 0)
            {
                StartCoroutine(SwitchToItem(0));
                //print("switching to 0");
            }
            if (Input.GetButtonDown("Slot 1") && inventoryIndex != 1)
            {
                StartCoroutine(SwitchToItem(1));
                //print("switching to 1");
            }
            if (Input.GetButtonDown("Slot 2") && inventoryIndex != 2)
            {
                StartCoroutine(SwitchToItem(2));
                //print("switching to 2");
            }
            if (Input.GetButtonDown("Slot 3") && inventoryIndex != 3)
            {
                StartCoroutine(SwitchToItem(3));
                //print("switching to 3");
            }
            if (Input.GetButtonDown("Slot 4") && inventoryIndex != 4)
            {
                StartCoroutine(SwitchToItem(4));
                //print("switching to 4");
            }
            if (Input.GetButtonDown("Slot 5") && inventoryIndex != 5)
            {
                StartCoroutine(SwitchToItem(5));
                //print("switching to 5");
            }
            if (Input.GetButtonDown("Slot 6") && inventoryIndex != 6)
            {
                StartCoroutine(SwitchToItem(6));
                //print("switching to 6");  
            }
            if (Input.GetButtonDown("Slot 7") && inventoryIndex != 7)
            {
                StartCoroutine(SwitchToItem(7));
                //print("switching to 7");  
            }


        }



    }




    IEnumerator GrabItem(GameObject item, bool? useTheRightHand = null, bool fromHolsterAndSwitchTo = false)
    {



        //PHASE 1: 1 HAND GOES TO ITEM HAND HOLD

        //set working variables
        stateChange = true;
        nextItem = item;
        nextItemScript = nextItem.GetComponent<Item>();


        if (useTheRightHand == null)
        {
            //find distances for each hand
            float distanceForRightHand = Vector3.Distance(rightHandTransform.position, nextItemScript.rightHandHold.position);
            float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, nextItemScript.leftHandHold.position);

			//find if right hand or if left hand should pick up the item
			useTheRightHand = distanceForRightHand < distanceForLeftHand;
        }

        //recast
        bool useRightHand = (bool)useTheRightHand;


        if (useRightHand)
        {

            //unparent right hand so it can move to gun smoothly
            rightHandTransform.parent = null;

            //you should do this to the hands so that it looks ok:
            rightHandTransform.gameObject.layer = LayerMask.NameToLayer("Body Part Alive");
            foreach (Transform trans in rightHandChildren)
            {
                trans.gameObject.layer = LayerMask.NameToLayer("Body Part Alive");
            }

            //set grab time to something proportional to the distance to the next item
            grabTime = Vector3.Distance(rightHandTransform.position, nextItemScript.rightHandHold.position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

            //Rotate right hand to align with right hand hold
            LeanTween.rotate(rightHandTransform.gameObject, nextItemScript.rightHandHold.rotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

            //Move right hand toward right hand transform
            LeanTween.move(rightHandTransform.gameObject, nextItemScript.rightHandHold, grabTime).setEase(LeanTweenType.easeOutQuart);

        }
        else
        {
            //unparent left hand
            leftHandTransform.parent = null;

            //making the left hand look ok in 3d space.
            leftHandTransform.gameObject.layer = LayerMask.NameToLayer("Body Part Alive");
            foreach (Transform trans in leftHandChildren)
            {
                trans.gameObject.layer = LayerMask.NameToLayer("Body Part Alive");
            }

            //setting grab time proportionally
            grabTime = Vector3.Distance(leftHandTransform.position, nextItemScript.leftHandHold.position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

            //Rotate left hand to align with left hand hold
            LeanTween.rotate(leftHandTransform.gameObject, nextItemScript.leftHandHold.rotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

            //Move left hand toward left hand transform
            LeanTween.move(leftHandTransform.gameObject, nextItemScript.leftHandHold, grabTime).setEase(LeanTweenType.easeOutQuart);
        }



        yield return new WaitForSeconds(grabTime);

		//PHASE 2: BRING ITEM TO THE ITEM'S PREFERRED HOLD POSITION
		//BRING NON GRABBING HAND TO ITS HAND HOLD ON THE ITEM



        //put it in the inventory
        inventory[inventoryIndex] = nextItem;

        //deactivate rigidbody physics
        Rigidbody rigid = nextItem.GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        rigid.useGravity = false;

        //reactivate item
        nextItemScript.active = true;

        //turn off that fucking collider so it doesn't fucking move my dude and it doesn't cause excess collision events and trigger events and ahofidajwepoijvz
		nextItemScript.col.enabled = false;

        //put item on Player Item Layer so it doesn't appear to clip through walls
        nextItem.layer = LayerMask.NameToLayer("Player Item");

        //do this for all its children
        Transform[] childTransforms = nextItem.GetComponentsInChildren<Transform>();
        foreach (Transform trans in childTransforms)
        {
            trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
        }



        //child item to super hand to make preferred local position and rotation easier to calculate
        nextItem.transform.parent = handTransform;


        //set grab time proportional to distance between hold position and the item's position
        grabTime = Vector3.Distance(nextItem.transform.position, handTransform.TransformPoint(nextItemScript.localHoldPosition)) * grabTimeMod;
        grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

        //move item to preferred hold position
        LeanTween.moveLocal(nextItem, nextItemScript.localHoldPosition, grabTime).setEase(LeanTweenType.easeOutQuart);

        //move item to preferred hold rotation
        LeanTween.rotateLocal(nextItem, nextItemScript.localHoldRotation, grabTime).setEase(LeanTweenType.easeOutQuart);



        if (useRightHand)
        {

			//you should do this to the grab hand too so that it looks ok:
			rightHandTransform.gameObject.layer = LayerMask.NameToLayer("Player Item");
			foreach (Transform trans in rightHandChildren)
			{
				trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
			}


			//child right hand to item to allow right hand to move along with item
			rightHandTransform.parent = nextItem.transform;

            //rotate left hand to left hand hold on item
            LeanTween.rotateLocal(leftHandTransform.gameObject, (Quaternion.Euler(nextItemScript.localHoldRotation) * Quaternion.Euler(nextItemScript.leftHandHold.localEulerAngles))
                         .eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

            //move left hand to left hand hold on item
            LeanTween.move(leftHandTransform.gameObject, nextItemScript.leftHandHold, grabTime).setEase(LeanTweenType.easeOutQuart);
        }
        else
        {
			//you should do this to the grab hand too so that it looks ok in world space
			leftHandTransform.gameObject.layer = LayerMask.NameToLayer("Player Item");
			foreach (Transform trans in leftHandChildren)
			{
				trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
			}

			//child left hand to item to allow right hand to move along with item
			leftHandTransform.parent = nextItem.transform;

            //rotate right hand to right hand hold on item
            LeanTween.rotateLocal(rightHandTransform.gameObject, (Quaternion.Euler(nextItemScript.localHoldRotation) * Quaternion.Euler(nextItemScript.rightHandHold.localEulerAngles))
                         .eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

            //move right hand to right hand hold on item
            LeanTween.move(rightHandTransform.gameObject, nextItemScript.rightHandHold, grabTime).setEase(LeanTweenType.easeOutQuart);
        }


        yield return new WaitForSeconds(grabTime);


        if (fromHolsterAndSwitchTo)
        {
            //nullify relavant vars 
            prevItem = null;
            prevItemScript = null;
        }


        //child non grab hand to the gun
        if(useRightHand){
            leftHandTransform.parent = nextItem.transform;
        }
        else{
            rightHandTransform.parent = nextItem.transform;
        }




        //nullify working variables
        nextItem = null;
        nextItemScript = null;
        stateChange = false;

    }



    void DropCurrentItem(float force)
    {

        //set working variables
        stateChange = true;
        prevItem = inventory[inventoryIndex];
        prevItemScript = prevItem.GetComponent<Item>();


        //unchild item from super hand 
        prevItem.transform.parent = null;

        //nullify its inventory slot
        inventory[inventoryIndex] = null;

        //deactivate rigidbody physics
        Rigidbody rigid = prevItem.GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.useGravity = true;

        //unactivate item
        prevItemScript.active = false;



        //put item on back on Item Layer
        prevItem.layer = LayerMask.NameToLayer("Item");
        Transform[] childTransforms = prevItem.GetComponentsInChildren<Transform>();
        foreach (Transform trans in childTransforms)
        {
            trans.gameObject.layer = LayerMask.NameToLayer("Item");
        }


        //throw the fucker
        rigid.AddForce(cameraTransform.forward * force, ForceMode.Impulse);

        //keep your hands on it for a bit and then retract them to make it seem as if you are throwing it with your hands hahahahhahaha
        StartCoroutine(ThrowWithYourHands(0.2f));

        //make sure that its trajectory is not going to be influenced by the current player so long as they are intersecting
        StartCoroutine(DontCollideWithDroppingItem(prevItemScript.col));


    

    }


	IEnumerator ThrowWithYourHands(float holdTime)
	{
		yield return new WaitForSeconds(holdTime);

		leftHandTransform.parent = handTransform;
		rightHandTransform.parent = handTransform;

		LeanTween.moveLocal(rightHandTransform.gameObject, rightHandOriginPos, dropTime).setEase(LeanTweenType.easeOutExpo);
		LeanTween.rotateLocal(rightHandTransform.gameObject, rightHandOriginRot.eulerAngles, dropTime).setEase(LeanTweenType.easeOutExpo);
		LeanTween.moveLocal(leftHandTransform.gameObject, leftHandOriginPos, dropTime).setEase(LeanTweenType.easeOutExpo);
		LeanTween.rotateLocal(leftHandTransform.gameObject, leftHandOriginRot.eulerAngles, dropTime).setEase(LeanTweenType.easeOutExpo).setOnComplete(() =>
		{

			//nullify working variables
			prevItem = null;
			prevItemScript = null;
			stateChange = false;
		});
	}


    IEnumerator DontCollideWithDroppingItem(Collider itemCol)
    {

        Physics.IgnoreCollision(itemCol, playerCollider);

		//turn collider back on alright fine jeez
        itemCol.enabled = true;

        yield return new WaitUntil(() => itemCol.bounds.Intersects(playerCollider.bounds) == false);

        Physics.IgnoreCollision(itemCol, playerCollider, false);

    }


    void HolsterAndSwitchToItem(GameObject item, int slotToHolsterTo)
    {

        //set working variables
        stateChange = true;

        prevItem = inventory[inventoryIndex];
        prevItemScript = prevItem.GetComponent<Item>();

        nextItem = item;
        nextItemScript = nextItem.GetComponent<Item>();


        //find if right hand or if left hand should carry the gun over to the holster

        float distanceForRightHand = Vector3.Distance(rightHandTransform.position, holsters[slotToHolsterTo].position);
        float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, holsters[slotToHolsterTo].position);


        bool useRightHand = distanceForRightHand < distanceForLeftHand;

        if (useRightHand)
        {
            rightHandTransform.parent = prevItem.transform;
            leftHandTransform.parent = handTransform;

        }
        else
        {
            leftHandTransform.parent = prevItem.transform;
            rightHandTransform.parent = handTransform;
        }

        //child item to target holster
        prevItem.transform.parent = holsters[slotToHolsterTo];

        //put it on the new inventory slot
        inventory[slotToHolsterTo] = prevItem;


        //unactivate item
        prevItemScript.active = false;


        grabTime = Vector3.Distance(prevItem.transform.position, holsters[slotToHolsterTo].position) * grabTimeMod;
        grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);


        LeanTween.moveLocal(prevItem, Vector3.zero, grabTime).setEase(LeanTweenType.easeOutExpo);
        LeanTween.rotateLocal(prevItem, Quaternion.identity.eulerAngles, grabTime).setEase(LeanTweenType.easeOutExpo);

        StartCoroutine(GrabItem(nextItem, !useRightHand, true));



    }


    IEnumerator SwitchToItem(int nextSlot)
    {


		//set working variables
		stateChange = true;

        bool fromHands = (inventory[inventoryIndex] == null);
        bool toHands = inventory[nextSlot] == null;

        if (!fromHands)
        {
            prevItem = inventory[inventoryIndex];
            prevItemScript = prevItem.GetComponent<Item>();

        }

        if(!toHands){

	        nextItem = inventory[nextSlot];
	        nextItemScript = nextItem.GetComponent<Item>();
        }


        //if you are just going from hands to hands, you literally just need to change the current inventory index and get the fuck out of here
        if (fromHands && toHands){
			inventoryIndex = nextSlot;
            stateChange = false;
            yield break;
        }
        //of course, this means that from here on, either fromHands is true or toHands is true; they are mutually exclusive



        bool useRightHand;

        if (!fromHands){

			//find if right hand or if left hand should carry the gun over to the holster
			float distanceForRightHand = Vector3.Distance(rightHandTransform.position, holsters[inventoryIndex].position);
			float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, holsters[inventoryIndex].position);

			useRightHand = distanceForRightHand < distanceForLeftHand;
        }
        else{

			//find if right hand or if left hand should get the next item
            float distanceForRightHand = Vector3.Distance(rightHandTransform.position, holsters[nextSlot].position);
            float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, holsters[nextSlot].position);

			useRightHand = distanceForRightHand < distanceForLeftHand;
        }

        //going from item to item or item to hands
        if (!fromHands)
        {

            if (useRightHand)
            {
                //child putting away hand to previtem so the hand can put it away
                rightHandTransform.parent = prevItem.transform;
            }
            else
            {
                leftHandTransform.parent = prevItem.transform;
            }

            //child item to target holster
            prevItem.transform.parent = holsters[inventoryIndex];

            //unactivate item
            prevItemScript.active = false;

            //set grab time
            grabTime = Vector3.Distance(prevItem.transform.position, holsters[inventoryIndex].position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

            //move and rotate gun to it's holster
            LeanTween.moveLocal(prevItem, Vector3.zero, grabTime).setEase(LeanTweenType.easeOutQuart);

            prevItem.transform.DOLocalRotateQuaternion(Quaternion.Euler(prevItemScript.localHoldRotation), grabTime).SetEase(Ease.OutQuart);


            //don't need to get next holstered item if you are just going to end up with hands
            if(!toHands){

				if (useRightHand)
				{
                 
                    //child non putting away hand to next item so it can move to the hand hold and prep to ready it
					leftHandTransform.parent = nextItem.transform;


					LeanTween.moveLocal(leftHandTransform.gameObject, nextItemScript.leftHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuart);
                    LeanTween.rotateLocal(leftHandTransform.gameObject, nextItemScript.leftHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);
				}
				else
				{
					//child non putting away hand to next item so it can move to the hand hold and prep to ready it
					rightHandTransform.parent = nextItem.transform;


					LeanTween.moveLocal(rightHandTransform.gameObject, nextItemScript.rightHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuart);
                    LeanTween.rotateLocal(rightHandTransform.gameObject, nextItemScript.rightHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);
				}
            }

            else{
				//you are going to hands so you need to move your non putting away hand back to its normal position and shit so it doesn't look weird 
				if (useRightHand)
				{
                    leftHandTransform.parent = handTransform.transform;

					LeanTween.moveLocal(leftHandTransform.gameObject, leftHandOriginPos, dropTime).setEase(LeanTweenType.easeOutQuart);
                    LeanTween.rotateLocal(leftHandTransform.gameObject, leftHandOriginRot.eulerAngles, dropTime).setEase(LeanTweenType.easeOutQuart);

				}
				else
				{
					//child non putting away hand to next item so it can move to the hand hold and prep to ready it
                    rightHandTransform.parent = handTransform.transform;


					LeanTween.moveLocal(rightHandTransform.gameObject, rightHandOriginPos, dropTime).setEase(LeanTweenType.easeOutQuart);
					LeanTween.rotateLocal(rightHandTransform.gameObject, rightHandOriginRot.eulerAngles, dropTime).setEase(LeanTweenType.easeOutQuart);
				}

            }

        }

        else 
        {
            //actually just move the closest hand to the next holstered item in preperation to ready it. you don't need both hands going to it like that just doesn't happen naturally duh hakan...


            if(useRightHand){

				rightHandTransform.parent = nextItem.transform;

				grabTime = Vector3.Distance(rightHandTransform.position, nextItemScript.rightHandHold.position) * grabTimeMod;
				grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

				LeanTween.moveLocal(rightHandTransform.gameObject, nextItemScript.rightHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuart);
				LeanTween.rotateLocal(rightHandTransform.gameObject, nextItemScript.rightHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

			}
            else{

				leftHandTransform.parent = nextItem.transform;

				grabTime = Vector3.Distance(leftHandTransform.position, nextItemScript.leftHandHold.position) * grabTimeMod;
				grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

				LeanTween.moveLocal(leftHandTransform.gameObject, nextItemScript.leftHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuart);
                LeanTween.rotateLocal(leftHandTransform.gameObject, nextItemScript.leftHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

            }

        }



        //yeah we are done with the first phase dawg
		yield return new WaitForSeconds(grabTime);






        if(!toHands){

            //put new item on handtransform so you can ready it and shit
			nextItem.transform.parent = handTransform;

			//put item on Player Item Layer so it doesn't appear to clip through walls
			nextItem.layer = LayerMask.NameToLayer("Player Item");

			//do this for all its children
			Transform[] childTransforms = nextItem.GetComponentsInChildren<Transform>();
			foreach (Transform trans in childTransforms)
			{
				trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
			}

            //the hands should already be on player item layer i think...

			//reactivate item
			nextItemScript.active = true;

            //set grab time
			grabTime = Vector3.Distance(nextItem.transform.position, handTransform.TransformPoint(nextItemScript.localHoldPosition)) * grabTimeMod;
			grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

            //move item to preferred hold position
            LeanTween.moveLocal(nextItem, nextItemScript.localHoldPosition, grabTime).setEase(LeanTweenType.easeOutQuart);

			//move item to preferred hold rotation
			nextItem.transform.DOLocalRotateQuaternion(Quaternion.Euler(nextItemScript.localHoldRotation), grabTime).SetEase(Ease.OutQuart);




            if (!fromHands){

				if (!useRightHand)
				{


					leftHandTransform.parent = nextItem.transform;

					//rotate left hand to left hand hold on item
					LeanTween.rotateLocal(leftHandTransform.gameObject, nextItemScript.leftHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

					//move left hand to left hand hold on item
					LeanTween.moveLocal(leftHandTransform.gameObject, nextItemScript.leftHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuart);


				}
				else
				{
					rightHandTransform.parent = nextItem.transform;


					//rotate left hand to left hand hold on item
					LeanTween.rotateLocal(rightHandTransform.gameObject, nextItemScript.rightHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);


					//move left hand to left hand hold on item
					LeanTween.moveLocal(rightHandTransform.gameObject, nextItemScript.rightHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuart);
				}
            }
            else{

				if (useRightHand)
				{

					leftHandTransform.parent = nextItem.transform;

					//rotate left hand to left hand hold on item
					LeanTween.rotateLocal(leftHandTransform.gameObject, nextItemScript.leftHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

					//move left hand to left hand hold on item
					LeanTween.moveLocal(leftHandTransform.gameObject, nextItemScript.leftHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuart);


				}
				else
				{
					rightHandTransform.parent = nextItem.transform;


					//rotate left hand to left hand hold on item
					LeanTween.rotateLocal(rightHandTransform.gameObject, nextItemScript.rightHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);


					//move left hand to left hand hold on item
					LeanTween.moveLocal(rightHandTransform.gameObject, nextItemScript.rightHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuart);
				}
            }

        }
        else{
			//if we are going to hands...
			//all you gotta do is send the putting away hand back to the normal orientation

			if (useRightHand)
			{
                //set grab time based on how far away the hand location is
                grabTime = Vector3.Distance(rightHandTransform.position, handTransform.TransformPoint(rightHandOriginPos)) * grabTimeMod;
				grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);


				//child putting away hand to hand transform so we can move it back to place
				rightHandTransform.parent = handTransform.transform;

				LeanTween.moveLocal(rightHandTransform.gameObject, rightHandOriginPos, dropTime).setEase(LeanTweenType.easeOutQuart);
				LeanTween.rotateLocal(rightHandTransform.gameObject, rightHandOriginRot.eulerAngles, dropTime).setEase(LeanTweenType.easeOutQuart);

			}
			else
			{
				//set grab time based on how far away the hand lockation is
				grabTime = Vector3.Distance(leftHandTransform.position, handTransform.TransformPoint(leftHandOriginPos)) * grabTimeMod;
				grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

				leftHandTransform.parent = handTransform.transform;

				LeanTween.moveLocal(leftHandTransform.gameObject, leftHandOriginPos, dropTime).setEase(LeanTweenType.easeOutQuart);
				LeanTween.rotateLocal(leftHandTransform.gameObject, leftHandOriginRot.eulerAngles, dropTime).setEase(LeanTweenType.easeOutQuart);

			}
        }



        yield return new WaitForSeconds(grabTime);


        //nullify working variables
        if (!fromHands){
			prevItem = null;
			prevItemScript = null;
        }

        if(!toHands){
			nextItem = null;
			nextItemScript = null;
        }

        stateChange = false;

        inventoryIndex = nextSlot;

    }


}


