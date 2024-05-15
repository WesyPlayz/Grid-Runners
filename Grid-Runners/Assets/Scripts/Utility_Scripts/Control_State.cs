using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Control_State : MonoBehaviour
{
    public Player current_Player = Player.Player_1;
    public int
        binded_Player_1,
        binded_Player_2;
    public enum Player
    {
        Player_1,
        Player_2
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Control_Binding(int control_Setup)
    {
        if (current_Player == Player.Player_1)
        {
            binded_Player_1 = control_Setup;
            current_Player = Player.Player_2;
        }
        else
        {
            binded_Player_2 = control_Setup;
            Start_Game();
        }
    }

    public void Start_Game()
    {
        SceneManager.LoadScene(1);
    }
}