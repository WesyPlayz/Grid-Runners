using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Data : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public GameObject weapon_Prefab;
        public Sprite Icon;

        public int upgrade_Lvl;

        [Header("Melee")]
        public bool is_Melee;

        [Header("Ranged")]
        public bool is_Ranged;

        public GameObject projectile;

        public float fire_Rate;

        public int max_Ammo;

        public int damage;
    }
    public List<Weapon> Weapons;

    [System.Serializable]
    public class Ordinance
    {
        public GameObject ordinance_Prefab;

        public ParticleSystem explosion;
    }
    public List<Ordinance> Ordinances;
}