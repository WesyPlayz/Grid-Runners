using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj_State : MonoBehaviour
{
    // This script is for all objects with traits reguarding detection and savable state.

    [Header("Object Variables")]
    public GameObject collided_Obj;
    public GameObject collided_Entity;
    public Vector2 collided_Surface;
    public GameObject trinary_Obj;

    public float Health;
    public float Speed;
    public float grivity;

    public int Item_ID;

    [Header("Interaction Variables")]
    public float Reach;

    [Header("Jump Variables")]
    public float jump_Force;

    public float jump_Cooldown;

    [Header("Attack Variables")]
    public float Damage;

    public float Attack_Cooldown;

    public float bullet_Speed;

    [Header("Collision Variables")]
    public int total_Collisions;

    public Vector2 last_Wall_Contact;
    public Vector2 last_Floor_Contact;
    public Vector2 last_Ceiling_Contact;
}