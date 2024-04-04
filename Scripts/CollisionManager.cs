using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private MovementManager movementManager;
    // Start is called before the first frame update
    private void Start()
    {
        // Assumes that the MovementManager is attached to the parent object or elsewhere in the hierarchy.
        movementManager = GetComponentInParent<MovementManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            movementManager.SetGroundedState(true);
        }
    }
}
