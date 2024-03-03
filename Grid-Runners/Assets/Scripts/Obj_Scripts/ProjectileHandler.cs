using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    void OnCollisionEnter(Collision obj) // Hit Object:
    {
        if (obj.gameObject.layer == LayerMask.NameToLayer("Terrain") && !obj.collider.isTrigger)
            Destroy(gameObject);
    }
}