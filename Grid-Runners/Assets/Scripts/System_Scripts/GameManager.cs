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
    public int max_Rounds;
    public int round_Time;
    public int current_Round_Time;
    public int intermission_Time;
    public int current_Intermission_Time;
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

    public int current_Round;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(Roundtime(round_Time, game_State.Round));
        playerInputManager = GetComponent<PlayerInputManager>();

        
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
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
        if (current_Round != max_Rounds)
        {
            switch (state)
            {
                case game_State.Round: // Round
                    current_Round++;
                    user_Handler_1.Respawn();
                    user_Handler_2.Respawn();
                    user_Handler_1.current_Mode = user_Handler_1.SwapMode(UserHandler.Mode.Play);
                    user_Handler_2.current_Mode = user_Handler_2.SwapMode(UserHandler.Mode.Play);

                    current_Round_Time = round_Time;
                    while (current_Round_Time > 0)
                    {
                        yield return new WaitForSeconds(1);
                        current_Round_Time--;
                        print(TimeToClock(current_Round_Time));
                    }
                    if (current_Round_Time == 0) // Start Intermission
                        StartCoroutine(Roundtime(intermission_Time, game_State.Intermission));
                    break;
                case game_State.Intermission: // Intermission
                    user_Handler_1.Respawn();
                    user_Handler_2.Respawn();
                    if (user_Handler_1.Points != user_Handler_2.Points)
                        winner = user_Handler_1.Points > user_Handler_2.Points ? 1 : 2;
                    user_Handler_1.current_Mode = user_Handler_1.SwapMode(winner == 1 ? UserHandler.Mode.Build : UserHandler.Mode.Menu);
                    user_Handler_2.current_Mode = user_Handler_2.SwapMode(winner == 2 ? UserHandler.Mode.Build : UserHandler.Mode.Menu);

                    current_Intermission_Time = intermission_Time;
                    while (current_Intermission_Time > 0)
                    {
                        yield return new WaitForSeconds(1);
                        current_Intermission_Time--;
                        print(TimeToClock(current_Intermission_Time));
                    }
                    if (current_Intermission_Time == 0) // Start Round
                        StartCoroutine(Roundtime(round_Time, game_State.Round));
                    break;
            }  
        }
        print("Game Over");
    }

    public void quit()
    {
        Application.Quit();
    }
}