using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [Header("Round Info")]
    private int round;
    public float RoundTimer;
    public float map_size;

    [Header("Point info")]
    public int P1_Points;
    public int P2_Points;
    public int winner;
    public int player_Point;

    [Header("player info")]
    public GameObject P1;
    public GameObject P2;
    private UserHandler P1S;
    private UserHandler P2S;
    public int players;
    public int players_Alive;


    // Start is called before the first frame update
    void Start()
    {
        RoundTimer = 45f * players * (map_size);
        Application.targetFrameRate = 60;
        P1S = P1.GetComponent<UserHandler>();
        P2S = P2.GetComponent<UserHandler>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EndRound()
    {
        if (round == 5)
            EndGame();
        round++;
    }

    public void EndGame()
    {
        winner = P1_Points > P2_Points ? 1 : 2;
        
    }

    //main menu options
    
    IEnumerator Roundtime()
    {
        yield return new WaitForSeconds(RoundTimer);
        
        player_Point = P1S.points > P2S.points ? 1 : 2;
        EndRound();
    }

    public void quit()
    {
        Application.Quit();
    }
}
