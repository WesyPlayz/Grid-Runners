using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField] private List<Button> Buttons = new List<Button>();

    void Start()
    {
        gameManager = GetComponent<GameManager>();

        //EventSystem.current.SetSelectedGameObject(start);

        for (int i = 0; i < Buttons.Count; i++)
        {
            Button button = Buttons[i];
            switch (button.name)
            {
                case "Purchase_Hyper_Blaster_1":
                    button.onClick.AddListener(() => item_Data.Add_Weapon(gameManager.user_Handler_1, 0));
                    break;

                case "Purchase_Hyper_Blaster_2":
                    button.onClick.AddListener(() => item_Data.Add_Weapon(gameManager.user_Handler_2, 0));
                    break;
            }
        }
        /*
        Buttons[0].onClick.AddListener(() => Play_Game()); // Play Button - Main Menu
        Buttons[1].onClick.AddListener(() => Load_Menu_State("Open")); // Load Button - Main Menu
        Buttons[2].onClick.AddListener(() => Load_Menu_State("Close")); // Exit Button - Load Menu

        Buttons[3].onClick.AddListener(() => Quit_Game()); // Quit Button - Main Menu
        */
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