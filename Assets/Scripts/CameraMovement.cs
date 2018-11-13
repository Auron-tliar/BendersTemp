using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public BoundingBox Bounds;
    public float MovementSpeed = 20f;
    public float RotationSpeed = 100f;
    public float ScrollSpeed = 100f;

	void Update ()
    {
        transform.position += (transform.forward * Input.GetAxis("Vertical") +
            transform.right * Input.GetAxis("Horizontal")) * MovementSpeed * Time.deltaTime -
            transform.up * Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, Bounds.XMin, Bounds.XMax),
            Mathf.Clamp(transform.position.y, Bounds.YMin, Bounds.YMax),
            Mathf.Clamp(transform.position.z, Bounds.ZMin, Bounds.ZMax));
        /*transform.position = new Vector3(Mathf.Clamp(transform.position.x + Input.GetAxis("Horizontal") * MovementSpeed *
            Time.deltaTime, Bounds.XMin, Bounds.XMax), transform.position.y, 
            Mathf.Clamp(transform.position.z + Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime,
            Bounds.ZMin, Bounds.ZMax));*/

        transform.Rotate(new Vector3(0f, Input.GetAxis("Rotation") * RotationSpeed * Time.deltaTime, 0f));
    }
}

[System.Serializable]
public class BoundingBox
{
    public float XMin, XMax, YMin, YMax, ZMin, ZMax;
}
