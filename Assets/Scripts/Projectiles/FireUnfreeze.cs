using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireUnfreeze : Projectile
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Bender" && _caster != collider.GetComponent<Bender>().Name)
        {
            Bender bender = collider.GetComponent<Bender>();

            if (bender.State == Bender.States.Frozen)
            {
                bender.GotUnfrozen();
            }

            PlayHitSound(bender.AudioSourcePlayer);
            Destroy(gameObject);
        }
    }
}
