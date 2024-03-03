using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponHandler : MonoBehaviour
{
    public int Weapon_ID;
    public int damage;

    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;

    public int maxAmmo;
    public int maxMagSize;
    public int ammo;
    public int bulletsPerShot;

    public bool canHoldToShoot;
    public bool shooting;
    public bool canShoot;
    public bool reloading;

    public Transform attackPoint;
    public RaycastHit rayHit;

    public GameObject user;
    private UserHandler UH;

    /*
    // Start is called before the first frame update
    void Start()
    {
        UH = user.GetComponent<UserHandler>();
        ammo = maxMagSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (UH.can_Attack && !reloading && ammo > 0)
        {
            if (canHoldToShoot)
            {
                if (Input.GetButton("Fire1"))
                {
                    shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire"))
                {
                    shoot();
                }
            }
        }
        if (Input.GetButtonDown("Reload") && !reloading)
        {
            reloading = true;
            UH.can_Attack = false;
            StartCoroutine(Reload(reloadTime));
        }
    }

    void shoot()
    {
        UH.can_Attack = false;
        StartCoroutine(UH.AttackCooldown(timeBetweenShots));

        ammo -= bulletsPerShot;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = UH.fpsCam.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(UH.fpsCam.transform.position, direction, out rayHit, range, UH.enemyTeam))
        {
            if (rayHit.collider.CompareTag(UH.enemyTeamTag))
                rayHit.collider.GetComponent<UserHandler>().hit(damage);
        }
    }

    IEnumerator Reload(float time)
    {
        yield return new WaitForSeconds(time);
        UH.can_Attack = true;
        reloading = false;
        maxAmmo -= (maxMagSize - ammo);
        ammo = maxMagSize;
    } */
}
