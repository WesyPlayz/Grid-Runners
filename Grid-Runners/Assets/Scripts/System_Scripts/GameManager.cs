using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class GameManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    [Header("Round Info")]
    private int round;
    public int round_Time;
    public int intermission_Time;
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
    public UserHandler user_Handler_1;
    public UserHandler user_Handler_2;

    [SerializeField] public List<GameObject> Players = new List<GameObject> { };

    public bool round_IP;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        //StartCoroutine(Roundtime(round_Time, game_State.Round));
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    string TimeToClock(int balls = 0)
    {
        return Mathf.Floor(balls/60)+":"+Mathf.Floor((balls-Mathf.Floor(balls/60)*60)/10)+(balls-((Mathf.Floor(balls/60)*60)+Mathf.Floor((balls-Mathf.Floor(balls/60)*60)/10)*10));
    }

    public void Player_Init()
    {
        print("Hello World");
    }
    public void Player_End()
    {
        print("Goodbye World");
    }

    IEnumerator Roundtime(int time, game_State state)
    {
        switch (state)
        {
            case game_State.Round:
                user_Handler_1.Respawn();
                user_Handler_2.Respawn();
                //user_Handler_1.current_Mode = user_Handler_1.SwapMode(UserHandler.Mode.Play);
                user_Handler_2.current_Mode = user_Handler_2.SwapMode(UserHandler.Mode.Play);
                while (round_Time > 0)
                {
                    yield return new WaitForSeconds(1);
                    round_Time--;
                    print(TimeToClock(round_Time));
                }
                if (round_Time == 0) // Start Intermission
                    StartCoroutine(Roundtime(intermission_Time, game_State.Intermission));
                break;
            case game_State.Intermission:
                user_Handler_1.Respawn();
                user_Handler_2.Respawn();
                //user_Handler_1.current_Mode = user_Handler_1.SwapMode(UserHandler.Mode.Build);
                user_Handler_2.current_Mode = user_Handler_2.SwapMode(UserHandler.Mode.Menu);
                while (intermission_Time > 0)
                {
                    yield return new WaitForSeconds(1);
                    intermission_Time--;
                    print(TimeToClock(intermission_Time));
                }
                if (intermission_Time == 0)
                    StartCoroutine(Roundtime(round_Time, game_State.Round));
                break;
        }
    }

    public void quit()
    {
        Application.Quit();
    }
}