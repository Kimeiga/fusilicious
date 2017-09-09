using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public static HUD hud;


    public Text ammoText;
    public Text[] slotTexts;
    public GameObject slotSelector;

    public Text healthText;
    public Text armorText;

    public Text driftText;
    public GameObject driftDirectionImage;

    public Radar radar;
    public Transform lookTrans;

    private void Awake()
    {
        if (hud == null)
            hud = this;
        else if(hud != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
