using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    private GameManager gameManager;
    public CursorHandler cursor_Handler1;
    public CursorHandler cursor_Handler2;
    public bool on_Main_Menu = false;
    public GameObject main_Menu;
    public GameObject levels;

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

    [Header("Sounds")]
    public AudioClip Click1;
    public AudioClip Click2;
    public AudioClip Flip1;
    public AudioClip Flip2;
    public AudioClip Hover1;
    public AudioClip Hover2;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        if (on_Main_Menu)
        {
            cursor_Handler1.in_Menu = true;
            cursor_Handler2.in_Menu = true;
        }

            //EventSystem.current.SetSelectedGameObject(start);
        }

    public void level_Select(bool going)
    {
        main_Menu.SetActive(!going);
        levels.SetActive(going);

        if (going)
        {
            cursor_Handler1.in_Menu = true;
            cursor_Handler2.in_Menu = true;
            //EventSystem.current.SetSelectedGameObject(null);
            //EventSystem.current.SetSelectedGameObject(Back);
        }
        if (!going)
        {
            cursor_Handler1.in_Menu = false;
            cursor_Handler2.in_Menu = false;
            //EventSystem.current.SetSelectedGameObject(null);
            //EventSystem.current.SetSelectedGameObject(start);
        }

        
    }

    public void startGame(int map)
    {
        SceneManager.LoadScene(map);
    }

    public void Pause(bool pausing)
    {
        
        cursor_Handler1.in_Menu = pausing;
        cursor_Handler2.in_Menu = pausing;

        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(P1_Resume);
        //EventSystem.current.SetSelectedGameObject(P2_Resume);
    }

    public void Open_shop(int player)
    {
        if (player == 1)
        {
            cursor_Handler1.in_Menu = true;
            P1_Shop_Menu.SetActive(true);
            //EventSystem.current.SetSelectedGameObject(null);
            //EventSystem.current.SetSelectedGameObject(hyper_Blaster);
        }
        else
        {
            cursor_Handler2.in_Menu = true;
            P2_Shop_Menu.SetActive(true);
            //EventSystem.current.SetSelectedGameObject(null);
            //EventSystem.current.SetSelectedGameObject(hyper_Blaster2);
        }

        
    }

    public void Close_shop()
    {
        cursor_Handler1.in_Menu = false;
        cursor_Handler2.in_Menu = false;
        P1_Shop_Menu.SetActive(false);
        P2_Shop_Menu.SetActive(false);
    }
}