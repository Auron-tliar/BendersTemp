﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBender : Bender
{
    public GameObject EarthPushPrefab;
    public GameObject EarthBlockPrefab;
    public GameObject EarthObstacleSpawnerPrefab;

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
                Projectile projectileBlock = Instantiate(EarthBlockPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectileBlock.SetDirection(Name, transform.forward);
                FinishCast();
                break;
            case States.Casting2:
                Projectile projectilePush = Instantiate(EarthPushPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectilePush.SetDirection(Name, transform.forward);
                FinishCast();
                break;
            case States.Casting3:
                Projectile projectileObstacle = Instantiate(EarthObstacleSpawnerPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectileObstacle.SetDirection(Name, transform.forward + transform.up);
                FinishCast();
                break;
            default:
                break;
        }
    }
}
