using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj_State : MonoBehaviour
{
    // This script is for all objects with traits reguarding detection and savable state.

    [Header("Object Variables")]
    public float Health;

    public int Team_ID;

    [Header("Inventory Variables")]
    public GameObject collided_Entity;
    public GameObject collided_Item;

    public int Primary_ID;
    public int Secondary_ID;
    public int Ability_ID;

    [Header("Interaction Variables")]
    public float Reach;

    [Header("Movement Variables")]
    public float Speed;
    public float jump_Force;
    public float gravity;

    [Header("Attack Variables")]
    public float Damage;
    public int Bullets_Per_Shot;
    public float Attack_Cooldown;
    public float bloom;
    public float recoil;
    public float bullet_Speed;

    [Header("Collision Variables")]
    public GameObject collided_Wall;
    public GameObject collided_Floor;
    public GameObject collided_Ceiling;

    public Vector3 last_Wall_Contact;
    public Vector3 last_Floor_Contact;
    public Vector3 last_Ceiling_Contact;

    public int total_Collisions;
}