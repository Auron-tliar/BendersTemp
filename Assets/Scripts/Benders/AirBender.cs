using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBender : Bender
{
    public float SpeedBuffSpeed = 8f;
    public float SpeedBuffDuration = 10f;
    public ParticleSystem BuffParticleSystem;
    public GameObject AirPushPrefab;
    public GameObject AirPullPrefab;

    private float _startTime;
    private float _keptStSpeed;

    protected override void Start()
    {
        base.Start();
        BuffParticleSystem.Stop();
        ParticleSystem.MainModule mod = BuffParticleSystem.main;
        mod.duration = SpeedBuffDuration;
    }


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
                Projectile projectilePush = Instantiate(AirPushPrefab, ProjectileSpawnPoint.position, 
                    new Quaternion()).GetComponent<Projectile>();
                projectilePush.SetDirection(Name, transform.forward);
                FinishCast();
                break;
            case States.Casting3:
                Projectile projectilePull = Instantiate(AirPullPrefab, ProjectileSpawnPoint.position,
                    new Quaternion()).GetComponent<Projectile>();
                projectilePull.SetDirection(Name, transform.forward);
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
