using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    void OnCollisionEnter(Collision projectile) // Hit Object:
    {
        Destroy(gameObject);
    }
}
