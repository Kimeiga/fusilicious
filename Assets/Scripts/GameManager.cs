using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager GM;

    public List<Player> players = new List<Player>();

    public Text minutesText;
    public Text secondsText;
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

        minutesText.text = ((int) remainingTime / 60).ToString("00");
        secondsText.text = ((int) remainingTime % 60).ToString("00");
        

    }
    
}