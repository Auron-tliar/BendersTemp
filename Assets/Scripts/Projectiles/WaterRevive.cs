using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRevive : Projectile
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Bender" && _caster != collider.GetComponent<Bender>().Name)
        {
            Bender bender = collider.GetComponent<Bender>();

            bender.GotRevived();

            PlayHitSound(bender.AudioSourcePlayer);
            Destroy(gameObject);
        }
    }
}
