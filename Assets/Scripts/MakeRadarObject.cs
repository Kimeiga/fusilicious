using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeRadarObject : MonoBehaviour {

    public Image icon;

	// Use this for initialization
	void Start () {

        Radar.RegisterRadarObject(gameObject, icon);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        Radar.RemoveRadarObject(gameObject);
    }
}
