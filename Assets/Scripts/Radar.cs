using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarObject
{
    public Image icon { get; set; }
    public GameObject owner { get; set; }
    public Sprite oriSprite { get; set; }
}


public class Radar : MonoBehaviour {

    public Transform playerTrans;
    public float radarScale = 2;
    

    public RectTransform innerRectTrans;
    private Rect innerRect;

    public float margin;

    public Image arrow;
    public Sprite arrowSprite;

    public static List<RadarObject> radarObjects = new List<RadarObject>();
    private List<Sprite> radarSprites = new List<Sprite>();

    public static void RegisterRadarObject(GameObject go, Image im)
    {
        Image image = Instantiate(im);
        radarObjects.Add(new RadarObject() { icon = image, owner = go , oriSprite = image.sprite});
        
    }

    public static void RemoveRadarObject(GameObject go)
    {
        List<RadarObject> newList = new List<RadarObject>();

        for (int i = 0; i < radarObjects.Count; i++)
        {
            if(radarObjects[i].owner == go)
            {
                Destroy(radarObjects[i].icon);
                continue;
            }
            else
            {
                newList.Add(radarObjects[i]);
            }
        }

        radarObjects.RemoveRange(0, radarObjects.Count);
        radarObjects.AddRange(newList);
    }

    void DrawRadarIcons()
    {
        foreach(RadarObject ro in radarObjects)
        {
            Vector3 radarPos = ro.owner.transform.position - playerTrans.position;
            //radarPos *= 0.01f;
            Vector3 playerPos2D = new Vector3(playerTrans.position.x, 0, playerTrans.position.z);
            Vector3 roOwnerPos2D = new Vector3(ro.owner.transform.position.x, 0, ro.owner.transform.position.z);

            float distToObject = Vector3.Distance(playerPos2D, roOwnerPos2D) * radarScale;
            float deltaY = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - playerTrans.eulerAngles.y;

            radarPos.x = distToObject * Mathf.Cos(deltaY * Mathf.Deg2Rad) * -1;
            radarPos.z = distToObject * Mathf.Sin(deltaY * Mathf.Deg2Rad);


            ro.icon.transform.SetParent(this.transform);

            Vector3 localPos = new Vector3(radarPos.x, radarPos.z, 0);


            

            if(localPos.x < innerRect.xMin || localPos.x > innerRect.xMax || localPos.y < innerRect.yMin || localPos.y > innerRect.yMax)
            {
                //draw a triangle pointing to where it should be...
                ro.icon.sprite = arrowSprite;
                //ro.icon.transform.rotation = Quaternion.LookRotation(new Vector3(radarPos.x, radarPos.z, 0) + this.transform.position);

                Vector3 rot = ro.icon.transform.localRotation.eulerAngles;

                ro.icon.transform.localRotation = Quaternion.Euler(0, 0, -deltaY + 90);

            }
            else
            {
                ro.icon.sprite = ro.oriSprite;

                ro.icon.transform.localRotation = Quaternion.identity;
            }


            localPos.x = Mathf.Clamp(localPos.x, innerRect.xMin, innerRect.xMax);
            localPos.y = Mathf.Clamp(localPos.y, innerRect.yMin, innerRect.yMax);


            ro.icon.transform.position = localPos + this.transform.position;
        }
    }

	// Use this for initialization
	void Start () {
        

        innerRect = innerRectTrans.rect;

	}
	
	// Update is called once per frame
	void Update () {
        DrawRadarIcons();
	}
}
