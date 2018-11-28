using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public BoundingBox Bounds;
    public float MovementSpeed = 20f;
    public float RotationSpeed = 100f;
    public float ScrollSpeed = 100f;

    public List<GameObject> benders = new List<GameObject>();
    public Vector3 offset;
    public float smoothFactor = 0.5f;


    private Vector3 velocity;

    void LateUpdate ()
    {
        if (benders.Count == 0)
        {
            return;
        }

        Move();
    }

    float GetGreatestDistance()
    {
        Bounds b = GetBounds();
        if(b.size.x < b.size.y)
        {
            return b.size.y;
        }
        else
        {
            return b.size.x;
        }

    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothFactor);
    }

    Vector3 GetCenterPoint()
    {
        if(benders.Count == 1)
        {
            return benders[0].transform.position;
        }
 
        return GetBounds().center;
    }

    Bounds GetBounds()
    {
        var bounds = new Bounds(benders[0].transform.position, Vector3.zero);
        for (int i = 0; i < benders.Count; i++)
        {
          
            bounds.Encapsulate(benders[i].transform.position);
        }
        return bounds;
    }
}

[System.Serializable]
public class BoundingBox
{
    public float XMin, XMax, YMin, YMax, ZMin, ZMax;
}
