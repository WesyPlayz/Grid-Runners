using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeHandler : MonoBehaviour
{
    public bool imediate_Explosion = false;
    public bool startedCharging = false;

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
    public AudioSource pin;
    public AudioClip Fuse;
    public AudioClip Boom;

    void Start()
    {
        mySpeaker.PlayOneShot(Fuse, volume);
    }
    void Update()
    {
        if (!startedCharging)
        {
            mySpeaker.PlayOneShot(charge);
            startedCharging = true;
        }
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
        mySpeaker.PlayOneShot(Boom, volume);
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit))
        {
            int hits = Physics.OverlapSphereNonAlloc(hit.point, blast_Radius, Hits, hit_Layer);

            for (int i = 0; i < hits; i++)
            {
                if (Hits[i].TryGetComponent<Hitboxhandler>(out Hitboxhandler HB))
                {
                    float distance = Vector3.Distance(hit.point, Hits[i].transform.position);

                    if (!Physics.Raycast(hit.point, (Hits[i].transform.position - hit.point).normalized, distance, Block_Layer.value))
                    {

                        HB.GetComponent<UserHandler>().Damage(Mathf.FloorToInt(Mathf.Lerp(max_dmg, min_dmg, distance / blast_Radius)));
                    }
                }
            }
        }
        Destroy(gameObject);
    }

    public IEnumerator fuse_delay()
    {
        yield return new WaitForSeconds(blast_Delay);
        explode();
    }
}