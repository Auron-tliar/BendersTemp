using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallZone : MonoBehaviour
{
    public float GravityModifyer = 100f;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Bender")
        {

            other.GetComponent<Bender>().NavAgent.enabled = false;
            other.attachedRigidbody.constraints = 
                RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            other.attachedRigidbody.AddForce(Physics.gravity * GravityModifyer);
        }
    }
}
