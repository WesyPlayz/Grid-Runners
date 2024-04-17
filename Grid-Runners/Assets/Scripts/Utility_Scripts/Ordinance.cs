using UnityEngine;
using UnityEditor;

using static Utilities.Generic;

[CustomEditor(typeof(Ordinance))]
public class Ordinance_Editor : Editor
{
    SerializedObject ser_Obj;

    void OnEnable()
    {
        ser_Obj = new SerializedObject(target);
    }
    public override void OnInspectorGUI()
    {
        ser_Obj.Update();
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
        ser_Obj.ApplyModifiedProperties();
    }
}
[CreateAssetMenu(fileName = "New Ordinance", menuName = "Items/Ordinance")]
public class Ordinance : Item
{
    [Header("Ordinace Attributes")]
    public bool is_Auto;
    public bool is_Impact;

    [Header("Projectile Attributes")]
    public GameObject projectile_Prefab;
    public GameObject held_Grenade;
    private GrenadeHandler grenade_Handler;

    public int max_Ammo;
    public int ammo;
    public float fuse_Time;

    [Range(1, 100)] public float bullet_Speed;
    [Range(0.025f, 5)] public float reload_Speed;
    [Range(0.025f, 5)] public float fire_Rate;
    public override void Attack(UserHandler user_Handler)
    {
        if (!grenade_Handler.startedCharging)
        {
            GameObject gre = Instantiate(projectile_Prefab, user_Handler.fire_Point.transform.position, user_Handler.camera_Handler.user_Camera.transform.rotation, user_Handler.camera_Handler.user_Camera.transform);
            held_Grenade = gre;
            grenade_Handler = held_Grenade.GetComponent<GrenadeHandler>();
            grenade_Handler.startedCharging = true;
            grenade_Handler.StartCoroutine(grenade_Handler.fuse_delay());
            user_Handler.can_Attack = false;
            ammo--;
        }
        else
        {
            held_Grenade.transform.parent = null;
            held_Grenade.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            LinearJump(user_Handler.camera_Handler.user_Camera.transform.forward, user_Handler.throw_force, held_Grenade);
        }
    }

    public override void Aim(UserHandler user_Handler, bool is_ADSing)
    {

    }

    public override void Action(UserHandler user_Handler)
    {

    }
}