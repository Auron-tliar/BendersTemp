using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float LifeTime = 5f;
    public float Speed = 5f;
    public AudioClip HitSound;
    
    protected Rigidbody _rigidbody;
    protected float _creationTime;
    protected string _caster;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected void Start()
    {
        _creationTime = Time.time;
    }

    public void SetDirection(string caster, Vector3 direction)
    {
        _caster = caster;
        _rigidbody.velocity = direction * Speed;
    }

    protected void Update()
    {
        if (Time.time - _creationTime >= LifeTime)
        {
            Destroy(gameObject);
        }
    }

    protected void PlayHitSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(HitSound);
    }
}
