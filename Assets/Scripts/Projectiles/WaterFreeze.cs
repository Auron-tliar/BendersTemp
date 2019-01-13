using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFreeze : Projectile
{
    public float Duration = 2f;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Bender" && _caster != collider.GetComponent<Bender>().Name)
        {
            Bender bender = collider.GetComponent<Bender>();
            //bender.NavAgent.SetDestination(transform.position);
            if (bender.NavAgent.enabled)
            {
                bender.NavAgent.isStopped = true;
            }

            bender.GotFrozen(Duration);
            PlayHitSound(bender.AudioSourcePlayer);
            Destroy(gameObject);
        }
    }
}
