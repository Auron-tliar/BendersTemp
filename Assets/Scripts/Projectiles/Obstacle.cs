using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float Duration = 4f;
    public float TransitionTime = 1f;
    public float Height = 1f;

    public AudioClip OnSound;
    public AudioClip OffSound;

    private float _startTime;
    private float _setTime, _decayTime, _endTime;
    private float _deltaHeight;

    private bool _offSoundToStart = true;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _startTime = Time.time;
        _setTime = _startTime + TransitionTime;
        _decayTime = _setTime + Duration;
        _endTime = _decayTime + TransitionTime;
        _deltaHeight = Height / TransitionTime;
        if (OnSound != null)
        {
            _audioSource.PlayOneShot(OnSound);
        }
    }

    private void Update()
    {
        if (Time.time >= _endTime)
        {
            Destroy(gameObject);
        }
        else if (Time.time >= _decayTime)
        {
            if (_offSoundToStart)
            {
                if (OffSound != null)
                {
                    _audioSource.PlayOneShot(OffSound);
                }
                _offSoundToStart = false;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y - _deltaHeight * Time.deltaTime,
                transform.position.z);
        }
        else if (Time.time >= _setTime)
        { }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + _deltaHeight * Time.deltaTime,
                transform.position.z);
        }
    }
}
