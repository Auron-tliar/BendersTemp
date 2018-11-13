using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPush : Projectile
{
    public float Power = 10f;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Bender")
        {
            Bender bender = collider.GetComponent<Bender>();
            //bender.NavAgent.SetDestination(transform.position);
            bender.NavAgent.enabled = false;
            collider.attachedRigidbody.AddForce(_rigidbody.velocity * Power);
            bender.GotHit();
            Destroy(gameObject);
        }
    }
}
