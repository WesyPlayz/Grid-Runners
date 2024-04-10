using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{

    public GameObject main_Menu;
    public GameObject levels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void level_Select(bool going)
    {
        main_Menu.SetActive(!going);
        levels.SetActive(going);
    }

    public void startGame(int map)
    {
        SceneManager.LoadScene(map);
    }
}
