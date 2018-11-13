using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float Duration = 1f;

    private float _startTime;

    private void Start()
    {
        _startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - _startTime >= Duration)
        {
            Destroy(gameObject);
        }
    }
}