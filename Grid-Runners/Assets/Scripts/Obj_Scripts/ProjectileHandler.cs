using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    public int dmg;
    void OnCollisionEnter(Collision obj) // Hit Object:
    {
        if (obj.gameObject.layer == LayerMask.NameToLayer("Terrain") && !obj.collider.isTrigger)
            Destroy(gameObject);
        if (obj.gameObject.CompareTag("Grenade"))
            Destroy(gameObject);
        Destroy(gameObject);
    }
}