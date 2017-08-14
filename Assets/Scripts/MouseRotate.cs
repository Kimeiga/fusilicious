using UnityEngine; using System.Collections;

using UnityEngine.UI;

/// MouseLook rotates the transform based on the mouse delta. /// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character: /// - Create a capsule. /// - Add a rigid body to the capsule /// - Add the MouseLook script to the capsule. /// -> Set the mouse look to use LookX. (You want to only turn character but not tilt it) /// - Add FPSWalker script to the capsule

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform. /// - Add a MouseLook script to the camera. /// -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)

public class MouseRotate : MonoBehaviour {

    public bool canRotate;

public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
public RotationAxes axes = RotationAxes.MouseXAndY;
public float sensitivityX = 2F;
public float sensitivityY = 2F;

public float minimumX = -360F;
public float maximumX = 360F;

public float minimumY = -90F;
public float maximumY = 90F;

public float rotationX = 0F;
public float rotationY = 0F;

Quaternion originalRotation;

    public float xOffset;
    public float yOffset;

void Update ()
{

        

        if(Input.GetButtonDown("Increase Sensitivity"))
        {
            sensitivityX += 0.3f;
            sensitivityY += 0.3f;
        }
        if (Input.GetButtonDown("Decrease Sensitivity"))
        {
            sensitivityX -= 0.3f;
            sensitivityY -= 0.3f;
        }

        if (canRotate) {

            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);
                

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX + xOffset, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY + yOffset, -Vector3.right);

                transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX + xOffset, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;

            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY + yOffset, -Vector3.right);
                transform.localRotation = originalRotation * yQuaternion;
            }
        }
	
}

void Start ()
{
	// Make the rigid body not change rotation
	if (GetComponent<Rigidbody>())
		GetComponent<Rigidbody>().freezeRotation = true;
	originalRotation = transform.localRotation;

        canRotate = true;
}

public static float ClampAngle (float angle, float min, float max)
{
	if (angle < -360F){
			angle += 360F;
		}
	if (angle > 360F){
		angle -= 360F;
		}
	return Mathf.Clamp (angle, min, max);
}

    public void Reset()
    {
        rotationX = 0;
        rotationY = 0;
    }
}