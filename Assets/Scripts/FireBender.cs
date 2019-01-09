using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBender : Bender
{
    public GameObject FirePushPrefab;

    private float _startTime;

    /*private void Start()
    {
        _startTime = Time.time;
    }*/

    private new void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        switch (State)
        {
            case States.Casting1:
            case States.Casting2:
                Projectile projectilePush = Instantiate(FirePushPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectilePush.SetDirection(transform.forward);
                FinishCast();
                break;
            case States.Casting3:

                // !!!!!!!!!!! Recover !!!!!!!!!!!!!!!! //

                FinishCast();
                break;
            default:
                break;
        }
    }
}
