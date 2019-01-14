using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBlock : Projectile
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Projectile")
        {
            Destroy(collider.gameObject);
            Destroy(gameObject);
        }
    }
}
