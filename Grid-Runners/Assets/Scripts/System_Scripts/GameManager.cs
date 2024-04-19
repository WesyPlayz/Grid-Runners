using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private UIHandler ui_Handler;
    [Header("Round Info")]
    private int round;
    public float RoundTimer;
    public float map_size;
    public int Players_Ready;

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


    // Start is called before the first frame update
    void Start()
    {
        RoundTimer = 90f;
        Application.targetFrameRate = 60;

        ui_Handler = GetComponent<UIHandler>();
        if (!ui_Handler.on_Main_Menu)
        {
            P1S = P1.GetComponent<UserHandler>();
            P2S = P2.GetComponent<UserHandler>();
            StartRound();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

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
        print(round);
        ui_Handler.Open_shop(winner);
        Time.timeScale = 0;
    }

    public void StartRound()
    {
        StartCoroutine(Roundtime());
        P1S.points = 0;
        P2S.points = 0;
        ui_Handler.Close_shop();
        Time.timeScale = 1;
    }

    public void Ready()
    {
        Players_Ready++;
        if (Players_Ready == 2)
            StartRound();
    }

    IEnumerator Roundtime()
    {
        yield return new WaitForSeconds(RoundTimer);

        P1_Points++;
        //player_Point = P1S.points > P2S.points ? 1 : 2;
        EndRound();
    }



    public void quit()
    {
        Application.Quit();
    }
}
