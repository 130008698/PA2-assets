using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowScript : MonoBehaviour
{

    void FixedUpdate()
    {
        Destroy(gameObject, 5f);
    }
}
