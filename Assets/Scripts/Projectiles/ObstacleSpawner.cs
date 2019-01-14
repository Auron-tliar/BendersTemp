using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : Projectile
{
    public GameObject ObstaclePrefab;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Terrain")
        {
            GameObject temp = Instantiate(ObstaclePrefab, transform.position, transform.rotation, transform.parent);
            Debug.Log("Spawning obstacle at " + temp.transform.position);
            Destroy(gameObject);
        }
    }
}
