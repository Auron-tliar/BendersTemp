using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallZone : MonoBehaviour
{
    public float GravityModifier = 0.01f;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HEnter");

        if (other.tag == "Bender")
        {
            if (other.GetComponent<AIAgent>() != null && other.GetComponent<AIAgent>().isInTrainingCamp)
            {
                Debug.Log("Hello");

                other.GetComponent<Bender>().Defeat();

                other.attachedRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                other.attachedRigidbody.velocity = -transform.forward * GravityModifier;

            }
            else
            {
                other.GetComponent<Bender>().enabled = false;
                other.GetComponent<Bender>().NavAgent.enabled = false;
                other.attachedRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                other.attachedRigidbody.velocity = -transform.forward * GravityModifier;
            }
        }
    }
}