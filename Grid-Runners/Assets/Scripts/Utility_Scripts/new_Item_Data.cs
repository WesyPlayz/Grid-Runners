using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class new_Item_Data : MonoBehaviour
{
    [Header("Item Collection")]
    [SerializeField] public List<Item> Items = new List<Item> { };

    [Header("Ability Collection")]
    [SerializeField] public List<Ability> Abilities = new List<Ability> { };

    [Header("General Data")]
    public float min_Speed_Mod;
    public float max_Speed_Mod;

    [HideInInspector] public int entity_Mask;

    private void Start()
    {
        Item.item_Data = this;

        entity_Mask = 9 << LayerMask.NameToLayer("Entity");
    }

    public void Equip_Weapon(UserHandler user_Handler, int slot, int weapon)
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
[CreateAssetMenu(fileName = "New Jammer", menuName = "Abilities/Utility/Jammer")]
public class Jammer : Utility
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
public class Double_Jump : Mobility
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
public class Speed_Boost : Mobility
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
public class Pinpoint_Accuracy : Offensive
{
    public override void Use(UserHandler user_Handler)
    {

    }
}
[CreateAssetMenu(fileName = "New Self Destruct", menuName = "Abilities/Offensive/Self Destruct")]
public class Self_Destruct : Offensive
{
    public override void Use(UserHandler user_Handler)
    {

    }
}
[CreateAssetMenu(fileName = "New Berserk", menuName = "Abilities/Offensive/Berserk")]
public class Berserk : Offensive
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
[CreateAssetMenu(fileName = "New Armor Lock", menuName = "Abilities/Defensive/Armor Lock")]
public class Armor_Lock : Defensive
{
    public override void Use(UserHandler user_Handler)
    {

    }
}