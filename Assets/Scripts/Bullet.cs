using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject bulletActual;
    public Renderer ren;
    private Material mat;
    public Color toColor = Color.black;
    public Color fromColor = Color.red;
    public float tweenTime = 1;
    public float scaleMod = 0.5f;

	// Use this for initialization
	void Start () {

        mat = ren.material;

        mat.SetColor("_Color", fromColor);

        LeanTween.value(gameObject, mat.color, toColor, 1).setEase(LeanTweenType.easeOutExpo).setOnUpdate((Color val) => { mat.SetColor("_Color", val); });

        LeanTween.scale(gameObject, gameObject.transform.localScale * scaleMod, tweenTime).setEase(LeanTweenType.easeOutExpo);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
