using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
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
    public GameObject Pause_Menu;
    public GameObject Resume;
    public GameObject Main_Menu;

    // Start is called before the first frame update
    void Start()
    {
        //EventSystem.current.SetSelectedGameObject(null);

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
        print("worked + " + pausing);
        Pause_Menu.SetActive(pausing);
        Time.timeScale = pausing ? 1 : 0;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(Resume);
    }
}
