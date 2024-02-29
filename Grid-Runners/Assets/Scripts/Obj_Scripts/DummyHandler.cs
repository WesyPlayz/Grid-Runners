using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyHandler : MonoBehaviour
{
    // Object Variables:
    private Vector3 start_Pos;

    // Hit Registration Variables:
    [Range(0, 2)]
    public float time = 0.2f;
    [Range(0, 2)]
    public float distance = 0.1f;
    [Range(0, 0.1f)]
    public float delayBetweenShakes;

    // Variable Initialization:
    void Start()
    {
        start_Pos = transform.position;
    }

    // Hit Detection System:
    private void Update() // Melee Based:
    {
        if (Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            StartCoroutine(Shake(0));
        }
    }
    void OnCollisionEnter(Collision projectile) // Ranged Based:
    {
        if (projectile.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            StopAllCoroutines();
            StartCoroutine(Shake(0));
        }
    }

    // Hit Registration System:
    public IEnumerator Shake(float timer)
    {
        while (timer < time)
        {
            timer += Time.deltaTime;
            Vector3 randomPos = start_Pos + (Random.insideUnitSphere * distance);
            transform.position = randomPos;
            if (delayBetweenShakes > 0)
                yield return new WaitForSeconds(delayBetweenShakes);
            else
                yield return null;
        }
        transform.position = start_Pos;
    }

    // Editor Tasks:
    private void OnValidate()
    {
        if (delayBetweenShakes > time)
            delayBetweenShakes = time;
    }
}