using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBender : Bender
{
    public GameObject EarthPushPrefab;

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
        switch (State)
        {
            case States.Casting1:
            case States.Casting2:
                Projectile projectilePush = Instantiate(EarthPushPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectilePush.SetDirection(transform.forward);
                FinishCast();
                break;
            case States.Casting3:
                break;
            default:
                break;
        }
    }
}
