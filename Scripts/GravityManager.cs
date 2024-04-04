using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    private Transform planetCenter; // Assign this in the editor
    public float gravityStrength = 9.81f; // You can adjust this as needed
    private GameObject myChild;
    private Rigidbody myRB;
    private Transform cameraOffset;
    void Start()
    {
        myChild = transform.GetChild(0).gameObject;
        myRB = myChild.GetComponent<Rigidbody>();
        myRB.useGravity = false; // Disable Unity's gravity
        if (planetCenter == null)
        {
            planetCenter = GameObject.Find("Sphere Center").transform;
            if (planetCenter == null)
            {
                Debug.LogError("Planet center not found. Please ensure there is a GameObject named 'PlanetCenter' in the scene.");
                return;
            }
        }
        if(cameraOffset == null)
        {
            cameraOffset = GameObject.Find("Camera Offset").transform;
            if (cameraOffset == null)
            {
                Debug.LogError("no camera offset");
                return;
            }
        }
    }

    void FixedUpdate()
    {
        // Calculate the direction from the object to the center of the planet
        Vector3 gravityDirection = (myRB.transform.position - planetCenter.position).normalized;
        Debug.Log(gravityDirection);
        // Apply the gravity as a force towards the planet's center
        myRB.AddForce(-gravityDirection * gravityStrength, ForceMode.Acceleration);

        // Adjust the object's rotation
        AlignToSurface(gravityDirection);
    }

    void AlignToSurface(Vector3 gravityDirection)
    {
        // Assumes that the object should be oriented such that its local up vector opposes gravity
        Quaternion targetOrientation = Quaternion.FromToRotation(transform.up, -gravityDirection) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetOrientation, 50 * Time.deltaTime);

        if (cameraOffset != null)
        {
            cameraOffset.up = gravityDirection;
        }
    }

}
