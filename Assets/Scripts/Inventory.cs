using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DG.Tweening;

public class Inventory : MonoBehaviour
{

    //Poder Variables
    public bool stateChange = false;


    //Initial Grab Raycast Variables
    public Transform cameraTransform;
    public float grabRange = 4;
    private LayerMask grabLayerMask;

    private RaycastHit hit;


    //Grabbing Behaviour Variables
    private GameObject currentItemNext;
    private Item currentItemNextScript;

    private GameObject currentItemPrev;
    private Item currentItemPrevScript;

    //public float grabTime = 0.3f;
    public float grabTimeMod = 4f;
    public float grabTimeLower = 0.2f;
    public float grabTimeUpper = 2f;


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


    public ItemType[] inventorySlots = {
        ItemType.Melee,
        ItemType.Gun,
        ItemType.Gun,
        ItemType.Gadget,
        ItemType.Gadget,
        ItemType.Sundry,
        ItemType.Sundry,
        ItemType.Sundry
    };

    private int[] meleeSlots;
    private int[] gunSlots;
    private int[] gadgetSlots;
    private int[] sundrySlots;


    public GameObject[] inventory;
    private int maxInventorySize;

    public int inventoryIndex;
    public int startingInventoryIndex = 1;



	public Transform[] holsters;

    //drop variables:
    public float dropTime = 0.3f;
    public float dropForce = 0.2f;

    public Collider playerCollider;


    //Throw Variables
	public float throwForce = 10;



	// Use this for initialization
	void Start () {

        //set max inventory size and starting index based on inventory slots and starting index
        maxInventorySize = inventorySlots.Length;
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


        //initialize slot int arrays:
        meleeSlots = inventorySlots.FindAllIndexof(ItemType.Melee);
        gunSlots = inventorySlots.FindAllIndexof(ItemType.Gun);
        gadgetSlots = inventorySlots.FindAllIndexof(ItemType.Gadget);
        sundrySlots = inventorySlots.FindAllIndexof(ItemType.Sundry);


        DOTween.Init();
	}
	
	// Update is called once per frame
	void Update () {

		//This Raycast just starts the grab

		//you can't grab (or attempt to grab) another item while you are grabbing
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, grabRange, grabLayerMask) && stateChange == false)
		{
            //you could make it glow or something in here to show the player that he/she can grab it right HERE



            //ONESHOT: player wants to grab something
            if (Input.GetButtonDown("Grab"))
			{
                //this should be only called once, so I'm moving it here
                if(hit.transform.gameObject.GetComponent<Item>() != null){

					//set working variables
					stateChange = true;
					currentItemNext = hit.transform.gameObject;
					currentItemNextScript = currentItemNext.GetComponent<Item>();

					//is the object a gun?
					if (currentItemNextScript.type == ItemType.Gun)
					{

						//are we in a gun/sundry slot?
						if (inventorySlots[inventoryIndex] == ItemType.Gun || inventorySlots[inventoryIndex] == ItemType.Sundry)
						{
							//you are on a gun slot or sundry slot

                            //do we have a current item
							if (inventory[inventoryIndex] == null)
							{
								//you have no item in this slot so just grab the fucker
                                StartCoroutine(GrabItem(hit.transform.gameObject));
							}
                            else{
                                //you currently have either a gun in a gun slot, or an ambiguous item in a sundry slot
                                //you probably want to holster what you are holding
                                //and grab the gun on the ground
                                //but you can only do that if you have a holster to put the current item in
                                //so we have to iterate through all of them and see if any are empty

                                //declare workvar for free slot index
                                int freeIndex = -1;

                                //iterate through gunslots first
                                foreach(int index in gunSlots){
                                    if(inventory[index] == null){
                                        freeIndex = index;
                                        break;
                                    }
                                }

                                //if it's still null, iterate though sundry slots
                                if (freeIndex == -1){
                                    foreach(int index in sundrySlots){
                                        if(inventory[index] == null){
                                            freeIndex = index;
                                            break;
                                        }
                                    }
                                }

                                //if it's still null, then there are no free slots
                                if (freeIndex == -1){
                                    //give an error here   
                                }

                                //if you have an empty slot after all, then pass it to the holster and switch to item function
                                //it will be used as the destination inventory slot to put the current item, and the index of the holster to put it in
                                //meanwhile, the current inventory index will stay the same and the new gun you are picking up will become the inventory[inventoryIndex]
                                HolsterAndSwitchToItem(hit.transform.gameObject, freeIndex);

                            }

						}
						else
						{
							//you are not in a gun/sundry slot
							//give an error sound
							return;
						}

					}
                }

			}

		}

        if (inventory[inventoryIndex] != null && stateChange == false){

            if(Input.GetButtonDown("Drop")){
                DropCurrentItem(dropForce);
            }
            else if(Input.GetButtonDown("Throw")){
                DropCurrentItem(throwForce, false);
            }

        }

        if (Input.GetButtonDown("Slot 0") && stateChange == false && inventory[0] != null && inventoryIndex != 0){
                StartCoroutine(SwitchToItem(0));
                //print("switching to 0");
        }
		if (Input.GetButtonDown("Slot 1") && stateChange == false && inventory[1] != null && inventoryIndex != 1)
		{
				StartCoroutine(SwitchToItem(1));
                //print("switching to 1");
		}
		if (Input.GetButtonDown("Slot 2") && stateChange == false && inventory[2] != null && inventoryIndex != 2)
		{
				StartCoroutine(SwitchToItem(2));
                //print("switching to 2");
		}
		if (Input.GetButtonDown("Slot 3") && stateChange == false && inventory[3] != null && inventoryIndex != 3)
		{
				StartCoroutine(SwitchToItem(3));
                //print("switching to 3");
		}
		if (Input.GetButtonDown("Slot 4") && stateChange == false && inventory[4] != null && inventoryIndex != 4)
		{
				StartCoroutine(SwitchToItem(4));
                //print("switching to 4");
		}
		if (Input.GetButtonDown("Slot 5") && stateChange == false && inventory[5] != null && inventoryIndex != 5)
		{
				StartCoroutine(SwitchToItem(5));
                //print("switching to 5");
		}
		if (Input.GetButtonDown("Slot 6") && stateChange == false && inventory[6] != null && inventoryIndex != 6)
		{
				StartCoroutine(SwitchToItem(6));
                //print("switching to 6");	
		}
		if (Input.GetButtonDown("Slot 7") && stateChange == false && inventory[7] != null && inventoryIndex != 7)
		{	
				StartCoroutine(SwitchToItem(7));
                //print("switching to 7");	
		}


	}




    IEnumerator GrabItem(GameObject item, bool fromReplace = false, bool? useTheRightHand = null, bool fromHolsterAndSwitchTo = false){

        //PHASE 1: RIGHT HAND TO RIGHT HAND HOLD ON ITEM


		//set working variables
        stateChange = true;
		currentItemNext = item;
		currentItemNextScript = currentItemNext.GetComponent<Item>();



        if(useTheRightHand == null){

	        //find if right hand or if left hand should pick up the gun
	        float distanceForRightHand = Vector3.Distance(rightHandTransform.position, currentItemNextScript.rightHandHold.position);
	        float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, currentItemNextScript.leftHandHold.position);


	        useTheRightHand = distanceForRightHand < distanceForLeftHand;
        }

        bool useRightHand = (bool)useTheRightHand;


        if(useRightHand){

	        //unparent right hand so it can move to gun smoothly
	        rightHandTransform.parent = null;
        }
        else{
            //unparent left hand
            leftHandTransform.parent = null;
        }


        if(useRightHand){

	        //you should do this to the hands too so that it looks ok:
	        rightHandTransform.gameObject.layer = LayerMask.NameToLayer("Body Part Alive");
	        foreach (Transform trans in rightHandChildren)
	        {
	            trans.gameObject.layer = LayerMask.NameToLayer("Body Part Alive");
	        }
        }
        else{

            leftHandTransform.gameObject.layer = LayerMask.NameToLayer("Body Part Alive");
            foreach (Transform trans in leftHandChildren)
            {
                trans.gameObject.layer = LayerMask.NameToLayer("Body Part Alive");
            }
        }

        float grabTime = 1;

        if(useRightHand){


            grabTime = Vector3.Distance(rightHandTransform.position,currentItemNextScript.rightHandHold.position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

	        //Rotate right hand to align with right hand hold
            LeanTween.rotate(rightHandTransform.gameObject, currentItemNextScript.rightHandHold.rotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

	        //Move right hand toward right hand transform
            LeanTween.move(rightHandTransform.gameObject, currentItemNextScript.rightHandHold, grabTime).setEase(LeanTweenType.easeOutQuart);   
        }
        else{

            grabTime = Vector3.Distance(leftHandTransform.position, currentItemNextScript.leftHandHold.position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

			//Rotate left hand to align with left hand hold
			LeanTween.rotate(leftHandTransform.gameObject, currentItemNextScript.leftHandHold.rotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuart);

	        //Move left hand toward left hand transform
            LeanTween.move(leftHandTransform.gameObject, currentItemNextScript.leftHandHold, grabTime).setEase(LeanTweenType.easeOutQuart);
        }

        yield return new WaitForSeconds(grabTime);

        //PHASE 2: ITEM TO ITEM'S PREFERRED HOLD POSITION
        //LEFT HAND TO LEFT HAND HOLD ON ITEM


        if (fromReplace){
            leftHandTransform.parent = handTransform;
        }

        //put it in the inventory
        inventory[inventoryIndex] = currentItemNext;

        //deactivate rigidbody physics
        Rigidbody rigid = currentItemNext.GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        rigid.useGravity = false;

        //reactivate item
        currentItemNextScript.active = true;

        //put collider to trigger mode
        Collider col = currentItemNextScript.col;
        col.isTrigger = true;

        //put item on Player Item Layer so it doesn't appear to clip through walls
        currentItemNext.layer = LayerMask.NameToLayer("Player Item");

        //do this for all its children
        Transform[] childTransforms = currentItemNext.GetComponentsInChildren<Transform>();
        foreach(Transform trans in childTransforms){
            trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
        }

        //you should do this to the hands too so that it looks ok:
        rightHandTransform.gameObject.layer = LayerMask.NameToLayer("Player Item");
        foreach (Transform trans in rightHandChildren)
		{
			trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
		}

        leftHandTransform.gameObject.layer = LayerMask.NameToLayer("Player Item");
        foreach (Transform trans in leftHandChildren)
		{
			trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
		}

        //child item to super hand to make preferred local position and rotation easier to calculate
        currentItemNext.transform.parent = handTransform;


        if(useRightHand){

			//child right hand to item to allow right hand to move along with item
			rightHandTransform.parent = currentItemNext.transform;
        }
        else{
            leftHandTransform.parent = currentItemNext.transform;
        }

        grabTime = Vector3.Distance(currentItemNext.transform.position, handTransform.TransformPoint(currentItemNextScript.localHoldPosition)) * grabTimeMod;
        grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

		//move item to preferred hold position
		LeanTween.moveLocal(currentItemNext, currentItemNextScript.localHoldPosition, grabTime).setEase(LeanTweenType.easeOutExpo);

        //move item to preferred hold rotation
        LeanTween.rotateLocal(currentItemNext, currentItemNextScript.localHoldRotation, grabTime).setEase(LeanTweenType.easeOutExpo);



        if(useRightHand){

            leftHandTransform.parent = handTransform;

            grabTime = Vector3.Distance(leftHandTransform.position, currentItemNextScript.leftHandHold.position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

	        //rotate left hand to left hand hold on item
	        LeanTween.rotateLocal(leftHandTransform.gameObject, (Quaternion.Euler(currentItemNextScript.localHoldRotation) * Quaternion.Euler(currentItemNextScript.leftHandHold.localEulerAngles))
	                     .eulerAngles, grabTime).setEase(LeanTweenType.easeOutExpo);

	        //move left hand to left hand hold on item
	        LeanTween.move(leftHandTransform.gameObject, currentItemNextScript.leftHandHold, grabTime).setEase(LeanTweenType.easeOutExpo).setOnComplete( ()=>{

                if (fromHolsterAndSwitchTo){
					currentItemPrev = null;
					currentItemPrevScript = null;

                }

	            //nullify working variables
	            currentItemNext = null;
	            currentItemNextScript = null;
	            stateChange = false;

	        });
        }
        else{
            rightHandTransform.parent = handTransform;

            grabTime = Vector3.Distance(rightHandTransform.position, currentItemNextScript.rightHandHold.position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

	        //rotate right hand to right hand hold on item
            LeanTween.rotateLocal(rightHandTransform.gameObject, (Quaternion.Euler(currentItemNextScript.localHoldRotation) * Quaternion.Euler(currentItemNextScript.rightHandHold.localEulerAngles))
	                     .eulerAngles, grabTime).setEase(LeanTweenType.easeOutExpo);

	        //move right hand to right hand hold on item
            LeanTween.move(rightHandTransform.gameObject, currentItemNextScript.rightHandHold, grabTime).setEase(LeanTweenType.easeOutExpo).setOnComplete( ()=>{

				if (fromHolsterAndSwitchTo)
				{
					currentItemPrev = null;
					currentItemPrevScript = null;

				}

	            //nullify working variables
	            currentItemNext = null;
	            currentItemNextScript = null;
	            stateChange = false;

	        });
        }

		//It makes sense to keep the right hand childed to the gun in case you want to do ADS or something
		//the left hand should be free so it can do other things



    }

    /*
    void ReplaceWithItem(){

        //heeeere weeee gooooo!!!!

        DropCurrentItem(dropForce,true);

        GrabItem(true);
    }
    */


    void DropCurrentItem(float force, bool fromReplace = false){


        //set working variables
        stateChange = true;
		currentItemPrev = inventory[inventoryIndex];
		currentItemPrevScript = currentItemPrev.GetComponent<Item>();

        //unchild right hand from item to handtransform
        rightHandTransform.parent = handTransform;


        //unchild item from super hand 
        currentItemPrev.transform.parent = null;

		//nullify its inventory slot
		inventory[inventoryIndex] = null;

		//deactivate rigidbody physics
		Rigidbody rigid = currentItemPrev.GetComponent<Rigidbody>();
        rigid.isKinematic = false;
		rigid.useGravity = true;

        //unactivate item
        currentItemPrevScript.active = false;



		//put collider to not trigger mode
		Collider col = currentItemPrevScript.col;
        col.isTrigger = false;

		//put item on back on Item Layer
		currentItemPrev.layer = LayerMask.NameToLayer("Item");

		//do this for all its children
		Transform[] childTransforms = currentItemPrev.GetComponentsInChildren<Transform>();
		foreach (Transform trans in childTransforms)
		{
			trans.gameObject.layer = LayerMask.NameToLayer("Item");
		}

        if (fromReplace == false){


            //Physics.IgnoreCollision(col, GetComponent<Collider>());


            StartCoroutine(DontCollideWithDroppingItem(col));

	        //push gun a little bit out
            rigid.AddForce(cameraTransform.forward * force,ForceMode.Impulse);

            rightHandTransform.parent = handTransform;
            leftHandTransform.parent = handTransform;

	        //move hands back to normal!! :)
            LeanTween.moveLocal(rightHandTransform.gameObject,rightHandOriginPos,dropTime).setEase(LeanTweenType.easeOutExpo);
            LeanTween.rotateLocal(rightHandTransform.gameObject, rightHandOriginRot.eulerAngles, dropTime).setEase(LeanTweenType.easeOutExpo);
            LeanTween.moveLocal(leftHandTransform.gameObject, leftHandOriginPos, dropTime).setEase(LeanTweenType.easeOutExpo);
            LeanTween.rotateLocal(leftHandTransform.gameObject, leftHandOriginRot.eulerAngles, dropTime).setEase(LeanTweenType.easeOutExpo).setOnComplete(()=>
            {

		        //nullify working variables
		        currentItemPrev = null;
		        currentItemPrevScript = null;
		        stateChange = false;
            });
		}
		else{

            //child left hand to the item
            leftHandTransform.parent = currentItemPrev.transform;

            //Physics.IgnoreCollision(col, GetComponent<Collider>());

            StartCoroutine(DontCollideWithDroppingItem(col));

	        //push gun a little bit out
            rigid.AddForce(-transform.right * force + transform.forward * 0.8f * force,ForceMode.Impulse);


	        //nullify working variables
	        currentItemPrev = null;
	        currentItemPrevScript = null;
	        stateChange = false;
        }

	}

    IEnumerator DontCollideWithDroppingItem( Collider itemCol){

        Physics.IgnoreCollision(itemCol, GetComponent<Collider>());
        //print(Time.time);

        yield return new WaitUntil(() => itemCol.bounds.Intersects(playerCollider.bounds) == false);
        //print(Time.time);
        Physics.IgnoreCollision(itemCol, GetComponent<Collider>(), false);

    }



    void HolsterAndSwitchToItem(GameObject item, int slotToHolsterTo){

        //if you don't have any room left, make error noise instead of auto emptying a holster. The player probably would want to choose which item to give up

		//set working variables
		stateChange = true;

        currentItemPrev = inventory[inventoryIndex];
        currentItemPrevScript = currentItemPrev.GetComponent<Item>();

		currentItemNext = item;
		currentItemNextScript = currentItemNext.GetComponent<Item>();


		//find if right hand or if left hand should carry the gun over to the holster

        float distanceForRightHand = Vector3.Distance(rightHandTransform.position, holsters[slotToHolsterTo].position);
        float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, holsters[slotToHolsterTo].position);


		bool useRightHand = distanceForRightHand < distanceForLeftHand;

        if(useRightHand){
            rightHandTransform.parent = currentItemPrev.transform;
            leftHandTransform.parent = handTransform;

        }
        else{
            leftHandTransform.parent = currentItemPrev.transform;
            rightHandTransform.parent = handTransform;
        }

		//child item to target holster
        currentItemPrev.transform.parent = holsters[slotToHolsterTo];

		//put it on the new inventory slot
        inventory[slotToHolsterTo] = currentItemPrev;


		//unactivate item
		currentItemPrevScript.active = false;


        float grabTime = Vector3.Distance(currentItemPrev.transform.position, holsters[slotToHolsterTo].position) * grabTimeMod;
        grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);


        LeanTween.moveLocal(currentItemPrev, Vector3.zero, grabTime).setEase(LeanTweenType.easeOutExpo);
        LeanTween.rotateLocal(currentItemPrev, Quaternion.identity.eulerAngles, grabTime).setEase(LeanTweenType.easeOutExpo);

        StartCoroutine(GrabItem(currentItemNext, false, !useRightHand, true));

  //      yield return new WaitForSeconds(grabTime * 2);

  //      //nullify working variables

  //      currentItemPrev = null;
  //      currentItemPrevScript = null;

		//currentItemNext = null;
		//currentItemNextScript = null;


		//stateChange = false;

    }


    IEnumerator SwitchToItem(int nextSlot){

		//set working variables
		stateChange = true;

        bool fromHands = (inventory[inventoryIndex] == null);


        if (!fromHands){

			currentItemPrev = inventory[inventoryIndex];
			currentItemPrevScript = currentItemPrev.GetComponent<Item>();   
        }


        currentItemNext = inventory[nextSlot];
		currentItemNextScript = currentItemNext.GetComponent<Item>();

		//find if right hand or if left hand should carry the gun over to the holster

		float distanceForRightHand = Vector3.Distance(rightHandTransform.position, holsters[inventoryIndex].position);
		float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, holsters[inventoryIndex].position);


		bool useRightHand = distanceForRightHand < distanceForLeftHand;

        float grabTime = 1;

        bool doneWithPhase1 = false;

        if (!fromHands){


			if (useRightHand)
			{
				rightHandTransform.parent = currentItemPrev.transform;
				leftHandTransform.parent = handTransform;
			}
			else
			{
				leftHandTransform.parent = currentItemPrev.transform;
				rightHandTransform.parent = handTransform;

			}


			//child item to target holster
			currentItemPrev.transform.parent = holsters[inventoryIndex];



			//unactivate item
			currentItemPrevScript.active = false;

            //Vector3 temp = currentItemPrev.transform.localRotation.eulerAngles;
            //temp.z = -temp.z;
            //currentItemPrev.transform.localRotation = Quaternion.Euler(temp);

            grabTime = Vector3.Distance(currentItemPrev.transform.position, holsters[inventoryIndex].position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

            LeanTween.moveLocal(currentItemPrev, Vector3.zero, grabTime).setEase(LeanTweenType.easeOutQuad);
			//LeanTween.rotateLocal(currentItemPrev, Quaternion.identity.eulerAngles, grabTime).setEase(LeanTweenType.easeOutExpo);

            //hakan


            //currentItemPrev.transform.localRotation = Quaternion.RotateTowards(currentItemPrev.transform.localRotation, Quaternion.Euler(currentItemPrevScript.localHoldRotation), 0.5f);



            currentItemPrev.transform.DOLocalRotateQuaternion(Quaternion.Euler(currentItemPrevScript.localHoldRotation),grabTime).SetEase(Ease.OutQuad);


			if (useRightHand)
			{
                

				leftHandTransform.parent = currentItemNext.transform;

                grabTime = Vector3.Distance(leftHandTransform.position, currentItemNextScript.leftHandHold.position) * grabTimeMod;
                grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

				LeanTween.moveLocal(leftHandTransform.gameObject, currentItemNextScript.leftHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuad);
                LeanTween.rotateLocal(leftHandTransform.gameObject, currentItemNextScript.leftHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>{
                    doneWithPhase1 = true;
                });
			}
			else
			{
				rightHandTransform.parent = currentItemNext.transform;

                grabTime = Vector3.Distance(rightHandTransform.position, currentItemNextScript.rightHandHold.position) * grabTimeMod;
                grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

				LeanTween.moveLocal(rightHandTransform.gameObject, currentItemNextScript.rightHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuad);
                LeanTween.rotateLocal(rightHandTransform.gameObject, currentItemNextScript.rightHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>{
                    doneWithPhase1 = true;
                });
			}

		}
        else{

	        rightHandTransform.parent = currentItemNext.transform;

            grabTime = Vector3.Distance(rightHandTransform.position, currentItemNextScript.rightHandHold.position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

	        LeanTween.moveLocal(rightHandTransform.gameObject, currentItemNextScript.rightHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuad);
	        LeanTween.rotateLocal(rightHandTransform.gameObject, currentItemNextScript.rightHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuad);


	        leftHandTransform.parent = currentItemNext.transform;

            grabTime = Vector3.Distance(leftHandTransform.position, currentItemNextScript.leftHandHold.position) * grabTimeMod;
            grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

	        LeanTween.moveLocal(leftHandTransform.gameObject, currentItemNextScript.leftHandHold.localPosition, grabTime).setEase(LeanTweenType.easeOutQuad);
            LeanTween.rotateLocal(leftHandTransform.gameObject, currentItemNextScript.leftHandHold.localRotation.eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>{
                doneWithPhase1 = true;
            });


        }





        yield return new WaitUntil(()=>doneWithPhase1);




        currentItemNext.transform.parent = handTransform;


        //Vector3 temp2 = currentItemNext.transform.localRotation.eulerAngles;
        //temp2.z = -temp2.z;
        //currentItemNext.transform.localRotation = Quaternion.Euler(temp2);

		//put item on Player Item Layer so it doesn't appear to clip through walls
		currentItemNext.layer = LayerMask.NameToLayer("Player Item");

        //do this for all its children
        Transform[] childTransforms = currentItemNext.GetComponentsInChildren<Transform>();
		foreach (Transform trans in childTransforms)
		{
			trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
		}

		//you should do this to the hands too so that it looks ok:
		rightHandTransform.gameObject.layer = LayerMask.NameToLayer("Player Item");
		foreach (Transform trans in rightHandChildren)
		{
			trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
		}

		leftHandTransform.gameObject.layer = LayerMask.NameToLayer("Player Item");
		foreach (Transform trans in leftHandChildren)
		{
			trans.gameObject.layer = LayerMask.NameToLayer("Player Item");
		}

        //reactivate item
        currentItemNextScript.active = true;



        grabTime = Vector3.Distance(currentItemNext.transform.position, handTransform.TransformPoint(currentItemNextScript.localHoldPosition)) * grabTimeMod;
        grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

        //move item to preferred hold position
        LeanTween.moveLocal(currentItemNext, currentItemNextScript.localHoldPosition, grabTime).setEase(LeanTweenType.easeOutQuad);

        //move item to preferred hold rotation
        //hakan

        //LeanTween.rotateLocal(currentItemNext, currentItemNextScript.localHoldRotation, grabTime).setEase(LeanTweenType.easeOutExpo);


        //currentItemNext.transform.localRotation = Quaternion.RotateTowards(currentItemNext.transform.localRotation, Quaternion.Euler(currentItemNextScript.localHoldRotation), 0.5f);

        currentItemNext.transform.DOLocalRotateQuaternion(Quaternion.Euler(currentItemNextScript.localHoldRotation), grabTime).SetEase(Ease.OutQuad);

        bool doneWithPhase2 = false;


        if (!fromHands){


	        if(!useRightHand){

	            leftHandTransform.parent = handTransform;

                grabTime = Vector3.Distance(leftHandTransform.transform.position, currentItemNextScript.leftHandHold.position) * grabTimeMod;
                grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

	            //rotate left hand to left hand hold on item
	            LeanTween.rotateLocal(leftHandTransform.gameObject, (Quaternion.Euler(currentItemNextScript.localHoldRotation) * Quaternion.Euler(currentItemNextScript.leftHandHold.localEulerAngles))
	                         .eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuad);

	            //move left hand to left hand hold on item
                LeanTween.move(leftHandTransform.gameObject, currentItemNextScript.leftHandHold, grabTime).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>{
                    doneWithPhase2 = true;
                });


	        }
	        else{
	            rightHandTransform.parent = handTransform;

                grabTime = Vector3.Distance(rightHandTransform.transform.position, currentItemNextScript.rightHandHold.position) * grabTimeMod;
                grabTime = Mathf.Clamp(grabTime, grabTimeLower, grabTimeUpper);

	            //rotate right hand to right hand hold on item
	            LeanTween.rotateLocal(rightHandTransform.gameObject, (Quaternion.Euler(currentItemNextScript.localHoldRotation) * Quaternion.Euler(currentItemNextScript.rightHandHold.localEulerAngles))
	                         .eulerAngles, grabTime).setEase(LeanTweenType.easeOutQuad);

	            //move right hand to right hand hold on item
                LeanTween.move(rightHandTransform.gameObject, currentItemNextScript.rightHandHold, grabTime).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>{
                    doneWithPhase2 = true;
                });
	        }
        }




        yield return new WaitUntil(()=>doneWithPhase2);


        leftHandTransform.parent = handTransform;

        //nullify working variables
        if (!fromHands){

			currentItemNext = null;
			currentItemNextScript = null;   
        }

        currentItemPrev = null;
        currentItemPrevScript = null;

        stateChange = false;


        inventoryIndex = nextSlot;

	}


}

public static class EM
{
	public static int[] FindAllIndexof<T>(this IEnumerable<T> values, T val)
	{
		return values.Select((b, i) => object.Equals(b, val) ? i : -1).Where(i => i != -1).ToArray();
	}
}
