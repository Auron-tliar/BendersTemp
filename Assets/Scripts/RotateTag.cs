using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTag : MonoBehaviour
{
    private Vector3 _lookVector;
    private void Update()
    {
        _lookVector = new Vector3(0f, Camera.main.transform.position.y - transform.position.y);

        transform.LookAt(Camera.main.transform.position - _lookVector);
        transform.rotation = Camera.main.transform.rotation;
    }
}
