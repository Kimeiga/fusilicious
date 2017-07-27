using UnityEngine;
using System.Collections;

public class Sway : MonoBehaviour
{


	private float xSway;
	private float ySway;
	public float moveAmount = 1;
	public float moveSpeed = 2;

	private Vector3 basePosition;

	private Vector3 swayOffset;


	// Use this for initialization
	void Start()
	{


		basePosition = transform.localPosition;

	}

	// Update is called once per frame
	void Update()
	{

		xSway = Input.GetAxis("Mouse X") * Time.deltaTime * moveAmount;
		ySway = Input.GetAxis("Mouse Y") * Time.deltaTime * moveAmount;

		Vector3 swayOffsetTarget = new Vector3(xSway, ySway, 0);
		swayOffset = Vector3.Lerp(swayOffset, swayOffsetTarget, Time.deltaTime * moveSpeed);

		transform.localPosition = basePosition + swayOffset;

	}


}
