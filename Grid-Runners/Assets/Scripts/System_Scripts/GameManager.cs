using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private UIHandler UI;
    [Header("Round Info")]
    private int round;
    public int round_Time;
    public int intermission_Time;
    public float map_size;
    public int Players_Ready;
    public enum game_State
    {
        Round,
        Intermission
    }

    [Header("Point info")]
    public int P1_Points;
    public int P2_Points;
    public int winner;
    public int player_Point;

    [Header("player info")]
    public GameObject P1;
    public GameObject P2;
    public UserHandler P1S;
    public UserHandler P2S;
    public int players;
    public int players_Alive;

    public bool round_IP;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        UI = GetComponent<UIHandler>();
        if (!UI.on_Main_Menu)
        {
            P1S = P1.GetComponent<UserHandler>();
            P2S = P2.GetComponent<UserHandler>();
        }
        StartCoroutine(Roundtime(round_Time = 60, game_State.Round));
    }

    public void EndGame()
    {
        winner = P1_Points > P2_Points ? 1 : 2;
    }

    public void EndRound()
    {
        if (round == 5)
            EndGame();
        round++;
    }

    public void StartRound()
    {
        P1S.points = 0;
        P2S.points = 0;
    }

    public void Ready()
    {
        Players_Ready++;
        if (Players_Ready == 2)
            StartRound();
    }

    IEnumerator Roundtime(int time, game_State state)
    {
        switch (state)
        {
            case game_State.Round:
                while (round_Time > 0)
                {
                    yield return new WaitForSeconds(1);
                    round_Time--;
                    print(round_Time);
                }
                if (round_Time == 0) // Start Intermission
                    StartCoroutine(Roundtime(intermission_Time = 30, game_State.Intermission));
                break;
            case game_State.Intermission:
                while (intermission_Time > 0)
                {
                    yield return new WaitForSeconds(1);
                    intermission_Time--;
                    print(intermission_Time);
                }
                print("Pause Game");
                break;
        }
    }

    //main menu options



    public void quit()
    {
        Application.Quit();
    }
}
