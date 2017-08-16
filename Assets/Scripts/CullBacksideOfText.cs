using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CullBacksideOfText : MonoBehaviour {

	void Awake()
	{
        TextMeshPro textObject = gameObject.GetComponent<TextMeshPro>();
		textObject.enableCulling = true;

	}

}
