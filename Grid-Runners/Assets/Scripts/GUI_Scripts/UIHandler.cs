using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    private GameManager gameManager;
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

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        if (on_Main_Menu)
            EventSystem.current.SetSelectedGameObject(start);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void level_Select(bool going)
    {
        main_Menu.SetActive(!going);
        levels.SetActive(going);

        if (going)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(Back);
        }
        if (!going)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(start);
        }

        
    }

    public void startGame(int map)
    {
        SceneManager.LoadScene(map);
    }

    public void Pause(bool pausing)
    {
        if (pausing)
        {
            gameManager.P1S.playerInputActions.Player.Disable();
            gameManager.P2S.playerInputActions.Player.Disable();
        }
        else if (!pausing)
        {
            gameManager.P1S.playerInputActions.Player.Enable();
            gameManager.P2S.playerInputActions.Player.Enable();
        }
        P1_Pause_Menu.SetActive(pausing);
        P2_Pause_Menu.SetActive(pausing);
        Time.timeScale = pausing ? 0 : 1;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(P1_Resume);
        EventSystem.current.SetSelectedGameObject(P2_Resume);
    }

    public void Open_shop(int player)
    {
        if (player == 1)
        {
            P1_Shop_Menu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(hyper_Blaster);
        }
        else
        {
            P2_Shop_Menu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(hyper_Blaster2);
        }

        
    }

    public void Close_shop()
    {
        P1_Shop_Menu.SetActive(false);
        P2_Shop_Menu.SetActive(false);
    }
}
