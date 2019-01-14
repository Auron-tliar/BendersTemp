using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObstacle : Obstacle
{
    public AudioClip HitSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bender")
        {
            Bender bender = collision.gameObject.GetComponent<Bender>();
            //bender.NavAgent.SetDestination(transform.position);
            if (bender.NavAgent.enabled)
            {
                bender.NavAgent.isStopped = true;
            }
            bender.GotHit();
            bender.AudioSourcePlayer.PlayOneShot(HitSound);
        }
    }
}
