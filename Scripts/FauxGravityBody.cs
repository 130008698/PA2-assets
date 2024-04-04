using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityBody : MonoBehaviour
{
    // Start is called before the first frame update
    private FauxGravityAttractor attractor;
    private Transform myTransform;
    void Start()
    {
        attractor = GameObject.Find("forest").GetComponent<FauxGravityAttractor>();;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<Rigidbody>().useGravity = false;
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        attractor.Attract(myTransform);
    }
}
