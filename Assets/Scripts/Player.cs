using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class Player : MonoBehaviour, IComparable<Player> {

    public string playerName;
    public Color color;
    private float health;
    public float startingHealth = 100;

    private float armor;
    public float startingArmor = 0;


    //public Text healthText;
    //public Text armorText;

    public float Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;

            //healthText.text = value.ToString() + "+";
        }
    }

    public float Armor
    {
        get
        {
            return armor;
        }

        set
        {
            armor = value;

            //armorText.text = value.ToString() + "×";
        }
    }

    // Use this for initialization
    void Start () {
        Health = startingHealth;
        Armor = startingArmor;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Player(string newName, Color newColor){
        playerName = newName;
        color = newColor;
    }

	//This method is required by the IComparable
	//interface. 
    public int CompareTo(Player other)
	{
        if (other == null)
        {
            return 1;
        }

		if (other.playerName == playerName)
		{
			return 0;
		}

        //Return the difference in power.
        String[] playerNames = {
            playerName,
            other.playerName
        };

        Array.Sort(playerNames);


        if(playerNames[0] == playerName){
            return 1;
        }
        if(playerNames[0] == other.playerName){
            return -1;
        }

        return 0;
	}

}
