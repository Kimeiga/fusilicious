using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

    public bool lockCursor = true;

    public Texture2D crosshairImage;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per fram
	void Update()
	{

        if(lockCursor){
            if (Input.GetKey(KeyCode.Escape))
	        {
                Cursor.lockState = CursorLockMode.None;
	            Cursor.visible = true;
	        }
	        else
	        {
                Cursor.lockState = CursorLockMode.Locked;
	            Cursor.visible = false;
	        }
        }
	}

    void OnGUI()
    {
        float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
        float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
    }
}
