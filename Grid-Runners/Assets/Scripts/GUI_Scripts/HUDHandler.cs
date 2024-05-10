using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class HUDHandler : MonoBehaviour
{
    public GameObject Player;
    private GameObject User;
    private UserHandler user_Handler;

    public Image Health_Bar;

    public TextMeshProUGUI current_Weapon_Name;

    public Image current_Weapon_Icon;
    public Image holstered_Weapon_Icon;

    public Sprite primary_Weapon_Icon;
    public Sprite secondary_Weapon_Icon;

    public Image ammo_Counter;

    [SerializeField] public List<GameObject> HUD_Menus = new List<GameObject>();

    void Start()
    {
        user_Handler = Player.GetComponent<UserHandler>();

        User = user_Handler.User;
    }
    void Update()
    {
        Health_Bar.fillAmount = user_Handler.Health / user_Handler.max_Health;
        if (user_Handler.current_Weapon is Ranged ranged_Weapon)
            ammo_Counter.fillAmount = (float)user_Handler.Ammo / ranged_Weapon.max_Ammo;
    }
}