using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBender : Bender
{
    public Transform ProjectileSpawnPoint;
    public float SpeedBuffSpeed = 8f;
    public float SpeedBuffDuration = 10f;
    public ParticleSystem BuffParticleSystem;
    public GameObject AirPushPrefab;
    public GameObject AirPullPrefab;

    private float _startTime;
    private float _keptStSpeed;

    private new void Start()
    {
        base.Start();
        BuffParticleSystem.Stop();
        ParticleSystem.MainModule mod = BuffParticleSystem.main;
        mod.duration = SpeedBuffDuration;
    }

    /*private new void Update()
    {
        if (Time.time - _startTime >= 2f)
        {
            
        }

        base.Update();
    }*/

    protected override void Cast()
    {
        switch (State)
        {
            case States.Casting1:
                _keptStSpeed = StandardSpeed;
                StandardSpeed = SpeedBuffSpeed;
                NavAgent.speed = SpeedBuffSpeed;
                BuffParticleSystem.Play();
                StartCoroutine(SpeedBuff());
                FinishCast();
                break;
            case States.Casting2:
                AirPush projectilePush = Instantiate(AirPushPrefab, ProjectileSpawnPoint.position, 
                    new Quaternion()).GetComponent<AirPush>();
                projectilePush.SetDirection(transform.forward);
                FinishCast();
                break;
            case States.Casting3:
                AirPull projectilePull = Instantiate(AirPullPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<AirPull>();
                projectilePull.SetDirection(transform.forward);
                FinishCast();
                break;
            default:
                break;
        }
    }

    protected IEnumerator SpeedBuff()
    {
        yield return new WaitForSeconds(SpeedBuffDuration);
        StandardSpeed = _keptStSpeed;
        NavAgent.speed = _keptStSpeed;
    }
}
