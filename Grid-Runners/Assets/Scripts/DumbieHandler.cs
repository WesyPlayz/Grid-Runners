using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbiesHandler : MonoBehaviour
{
    private Vector3 startPos;
    private float timer;
    private Vector3 randomPos;
    public GameObject damaged_Particle;
    public AudioSource damaged_Sound;

    private Transform myT;

    [Range(0f, 2f)]
    public float time = 0.2f;
    [Range(0f, 2f)]
    public float distance = 0.1f;
    [Range(0f, 0.1f)]
    public float delayBetweenShakes = 0f;

    private void Awake()
    {
        startPos = transform.position;
        myT = GetComponent<Transform>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Begin();
        }
    }

    private void OnValidate()
    {
        if (delayBetweenShakes > time)
            delayBetweenShakes = time;
    }

    public void Begin()
    {
        print("begin");
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    public IEnumerator Shake()
    {
        timer = 0f;

        while (timer < time)
        {
            timer += Time.deltaTime;

            randomPos = startPos + (Random.insideUnitSphere * distance);

            myT.position = randomPos;

            if (delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(delayBetweenShakes);
            }
            else
            {
                yield return null;
            }
        }

        transform.position = startPos;

        print("did shooketh");
    }

    /*
    void OnCollisionEnter2D(Collision2D sender)
    {
        if (sender.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Obj_State projectile_Data = sender.gameObject.GetComponent<Obj_State>();
            damaged_Particle.GetComponent<ParticleSystem>().Play();
            Destroy(sender.gameObject);

            Begin();
        }
    }
    */
}
