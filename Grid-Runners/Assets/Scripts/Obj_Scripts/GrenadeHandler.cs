using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeHandler : MonoBehaviour
{
    public bool imediate_Explosion = false;
    private bool startedCharging = false;

    public LayerMask hit_Layer;
    public LayerMask Block_Layer;

    public GameObject boomparticle;
    public AudioSource boomy;

    public float blast_Radius;
    public float blast_Delay;

    public int max_dmg;
    public int min_dmg;

    private Collider[] Hits;

    public AudioSource mySpeaker;
    public float volume;
    public AudioClip charge;
    public AudioClip boom;


    void Update()
    {
        if (!startedCharging)
        {
            mySpeaker.PlayOneShot(charge);
            startedCharging = true;
        }

        if (blast_Delay <= 0)
            explode();
        else
            blast_Delay -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (imediate_Explosion)
            explode();
    }

    void explode()
    {
        GameObject b = Instantiate(boomparticle, transform.position, transform.rotation);
        Destroy(b, 1);
        mySpeaker.PlayOneShot(boom, volume);
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit))
        {
            int hits = Physics.OverlapSphereNonAlloc(hit.point, blast_Radius, Hits, hit_Layer);

            for (int i = 0; i < hits; i++)
            {
                if (Hits[i].TryGetComponent<UserHandler>(out UserHandler UH))
                {
                    float distance = Vector3.Distance(hit.point, Hits[i].transform.position);

                    if (!Physics.Raycast(hit.point, (Hits[i].transform.position - hit.point).normalized, distance, Block_Layer.value))
                    {
                        UH.Damage(Mathf.FloorToInt(Mathf.Lerp(max_dmg, min_dmg, distance / blast_Radius)));
                    }
                }
            }
        }
        Destroy(gameObject);
    }
}