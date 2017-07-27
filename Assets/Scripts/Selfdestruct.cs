using UnityEngine;
using System.Collections;

public class Selfdestruct : MonoBehaviour {

    public float time;
    public GameObject particle;
    private float startTime;

	// Use this for initialization
	void Start () {

        startTime = Time.time;

	}

    // Update is called once per frame
    void Update()
    {

        if (Time.time > startTime + time)
        {

            if (particle != null)
            {
                Instantiate(particle, transform.position, transform.rotation);
            }

            Destroy(gameObject);

        }
    }
}
