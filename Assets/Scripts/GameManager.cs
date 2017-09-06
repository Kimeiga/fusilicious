using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager GM;

    public List<Player> players = new List<Player>();

    public Text timeText;
    public float roundTime = 15;


    private void Awake()
    {
        if (GM != null)
        {
            Destroy(GM);
        }
        else
        {
            GM = this;
        }

        DontDestroyOnLoad(this);
    }


    // Use this for initialization
    void Start()
    {
        GameObject[] playerGOs = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject go in playerGOs){
            //this means that if you set some of them in the inspector, they won't be overwrited
            //(I don't know why this would be useful)

            players.Add(go.GetComponent<Player>());
        }



    }

    // Update is called once per frame
    void Update()
    {
        float remainingTime = roundTime - Time.time;

        int minutes = (int) remainingTime / 60;
        int seconds = (int) remainingTime % 60;


        timeText.text = minutes + ":" + seconds;
    }

    //Behavior I want:
    //player looks at item, when it is able to be grabbed by a player, this script makes it glow the player's color
    //however, if another player aims at it while it is being aimed at, it has a black outline (or whatever the contested color will be)
    //when a player stops aiming at an item, and nobody is aiming at it, then it stops glowing

    //oneshots for enter aiming and exit aiming for each item
    //may need a dictionary with each item and owejfpoiajwefpoaiwjef
    //what if i just make the item do it itself........................................

    //the item would just store how many players are looking at it, and the player inventory script would tell it when it started looking at it and when it stopped
    //the item script would use that to control its own glowing

    //so this code should be on item and not on game manager
}