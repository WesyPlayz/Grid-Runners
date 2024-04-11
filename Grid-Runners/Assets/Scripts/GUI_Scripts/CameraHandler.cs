using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    // user Variables:
    private GameObject player_Obj;

    private UserHandler user_Handler;

    [Header("Camera Variables")]
    public Camera user_Camera;
    public Camera ui_Camera;

    public GameObject 
        Camera_Pos0, 
        Camera_Pos1;

    [HideInInspector] public float origin_FOV;

    [Header("user Variables")]
    [Range(1, 2)]
    public int Player;

    [Header("Rotation Variables")]
    [Range(1, 5)]
    public float Sensitivity;

    [Range(0, 360)]
    public float look_Limit;
    [Range(-360, 360)]
    public float peek_Angle;
    [Range(0, 360)]
    public float Peek_Limit;

    public bool is_Peeking;
    private int Side;

    // Variable Initialization System:
    void Start()
    {
        // Player Binding System:
        player_Obj = GameObject.Find((Player == 1 ? "Player_1" : "Player_2")); // Finds which player this script is assigned to

        user_Handler = player_Obj.GetComponent<UserHandler>();

        // Camera Variable Setup:
        origin_FOV = ui_Camera.fieldOfView; // Assigns base FOV

        // Input Initalization:
        user_Handler.playerInputActions.Player.Scope.performed += ADS; // ADS Active
        user_Handler.playerInputActions.Player.Scope.canceled += ADS; // ADS Inactive

        user_Handler.playerInputActions.Player.Peek_Right.performed += phase => Peek(phase, 0); // Peek Right Active
        user_Handler.playerInputActions.Player.Peek_Left.performed += phase => Peek(phase, 1); // Peek Left Active
        user_Handler.playerInputActions.Player.Peek_Right.canceled += phase => Peek(phase, 0); // Peek Right Inactive
        user_Handler.playerInputActions.Player.Peek_Left.canceled += phase => Peek(phase, 1); // Peek Left Inactive
    }

    // View Mechanics:
    public void Update() // Look System:
    {
        if (user_Handler.Mode != 2)
        {
            float lookHorizontal = Sensitivity * user_Handler.playerInputActions.Player.MouseX.ReadValue<float>(); // Y-Axis Rotational Value
            float lookVertical = -Sensitivity * user_Handler.playerInputActions.Player.MouseY.ReadValue<float>(); // X-Axis Rotational Value

            if (lookHorizontal != 0 || lookVertical != 0) // Movement Check:
            {
                (user_Handler.Mode == 0 ? user_Handler.User : user_Handler.user_Spectate).transform.Rotate(Vector3.up * lookHorizontal, Space.World); // Y-Axis Rotation
                (user_Handler.Mode == 0 ? user_Handler.Neck : user_Handler.user_Spectate).transform.Rotate(Vector3.right * lookVertical); // X-Axis Rotation

                if (user_Handler.Mode == 0) // X-Axis Limit System:
                {
                    Vector3 local_Rot = user_Handler.Neck.transform.localEulerAngles; // Gets local angle values
                    local_Rot.x = (local_Rot.x > 180) ? local_Rot.x - 360 : local_Rot.x; // creates a loop for the angle value
                    local_Rot.x = Mathf.Clamp(local_Rot.x, (!is_Peeking ? -look_Limit : -Peek_Limit), (!is_Peeking ? look_Limit : Peek_Limit)); // Limits X-Axis
                    if (is_Peeking)
                    {
                        local_Rot.z = (local_Rot.z > 180) ? local_Rot.z - 360 : local_Rot.z; // creates a loop for the angle value
                        local_Rot.z = Mathf.Clamp(local_Rot.z, (Side == 0 ? -Peek_Limit : -peek_Angle), (Side == 0 ? -peek_Angle : Peek_Limit)); // Limits Z-Axis
                        local_Rot.y = 0; // Locks Y-Axis
                    }
                    else
                        local_Rot.z = 0; // Locks Z-Axis
                    user_Handler.Neck.transform.localEulerAngles = local_Rot; // Sets Modified Rotation
                }
            }
        }
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
    public void Peek(InputAction.CallbackContext phase, int side) // Peek System:
    {
        peek_Angle = (Side = side) == 0 ? peek_Angle : -peek_Angle; // Side Check

        if (user_Handler.current_Weapon is Ranged ranged_Item) // Range Check:
            ranged_Item.Peek(user_Handler, peek_Angle, side, (is_Peeking = phase.performed ? true : false)); // Peek Active or Inactive
    }
}