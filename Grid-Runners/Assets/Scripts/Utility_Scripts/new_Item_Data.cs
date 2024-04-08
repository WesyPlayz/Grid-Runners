using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class new_Item_Data : MonoBehaviour
{
    [Header("Item Collection")]
    [SerializeField] public List<Item> Items = new List<Item>{};

    [Header("Ability Collection")]
    [SerializeField] public List<Ability> Abilities = new List<Ability>{};

    [Header("General Data")]
    public float min_Speed_Mod;
    public float max_Speed_Mod;

    [HideInInspector] public int entity_Mask;

    private void Start()
    {
        Item.item_Data = this;

        entity_Mask = 9 << LayerMask.NameToLayer("Entity");
    }

    public void Equip_Weapon(UserHandler user_Handler)
    {

    }

    // Item Coroutines:
    public IEnumerator Attack_Rate(UserHandler user_Handler, float length)
    {
        yield return new WaitForSeconds(length);
        user_Handler.can_Attack = true;
    }

    // Range Specific Coroutines:
    public IEnumerator Reload_Cooldown(UserHandler user_Handler, int capacity, float length)
    {
        yield return new WaitForSeconds(length);
        user_Handler.Ammo = capacity;
        user_Handler.can_Attack = true;
    }
    public IEnumerator Fire_Rate(UserHandler user_Handler, Ranged weapon, bool state)
    {
        while (user_Handler.is_Attacking && user_Handler.Ammo > 0)
        {
            weapon.Attack(user_Handler);
            yield return new WaitForSeconds(weapon.fire_Rate);
        }
        user_Handler.can_Attack = true;
    }

    // Ability Coroutines:
    public IEnumerator Ability_Cooldown(UserHandler user_Handler, float length)
    {
        yield return new WaitForSeconds(length);
    }
}

public abstract class Item : ScriptableObject
{
    [Header("Object Data")]
    public GameObject weapon_Prefab;

    public static new_Item_Data item_Data;

    public int Damage;

    [Range(0, 1)] public float speed_Mod;

    [Header("GUI Data")]
    public Sprite Icon;

    public abstract void Attack(UserHandler user_Handler);
    public abstract void Aim(UserHandler user_Handler, bool is_ADSing);
    public abstract void Action(UserHandler user_Handler);
}

[CreateAssetMenu(fileName = "New Melee", menuName = "Items/Melee")]
public class Melee : Item
{
    public GameObject melee_prefab;

    public float range;
    public float fire_Rate;

    public override void Attack(UserHandler user_Handler)
    {
        switch (user_Handler.secondary_Data.collided_Entity.tag)
        {
            case "Dummy":
                DummyHandler dummy_Handler = user_Handler.secondary_Data.collided_Entity.GetComponent<DummyHandler>();
                dummy_Handler.StopAllCoroutines();
                dummy_Handler.StartCoroutine(dummy_Handler.Shake(0f));
                break;

        }

        item_Data.StartCoroutine(item_Data.Attack_Rate(user_Handler, fire_Rate));
    }

    public override void Aim(UserHandler user_Handler, bool is_ADSing)
    {
        
    }

    public override void Action(UserHandler user_Handler)
    {
        
    }
}

// Ranged Item System:
[CreateAssetMenu(fileName = "New Ranged", menuName = "Items/Ranged")]
public class Ranged : Item
{
    // Ranged GUI Data:
    public Sprite Ammo;

    [Header("Ranged Attributes")]
    public bool is_Hit_Scan;
    public bool is_Spread;
    public bool is_Auto;

    [Header("Projectile Attributes")]
    public GameObject projectile_Prefab;

    public int max_Ammo;

    [Range(1, 100)] public float bullet_Speed;
    [Range(0.025f, 5)] public float reload_Speed;
    [Range(0.025f, 5)] public float fire_Rate;

    [Header("Action Attributes")]
    [Range(1, 100)] public float ADS_FOV;
    [Range(1, 100)] public float ADS_Speed;

    public override void Attack(UserHandler user_Handler)
    {
        user_Handler.Ammo--;
        if (is_Hit_Scan)
        {
            RaycastHit target;
            if (Physics.Raycast(user_Handler.fire_Point.transform.position, user_Handler.fire_Point.transform.forward, out target, Mathf.Infinity, item_Data.entity_Mask))
            {
                GameObject selected_Target = target.transform.gameObject;
                if (selected_Target.CompareTag("User"))
                {
                    UserHandler target_Handler = selected_Target.GetComponent<UserHandler>();
                    target_Handler.Damage(Damage);
                }
                else if (selected_Target.CompareTag("Dummy"))
                {
                    DummyHandler target_Handler = selected_Target.GetComponent<DummyHandler>();
                    target_Handler.StopAllCoroutines();
                    target_Handler.StartCoroutine(target_Handler.Shake(0));
                }
            }
        }
        GameObject new_Projectile = Get_Projectile(user_Handler);

        new_Projectile.transform.SetPositionAndRotation(user_Handler.fire_Point.transform.position, user_Handler.fire_Point.transform.rotation);
        new_Projectile.GetComponent<Rigidbody>().AddForce(user_Handler.fire_Point.transform.forward * bullet_Speed, ForceMode.Impulse);

        user_Handler.muzzle_Flash.Play();

        if (!is_Auto)
            item_Data.StartCoroutine(item_Data.Attack_Rate(user_Handler, fire_Rate));
    }

    public GameObject Get_Projectile(UserHandler user_Handler)
    {
        foreach (Transform projectile in user_Handler.Projectiles.transform)
        {
            if (!projectile.gameObject.activeInHierarchy && projectile.gameObject.name == projectile_Prefab.name)
            {
                projectile.gameObject.SetActive(true);
                projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
                return projectile.gameObject;
            }
        }
        GameObject new_Projectile = Instantiate(projectile_Prefab, user_Handler.Projectiles.transform);
        new_Projectile.name = projectile_Prefab.name;
        return new_Projectile;
    }

    public void Reload(UserHandler user_Handler)
    {
        item_Data.StopCoroutine("Fire_Rate");
        item_Data.StartCoroutine(item_Data.Reload_Cooldown(user_Handler, max_Ammo, reload_Speed));
    }

    public override void Aim(UserHandler user_Handler, bool is_ADSing)
    {
        float direction = is_ADSing ? -1 : 1;
        user_Handler.item_Holder.transform.position += direction * user_Handler.item_Holder.transform.right * 0.4f;
        user_Handler.item_Holder.transform.position -= direction * user_Handler.item_Holder.transform.up * 0.06f;

        user_Handler.obj_Data.walk_Speed *= is_ADSing ? item_Data.min_Speed_Mod : item_Data.max_Speed_Mod;
        user_Handler.camera_Handler.ui_Camera.fieldOfView = Mathf.Lerp(user_Handler.camera_Handler.ui_Camera.fieldOfView, is_ADSing ? ADS_FOV : user_Handler.camera_Handler.origin_FOV, ADS_Speed * Time.deltaTime);
    }

    public void Peek(UserHandler user_Handler, float angle, int side, bool enabled)
    {
        user_Handler.Neck.transform.Rotate(Vector3.forward, (enabled ? -angle : angle), Space.Self);
        if (!enabled)
            user_Handler.Neck.transform.localRotation = Quaternion.Euler(user_Handler.Neck.transform.localEulerAngles.x, 0, 0);
    }

    public override void Action(UserHandler user_Handler)
    {
        
    }
}

[CreateAssetMenu(fileName = "New Ordinance", menuName = "Items/Ordinance")]
public class Ordinance : Item
{
    [Header("Ordinace Attributes")]
    public bool is_Auto;
    public bool is_Impact;
    public bool is_cooking;

    [Header("Projectile Attributes")]
    public GameObject projectile_Prefab;
    public GameObject held_Grenade;

    public int max_Ammo;
    public int ammo;
    public float fuse_Time;

    [Range(1, 100)] public float bullet_Speed;
    [Range(0.025f, 5)] public float reload_Speed;
    [Range(0.025f, 5)] public float fire_Rate;
    public override void Attack(UserHandler user_Handler)
    {
        if (!is_cooking)
        {
            GameObject gre = Instantiate(projectile_Prefab, user_Handler.fire_Point.transform.position, user_Handler.camera_Handler.user_Camera.transform.rotation, user_Handler.camera_Handler.user_Camera.transform);
            held_Grenade = gre;
            user_Handler.can_Attack = false;
            ammo--;
        }
    }

    public override void Aim(UserHandler user_Handler, bool is_ADSing)
    {
        
    }

    public override void Action(UserHandler user_Handler)
    {
        
    }
}

public abstract class Ability : ScriptableObject
{
    [Header("Ability Data")]
    public static new_Item_Data item_Data;

    public float Cooldown;

    public abstract void Use(UserHandler user_Handler);
}

// Utility Abilities:
public abstract class Utility : Ability
{
    public void Engineer()
    {

    }
    public abstract override void Use(UserHandler user_Handler);
}
[CreateAssetMenu(fileName = "New Scan", menuName = "Abilities/Utility/Scan")]
public class Scan : Utility
{
    public override void Use(UserHandler user_Handler)
    {

    }
}
[CreateAssetMenu(fileName = "New Cloak", menuName = "Abilities/Utility/Cloak")]
public class Cloak : Utility
{
    public override void Use(UserHandler user_Handler)
    {

    }
}

// Mobility Abilities:
public abstract class Mobility : Ability
{
    public void Scout()
    {

    }
    public abstract override void Use(UserHandler user_Handler);
}
[CreateAssetMenu(fileName = "New Double Jump", menuName = "Abilities/Mobility/Double Jump")]
public class Double_Jump: Mobility
{
    public override void Use(UserHandler user_Handler)
    {

    }
}
[CreateAssetMenu(fileName = "New Teleportation", menuName = "Abilities/Mobility/Teleportation")]
public class Teleportation : Mobility
{
    public override void Use(UserHandler user_Handler)
    {

    }
}
[CreateAssetMenu(fileName = "New Phase", menuName = "Abilities/Mobility/Phase")]
public class Phase : Mobility
{
    public override void Use(UserHandler user_Handler)
    {

    }
}
[CreateAssetMenu(fileName = "New Speed Boost", menuName = "Abilities/Mobility/Speed Boost")]
public class Speed_Boost: Mobility
{
    public override void Use(UserHandler user_Handler)
    {

    }
}

// Offensive Abilities:
public abstract class Offensive : Ability
{
    public void Assault()
    {

    }
    public abstract override void Use(UserHandler user_Handler);
}
[CreateAssetMenu(fileName = "New Pinpoint Accuracy", menuName = "Abilities/Offensive/Pinpoint Accuracy")]
public class Pinpoint_Accuracy: Offensive
{
    public override void Use(UserHandler user_Handler)
    {
        
    }
}
[CreateAssetMenu(fileName = "New Self Destruct", menuName = "Abilities/Offensive/Self Destruct")]
public class Self_Destruct: Offensive
{
    public override void Use(UserHandler user_Handler)
    {

    }
}

// Defensive Abilities:
public abstract class Defensive : Ability
{
    public void Defend()
    {

    }
    public abstract override void Use(UserHandler user_Handler);
}
[CreateAssetMenu(fileName = "New Heal", menuName = "Abilities/Defensive/Heal")]
public class Heal : Defensive
{
    [Header("Healing Attributes")]
    public float heal_Value;

    public override void Use(UserHandler user_Handler)
    {
        user_Handler.Health = Mathf.Max(user_Handler.Health + heal_Value, user_Handler.max_Health);
    }
}
[CreateAssetMenu(fileName = "New Deflect", menuName = "Abilities/Defensive/Deflect")]
public class Deflect : Defensive
{
    public override void Use(UserHandler user_Handler)
    {
        
    }
}