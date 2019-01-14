using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBender : Bender
{
    public GameObject FirePushPrefab;
    public GameObject FireUnfreezePrefab;
    public GameObject FireObstacleSpawnerPrefab;

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
                Projectile projectileUnfreeze = Instantiate(FireUnfreezePrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectileUnfreeze.SetDirection(Name, transform.forward);
                FinishCast();
                break;
            case States.Casting2:
                Projectile projectilePush = Instantiate(FirePushPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectilePush.SetDirection(Name, transform.forward);
                FinishCast();
                break;
            case States.Casting3:
                Projectile projectileObstacle = Instantiate(FireObstacleSpawnerPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectileObstacle.SetDirection(Name, transform.forward + transform.up);
                FinishCast();
                break;
            default:
                break;
        }
    }
}
