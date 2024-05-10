using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class new_Item_Data : MonoBehaviour
{
    [Header("Item Collection")]
    [SerializeField] public List<Item> Items = new List<Item> { };
    public List<Economy> Purchased = new List<Economy>();

    [Header("Weapon Interfaces")]
    [SerializeField] public List<shop_Interface> Interfaces = new List<shop_Interface> { };

    [Header("Ability Collection")]
    [SerializeField] public List<Ability> Abilities = new List<Ability> { };

    [Header("General Data")]
    public float min_Speed_Mod;
    public float max_Speed_Mod;

    [HideInInspector] public int entity_Mask;

    private void Start()
    {
        Item.item_Data = this;

        for (int i = 0; i < Items.Count; i++)
        {
            Economy new_Economy = new Economy { };
            Purchased.Add(new_Economy);
        }

        entity_Mask = 9 << LayerMask.NameToLayer("Entity");
    }

    // Weapon State System:
    public void Add_Weapon(UserHandler user_Handler, int selected_Weapon)
    {
        Item weapon = Items[selected_Weapon];
        if (Purchased[selected_Weapon].Validity == false)
        {
            if (user_Handler.Points < weapon.Cost)
                return;
            else
            {
                user_Handler.Points = Mathf.Max(user_Handler.Points - weapon.Cost, 0);
                Purchased[selected_Weapon].Validity = true;
                Interfaces[selected_Weapon].purchase_Button.interactable = false;
                Interfaces[selected_Weapon].Cost.text = "Item Purchased";

                Interfaces[selected_Weapon].equip_Text.gameObject.SetActive(true);
                Interfaces[selected_Weapon].equip_Button.interactable = true;
            }
        }
        if (weapon.weapon_Class == Item.Class.Primary)
            user_Handler.primary_Weapon = selected_Weapon;
        else
            user_Handler.secondary_Weapon = selected_Weapon;
    }
    public void Equip_Weapon(UserHandler user_Handler, int slot, int weapon)
    {
        if (weapon >= 0)
        {
            if (user_Handler.Weapon != null)
                Destroy(user_Handler.Weapon);

            // Initialize Weapon:
            Item selected_Weapon = Items[weapon];
            user_Handler.current_Weapon = selected_Weapon;
            GameObject new_Weapon = Instantiate(selected_Weapon.weapon_Prefab);
            new_Weapon.transform.parent = user_Handler.item_Holder.transform;
            new_Weapon.transform.SetPositionAndRotation(user_Handler.item_Holder.transform.position, user_Handler.item_Holder.transform.rotation);

            user_Handler.Weapon = new_Weapon;

            // Initialize Weapon HUD:
            user_Handler.hud_Handler.current_Weapon_Name.text = selected_Weapon.weapon_Prefab.name;
            if (slot == 0 && selected_Weapon.Icon != user_Handler.hud_Handler.primary_Weapon_Icon)
                user_Handler.hud_Handler.primary_Weapon_Icon = selected_Weapon.Icon;
            else if (slot == 1 && selected_Weapon.Icon != user_Handler.hud_Handler.secondary_Weapon_Icon)
                user_Handler.hud_Handler.secondary_Weapon_Icon = selected_Weapon.Icon;
            else
            {
                user_Handler.hud_Handler.holstered_Weapon_Icon.sprite = user_Handler.hud_Handler.current_Weapon_Icon.sprite;
                user_Handler.hud_Handler.current_Weapon_Icon.sprite = selected_Weapon.Icon;
            }
            if (selected_Weapon is Ranged ranged_Item)
            {
                foreach (Transform child in new_Weapon.transform)
                {
                    switch (child.name)
                    {
                        case "Fire_Point":
                            user_Handler.fire_Point = child.gameObject;
                            break;
                        case "Muzzle_Flash":
                            user_Handler.muzzle_Flash = child.GetComponent<ParticleSystem>();
                            break;
                    }
                }
                user_Handler.Ammo =
                weapon == user_Handler.primary_Weapon && slot == 0 ? user_Handler.primary_Ammo :
                weapon == user_Handler.secondary_Weapon && slot == 1 ? user_Handler.secondary_Ammo :
                ranged_Item.max_Ammo;
            }
        }
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

// Item Shop Systems:
public class Economy
{
    public bool Validity = false;
}
[System.Serializable]
public class shop_Interface
{
    public Player bound_Player;
    public enum Player
    {
        Player_1,
        Player_2
    }
    public TMP_Text Name;

    public Image Upgrade;
    public Image Icon;

    public Button purchase_Button;
    public Button equip_Button;

    public TMP_Text Cost;
    public TMP_Text equip_Text;

    public TMP_Text DMG;
    public TMP_Text FR;
    public TMP_Text Handling;
}

// Item Classes:
public abstract class Item : ScriptableObject
{
    [Header("Object Data")]
    public GameObject weapon_Prefab;
    public Class weapon_Class;
    public enum Class
    {
        Primary,
        Secondary,
        Equipment
    }
    public int Cost;

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

// Mobility Abilities:
public abstract class Mobility : Ability
{
    public void Scout()
    {

    }
    public abstract override void Use(UserHandler user_Handler);
}

// Offensive Abilities:
public abstract class Offensive : Ability
{
    public void Assault()
    {

    }
    public abstract override void Use(UserHandler user_Handler);
}

// Defensive Abilities:
public abstract class Defensive : Ability
{
    public void Defend()
    {

    }
    public abstract override void Use(UserHandler user_Handler);
}