using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBender : Bender
{
    public Transform ProjectileSpawnPoint;
    //public GameObject WaterPushPrefab;

    private float _startTime;

    /*private void Start()
    {
        _startTime = Time.time;
    }*/

    private new void Update()
    {
        /*if (Time.time - _startTime >= 2f)
        {
            BenderAnimator.SetTrigger("Attack1Trigger");
            AirPush projectile = Instantiate(AirPushPrefab, ProjectileSpawnPoint).GetComponent<AirPush>();
            projectile.SetDirection(transform.forward);
            _startTime = float.PositiveInfinity;
        }*/

        base.Update();
    }

    protected override void Cast()
    {
        throw new System.NotImplementedException();
    }
}
