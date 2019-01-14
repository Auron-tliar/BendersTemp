using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthObstacle : Obstacle
{
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collider: " + collider.gameObject.tag);
        if (collider.gameObject.tag == "Projectile")
        {
            Destroy(collider.gameObject);
        }
    }
}
