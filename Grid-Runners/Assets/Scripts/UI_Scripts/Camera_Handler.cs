// System Libraries :
using System.Collections;
using System.Collections.Generic;

// Unity Engine Libraries :
using UnityEngine;
using UnityEngine.InputSystem;

// Game Libraries :
using Utilities;

// Script Class :
public class Camera_Handler : MonoBehaviour
{
    private Game_Manager game_Manager; // [ Game_Manager Reference ]

    [Header("User Variables")] // Variables Associated With The User :
    //{
        public Game_Manager.Player player;
        private GameObject player_Obj;

        private User_Handler user_Handler;
        private Uni_User_Handler uni_User_Handler;
    //}

    [Header("Camera Variables")] // Variables Associated With The Camera :
    //{
        public Camera 
            user_Camera, 
            ui_Camera;

        public GameObject
            user_Cam_Pos,
            spec_Cam_Pos,
            menu_Cam_Pos;

        [HideInInspector] public float origin_FOV;
        [Range(.1f, 69)] public float camera_Sensitivity;
    //}

    [Header("Peek Variables")] // Variables Associated With The Camera Peek System :
    //{
        public bool is_Peeking;
        public Side current_Side;
        public enum Side
        {
            Right,
            Left
        }
        [Range(-360, 360)] public float peek_Angle;
    //}

    [Header("Limit Variables")] // Variables Associated With The Camera Limit System :
    //{
        [Range(0, 360)] public float look_Limit;
        [Range(0, 360)] public float Peek_Limit;
    //}

    // Initialization System :
    public static string ID_01 = "ID : Cam_01";
    /// <summary>
    /// [ ID : Cam_01 ]
    /// Params : (None)
    /// </summary>
    public void Start()
    {
        string parameters = string.Empty;

        // User Assignment :
        player_Obj = GameObject.Find // Finds The Player Which This Script Is Assigned To :
            ((
                player != Game_Manager.Player.Player ? // [ Checks If Player Is Not Universal ]
                (
                    player == Game_Manager.Player.Player_1 ? "Player_1" : // [ Checks If Player Is FIrst Player ]
                    "Player_2" // [ Assigns To Second Player If All Other's Are False ]
                ) :
                "Player" // [ Assigns To Universal Player If All Other's Are False ]
            ));
        parameters = "(Player_1 | Player_2 | Player)";
        if (player_Obj == null) // Existential Check For player_obj :
            Debug_System.Shutdown
                (
                    this, 
                    ID_01 + "| Null Reference : player_obj was not defined.\nParameters : " + parameters
                );
        parameters = "(Game_Manager)";
        if (game_Manager != null) // Existential Check For game_Manager :
        {
            if (game_Manager.input_State != Game_Manager.Input_Type.Singular) // Checks If Input Type Is Universal Or Not :
            {
                user_Handler = player_Obj.GetComponent<User_Handler>(); // [ Assigns The Reference Of User_Handler ]
                parameters = "(User_Handler)";
                if (user_Handler == null) // Existential Check For user_Handler :
                    Debug_System.Shutdown
                        (
                            this, 
                            ID_01 + "| Null Reference : user_Handler was not defined.\nParameters : " + parameters
                        );
            }
            else if (game_Manager.input_State != Game_Manager.Input_Type.Universal)
            {
                uni_User_Handler = player_Obj.GetComponent<Uni_User_Handler>(); // [ Assigns The Reference Of Uni_User_Handler ]
                parameters = "(Uni_User_Handler)";
                if (uni_User_Handler == null) // Existential Check For uni_User_Handler :
                    Debug_System.Shutdown
                        (
                            this, 
                            ID_01 + "| Null Reference : uni_User_Handler was not defined.\nParameters : " + parameters
                        );
            }
        }
        else
            Debug_System.Shutdown
                (
                    this, 
                    ID_01 + "| Null Reference : game_Manager was not defined.\nParameters : " + parameters
                );

        // Camera Variable Setup:
        origin_FOV = ui_Camera.fieldOfView; // Assigns base FOV
    }

    // View Mechanics:
    public static string ID_02 = "ID : Cam_02";
    /// <summary>
    /// [ ID : Cam_02 ]
    /// Params : (None)
    /// </summary>
    public void Update() // Look System:
    {
        // Initialize Camera Values :
        float camera_Horizontal = 0;
        float camera_Vertical = 0;

        // Direct Input :
        switch (game_Manager.input_State)
        {
            // Singular Player Input :
            case Game_Manager.Input_Type.Singular:
                if (user_Handler.current_Mode != User_Handler.Mode.Menu) // [ Checks If Player Is Currently In A Menu ]
                {
                    camera_Horizontal = camera_Sensitivity * user_Handler.GetInputAxis // Y-Axis Movement Value :
                        (
                            User_Handler.User_Axis.Horizontal
                        );
                    camera_Vertical = camera_Sensitivity * user_Handler.GetInputAxis // X-Axis Movement Value :
                        (
                            User_Handler.User_Axis.Vertical
                        );
                }
                break;

            // Universal Player Input :
            case Game_Manager.Input_Type.Universal:
                if (uni_User_Handler.current_Mode != Uni_User_Handler.Mode.Menu) // [ Checks If The Players Are Currently In A Menu ]
                {
                    camera_Horizontal = camera_Sensitivity * uni_User_Handler.GetInputAxis // Y-Axis Movement Value :
                        (
                            Uni_User_Handler.User_Axis.Horizontal
                        );
                    camera_Vertical = camera_Sensitivity * uni_User_Handler.GetInputAxis // X-Axis Movement Value :
                        (
                            Uni_User_Handler.User_Axis.Vertical
                        );
                }
                break;
        }

        // Cursor Movement System :
        if (game_Manager.input_State != Game_Manager.Input_Type.None && camera_Horizontal + camera_Vertical != 0) // [ Checks If Input Is Active ]
            user_Handler.Neck.transform.localEulerAngles = Turn_Camera // Run Cursor Movement :
                ( 
                    camera_Horizontal, 
                    camera_Vertical
                );

        if (user_Handler.current_Mode != UserHandler.Mode.Menu)
        {
            float lookHorizontal = Sensitivity * user_Handler.GetInputAxis(UserHandler.User_Axis.Horizontal); // Y-Axis Rotational Value
            float lookVertical = -Sensitivity * user_Handler.GetInputAxis(UserHandler.User_Axis.Vertical); // X-Axis Rotational Value

            if (lookHorizontal != 0 || lookVertical != 0) // Movement Check:
            {
                (user_Handler.current_Mode == UserHandler.Mode.Play ? user_Handler.User : user_Handler.user_Spectate).transform.Rotate(Vector3.up * lookHorizontal, Space.World); // Y-Axis Rotation
                (user_Handler.current_Mode == UserHandler.Mode.Play ? user_Handler.Neck : user_Handler.user_Spectate).transform.Rotate(Vector3.right * lookVertical); // X-Axis Rotation

                if (user_Handler.current_Mode == UserHandler.Mode.Play) // X-Axis Limit System:
                {
                    Vector3 local_Rot = user_Handler.Neck.transform.localEulerAngles; // Gets local angle values
                    local_Rot.x = (local_Rot.x > 180) ? local_Rot.x - 360 : local_Rot.x; // creates a loop for the angle value
                    local_Rot.x = Mathf.Clamp(local_Rot.x, (!is_Peeking ? -look_Limit : -Peek_Limit), (!is_Peeking ? look_Limit : Peek_Limit)); // Limits X-Axis
                    if (is_Peeking)
                    {
                        local_Rot.z = (local_Rot.z > 180) ? local_Rot.z - 360 : local_Rot.z; // creates a loop for the angle value
                        local_Rot.z = Mathf.Clamp(local_Rot.z, (current_Side == Side.Right ? -Peek_Limit : -peek_Angle), (current_Side == Side.Right ? -peek_Angle : Peek_Limit)); // Limits Z-Axis
                    }
                    else
                        local_Rot.z = 0; // Locks Z-Axis
                    local_Rot.y = 0; // Locks Y-Axis
                    user_Handler.Neck.transform.localEulerAngles = local_Rot; // Sets Modified Rotation
                }
            }
        }
    }

    public Vector3 Turn_Camera(float x, float y)
    {
        Vector3 local_Rot = Vector3.zero;

        // Direct Input :
        switch (game_Manager.input_State)
        {
            // Singular Player Input :
            case Game_Manager.Input_Type.Singular:
                (user_Handler.current_Mode == User_Handler.Mode.Play ? user_Handler.User : user_Handler.user_Spectate).transform.Rotate(Vector3.up * x, Space.World); // Y-Axis Rotation
                (user_Handler.current_Mode == User_Handler.Mode.Play ? user_Handler.Neck : user_Handler.user_Spectate).transform.Rotate(Vector3.right * y); // X-Axis Rotation
                local_Rot = user_Handler.Neck.transform.localEulerAngles; // Gets local angle values
                break;

            // Universal Player Input :
            case Game_Manager.Input_Type.Universal:
                if (uni_User_Handler.current_Mode != Uni_User_Handler.Mode.Menu) // [ Checks If The Players Are Currently In A Menu ]
                {
                    (uni_User_Handler.current_Mode == Uni_User_Handler.Mode.Play ? uni_User_Handler.User : uni_User_Handler.user_Spectate).transform.Rotate(Vector3.up * x, Space.World); // Y-Axis Rotation
                    (uni_User_Handler.current_Mode == Uni_User_Handler.Mode.Play ? uni_User_Handler.Neck : uni_User_Handler.user_Spectate).transform.Rotate(Vector3.right * y); // X-Axis Rotation
                    local_Rot = uni_User_Handler.Neck.transform.localEulerAngles; // Gets local angle values
                }
                break;
        }
        local_Rot.x = (local_Rot.x > 180) ? local_Rot.x - 360 : local_Rot.x; // creates a loop for the angle value
        local_Rot.x = Mathf.Clamp(local_Rot.x, (!is_Peeking ? -look_Limit : -Peek_Limit), (!is_Peeking ? look_Limit : Peek_Limit)); // Limits X-Axis
        if (is_Peeking)
        {
            local_Rot.z = (local_Rot.z > 180) ? local_Rot.z - 360 : local_Rot.z; // creates a loop for the angle value
            local_Rot.z = Mathf.Clamp(local_Rot.z, (current_Side == Side.Right ? -Peek_Limit : -peek_Angle), (current_Side == Side.Right ? -peek_Angle : Peek_Limit)); // Limits Z-Axis
        }
        else
            local_Rot.z = 0; // Locks Z-Axis
        local_Rot.y = 0; // Locks Y-Axis
        return local_Rot; // [ Returns The Rotation Value ]
    }

    public void ADS(InputAction.CallbackContext phase) // Aim System:
    {
        user_Handler.can_Use_Action = !phase.performed;
        user_Handler.is_Using_Action = phase.performed;

        if (user_Handler.current_Weapon is Ranged ranged_Item) // Range Check:
            ranged_Item.Aim(user_Handler, user_Handler.is_Using_Action); // Aim Active or Inactive
        else if (user_Handler.current_Weapon is Melee melee_Item) // Melee Check:
            melee_Item.Aim(user_Handler, user_Handler.is_Using_Action); // Aim Active or Inactive
        else if (user_Handler.current_Weapon is Ordinance ordinance_Item) // Ordinance Check:
            ordinance_Item.Aim(user_Handler, user_Handler.is_Using_Action); // Aim Active or Inactive
    }
    public void Peek(InputAction.CallbackContext phase, Side side) // Peek System:
    {
        peek_Angle = (current_Side = side) == Side.Right ? peek_Angle : -peek_Angle; // Side Check

        if (user_Handler.current_Weapon is Ranged ranged_Item) // Range Check:
            ranged_Item.Peek(user_Handler, peek_Angle, (is_Peeking = phase.performed ? true : false)); // Peek Active or Inactive
    }
}