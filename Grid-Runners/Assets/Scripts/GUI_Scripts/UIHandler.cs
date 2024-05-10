using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class UIHandler : MonoBehaviour
{
    private GameManager gameManager;
    public CursorHandler cursor_Handler1;
    public CursorHandler cursor_Handler2;
    public bool on_Main_Menu = false;
    public GameObject main_Menu;
    public GameObject levels;

    public new_Item_Data item_Data;

    [Header("Main menu")]
    public GameObject start;

    [Header("Level select")]
    public GameObject Back;
    public GameObject Tutorial;
    public GameObject Medieval;
    public GameObject Waterpark;
    public GameObject Futuristic;

    [Header("Pause")]
    public GameObject P1_Pause_Menu;
    public GameObject P2_Pause_Menu;
    public GameObject P1_Resume;
    public GameObject P2_Resume;
    public GameObject Main_Menu;

    [Header("Shop")]
    public GameObject P1_Shop_Menu;
    public GameObject P2_Shop_Menu;
    public GameObject hyper_Blaster;
    public GameObject hyper_Blaster2;
    public GameObject P1_Weapons;
    public GameObject P2_Weapons;
    public GameObject P1_Abilities;
    public GameObject P2_Abilities;

    [Header("Sounds")]
    public AudioClip Click1;
    public AudioClip Click2;
    public AudioClip Flip1;
    public AudioClip Flip2;
    public AudioClip Hover1;
    public AudioClip Hover2;

    void Start()
    {
        gameManager = GetComponent<GameManager>();

        // Weapon Interfaces:
        for (int i = 0; i < item_Data.Items.Count; i++)
        {
            Item weapon = item_Data.Items[i];
            shop_Interface weapon_Interface = item_Data.Interfaces[i];

            weapon_Interface.Name.text = weapon.weapon_Prefab.name;
            weapon_Interface.Icon.sprite = weapon.Icon;

            weapon_Interface.DMG.text = "DMG | " + weapon.Damage.ToString();

            if (weapon is Ranged ranged_Weapon)
            {
                weapon_Interface.FR.text = "FR | " + (Mathf.Round((1 / ranged_Weapon.fire_Rate) * 10) / 10).ToString();
                weapon_Interface.Handling.text = "Handling | " + (Mathf.Round((ranged_Weapon.reload_Speed + (ranged_Weapon.ADS_Speed / 10) + ranged_Weapon.speed_Mod / 3) * 10) / 10).ToString();
            }
            weapon_Interface.Cost.text = weapon.Cost.ToString() + " (Points)";

            weapon_Interface.purchase_Button.onClick.AddListener(() => item_Data.Add_Weapon((weapon_Interface.bound_Player == shop_Interface.Player.Player_1 ? gameManager.user_Handler_1 : gameManager.user_Handler_2), i));
            weapon_Interface.equip_Button.onClick.AddListener(() => item_Data.Add_Weapon((weapon_Interface.bound_Player == shop_Interface.Player.Player_1 ? gameManager.user_Handler_1 : gameManager.user_Handler_2), i));
        }
    }


    public void startGame(int map)
    {
        SceneManager.LoadScene(map);
    }

    public void Toggle_Abilities(int player_Going)
    {
        if (player_Going == 1)
        {
            P1_Weapons.SetActive(false);
            P1_Abilities.SetActive(true);
        }
        else if (player_Going == 2)
        {
            P1_Weapons.SetActive(true);
            P1_Abilities.SetActive(false);
        }

        else if (player_Going == 3)
        {
            P2_Weapons.SetActive(false);
            P2_Abilities.SetActive(true);
        }
        else if (player_Going == 4)
        {
            P2_Weapons.SetActive(true);
            P2_Abilities.SetActive(false);
        }
    }

    void Load_Menu_State(string state)
    {
        
    }
    void Quit_Game()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}