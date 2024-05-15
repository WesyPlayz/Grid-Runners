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
    private Control_State control_State;
    public enum game_State
    {
        Round,
        Intermission
    }

    [Header("Point info")]
    public int P1_Points;
    public int P2_Points;
    public int P1_kills;
    public int P2_kills;
    public int P1_Wins;
    public int P2_Wins;
    public int Winner;
    public int player_Point;

    [Header("player info")]
    public GameObject P1;
    public GameObject P2;
    public UserHandler user_Handler_1;
    public UserHandler user_Handler_2;
    private UIHandler UI_Handler;

    [Header("user info")]
    public GameObject User;
    public GameObject user_Spectate;

    //public GameObject Gm_OBJ;

    [HideInInspector] public GameObject Neck;

    // User Physics Variables:
    private CharacterController
        user_Controller,
        spectator_Controller;

    private Collider body_Hitbox;
    private Bounds body_Hitbox_Bounds;

    // User Script Variables:
    public CameraHandler camera_Handler;
    public CursorHandler cursor_Handler;
    public int player;

    public new_Item_Data item_Data;

    // User Input Variables:
    public PlayerInputActions Actions;
    public PlayerInput player_Input;
    public enum User_Input
    {
        Move,
        Attack,
        Reload
    }
    public enum User_Axis
    {
        Horizontal,
        Vertical
    }

    [Header("Data Variables")]
    public Mode current_Mode;
    public enum Mode
    {
        Play,
        Build,
        Menu,
        Idle
    }

    // Play Mode Variables:
    public GameObject Spawn;

    public float walk_Speed;
    public float sprint_Speed;

    public int Points;

    private bool changing_Mode;

    public float Health;

    [Header("Camera Variables")]
    public Camera user_Camera;

    // Movement Variables:
    private bool is_Sprinting;

    [Header("Weapon Variables")]
    public GameObject item_Holder;
    public GameObject Weapon;
    public GameObject current_ability;
    public GameObject Projectiles;
    public bool is_Blocking;

    public int selected_Weapon;
    public enum Slot
    {
        Primary,
        Secondary
    }
    public Slot current_Slot;

    public Item current_Weapon;

    public int primary_Weapon;
    public int secondary_Weapon;

    public GameObject fire_Point;

    public bool can_Attack = true;
    public bool can_Use_Action = true;
    public bool is_Using_Action;

    public float grenade_Rate;
    public float throw_force;

    public int Ammo;

    public int primary_Ammo;
    public int secondary_Ammo;

    public int grenades;
    public int max_Grenades;

    public Vector3 velocity;
    public float verticalVelocity;
    public float gravity;
    public float jumpForce;

    // Grid Variables:
    public GameObject secondary_Obj;
    public Obj_State secondary_Data;

    // Jumping Variables:

    // Collision Variables:
    public bool is_Walking;
    public bool is_Attacking;

    public bool ground = false;
    public bool wasGrounded;

    //store variables:
    private int weapon_cost;
    private GameObject equip_Button;
    public bool on_tutorial = false;

    [SerializeField] public List<GameObject> Players = new List<GameObject> { };

    public bool round_IP;
    public bool on_Tutorial = false;
    public bool on_Win = false;

    public int current_Round;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(Roundtime(round_Time, game_State.Round));
        playerInputManager = GetComponent<PlayerInputManager>();
        UI_Handler = GetComponent<UIHandler>();

        control_State = GameObject.Find("GameManager").GetComponent<Control_State>();
        if (control_State.binded_Player_1 == 1)
            P1.AddComponent<User_1>();
        else
            P1.AddComponent<User_2>();
        if (control_State.binded_Player_2 == 1)
            P2.AddComponent<User_1>();
        else
            P2.AddComponent<User_2>();

        user_Handler_1.GetComponent<User_1>()

        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
    
    public string TimeToClock(int balls = 0)
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
                    P1_Points -= 2;
                    P2_Points -= 2;
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
                    {
                        if (on_Tutorial)
                            UI_Handler.startGame(0);
                        else
                            StartCoroutine(Roundtime(intermission_Time, game_State.Intermission));
                    }
                    break;
                case game_State.Intermission: // Intermission
                    user_Handler_1.Respawn();
                    user_Handler_2.Respawn();
                    Winner = P1_kills > P2_kills? 1 : 2;
                    user_Handler_1.current_Mode = user_Handler_1.SwapMode(Winner == 1 ? UserHandler.Mode.Build : UserHandler.Mode.Menu);
                    user_Handler_2.current_Mode = user_Handler_2.SwapMode(Winner == 2 ? UserHandler.Mode.Build : UserHandler.Mode.Menu);
                    P1_Wins += Winner == 1 ? 1 : 0;
                    P1_Wins += Winner == 2 ? 1 : 0;
                    P1_kills = 0;
                    P2_kills = 0;
                    user_Handler_1.Points = P1_Points;
                    user_Handler_2.Points = P2_Points;

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
        else if (!UI_Handler.on_Main_Menu)
        {
            SceneManager.LoadScene(P1_Wins > P2_Wins ? 4 : 5);

        }
        else if (on_Win)
        {
            SceneManager.LoadScene(1);
        }
        print("Game Over");

    }

    public void quit()
    {
        Application.Quit();
    }

    public void deactivate(GameObject deactive)
    {
        deactive.SetActive(false);
    }
}