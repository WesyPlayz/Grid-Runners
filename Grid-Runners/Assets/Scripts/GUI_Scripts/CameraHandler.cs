using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    // user Variables:
    private GameObject player_Obj;

    private UserHandler user_Handler;
    private new_Item_Data item_Data;

    [Header("user Variables")]
    [Range(1, 2)]
    public int Player;

    public GameObject Body;
    public GameObject Head;
    public GameObject Neck;

    // User Rotation Variables:
    private float rotationX_Spec;
    private float rotationY_Spec;

    private float distance;

    [Header("Rotation Variables")]
    [Range(50, 250)]
    public float Sensitivity;

    [Range(-360, 360)]
    public float down_Limit;
    [Range(-360, 360)]
    public float up_Limit;
    [Range(0, 360)]
    public float peak_Angle;

    // Variable Initialization System:
    void Start()
    {
        // Player Binding System:
        player_Obj = GameObject.Find((Player == 1 ? "Player_1" : "Player_2"));

        user_Handler = player_Obj.GetComponent<UserHandler>();
        item_Data = user_Handler.item_Data;

        // Head & Neck DIstance Calculation:
        distance = Vector3.Distance(Head.transform.position, Neck.transform.position);

        user_Handler.playerInputActions.Player.Scope.performed += ADS; // ADS Active
        user_Handler.playerInputActions.Player.Scope.canceled += ADS; // ADS Inactive
    }

    // Look Direction System:
    void Update()
    {
        if (!Input.GetKey(KeyCode.Tab)) // Checks to ensure mode change is not in progress (will be changed)
        {
            // Rotation Value Calculation:
            float lookHorizontal = Sensitivity * user_Handler.playerInputActions.Player.MouseX.ReadValue<float>() * Time.deltaTime;
            float lookVertical = -Sensitivity * user_Handler.playerInputActions.Player.MouseY.ReadValue<float>() * Time.deltaTime;

            // Rotation System:
            if ((lookHorizontal != 0 || lookVertical != 0))
            {
                switch(user_Handler.Mode)
                {
                    // Play Mode:
                    case 0:
                        Body.transform.Rotate(0, lookHorizontal, 0);
                        Head.transform.Rotate(lookVertical, 0, 0, Space.Self);
                        Vector3 localRotation = Head.transform.localEulerAngles;
                        if (localRotation.x > 180)
                            localRotation.x -= 360;
                        localRotation.x = Mathf.Clamp(localRotation.x, up_Limit, down_Limit);
                        Head.transform.localEulerAngles = localRotation;
                        if (!Input.GetKey(KeyCode.V))
                            Head.transform.position = Neck.transform.position + Head.transform.rotation * Vector3.forward * distance;
                        break;

                    // Build Mode:
                    case 1:
                        rotationX_Spec += lookVertical;
                        rotationY_Spec += lookHorizontal;
                        user_Handler.user_Spectate.transform.rotation = Quaternion.Euler(rotationX_Spec, rotationY_Spec, 0);
                        break;

                    // Menu Mode: (nothing for now unless new mechanics are added)
                    /* case 2:
                        break; */
                }
            }

            // Play Mode View Mechanics:
            if (user_Handler.Mode == 0)
            {
                if (Input.GetKeyDown(KeyCode.V))
                    Peak(true);
                else if (Input.GetKeyUp(KeyCode.V))
                    Peak(false);
            }
        }
    }

    // View Mechanics:
    public void ADS(InputAction.CallbackContext phase)
    {
        if (user_Handler.can_Use_Action && !user_Handler.is_Using_Action && phase.performed) // ADS Active:
        {
            user_Handler.can_Use_Action = false;
            Item current_Item = item_Data.Items[(user_Handler.selected_Weapon == 0 ? user_Handler.primary_Weapon : user_Handler.secondary_Weapon)];

            if (current_Item is Ranged ranged_Item) // Ranged ADS Active:
                ranged_Item.Aim(user_Handler, user_Handler.is_Using_Action = true);
            else if (current_Item is Melee melee_Item) // Melee ADS Active:
                melee_Item.Aim(user_Handler, user_Handler.is_Using_Action = true);
            else if (current_Item is Ordinance ordinance_Item) // Ordinance ADS Active:
                ordinance_Item.Aim(user_Handler, user_Handler.is_Using_Action = true);
        }
        else if (!user_Handler.can_Use_Action && user_Handler.is_Using_Action && phase.canceled) // ADS Inactive:
        {
            Item current_Item = item_Data.Items[(user_Handler.selected_Weapon == 0 ? user_Handler.primary_Weapon : user_Handler.secondary_Weapon)];

            if (current_Item is Ranged ranged_Item) // Ranged ADS Inactive:
                ranged_Item.Aim(user_Handler, user_Handler.is_Using_Action = false);
            else if (current_Item is Melee melee_Item) // Melee ADS Inactive:
                melee_Item.Aim(user_Handler, user_Handler.is_Using_Action = false);
            else if (current_Item is Ordinance ordinance_Item) // Ordinance ADS Inactive:
                ordinance_Item.Aim(user_Handler, user_Handler.is_Using_Action = false);

            user_Handler.can_Use_Action = true;
        }
    }

    public void Peak(bool enabled)
    {
        Quaternion rotation = Quaternion.AngleAxis((enabled ? peak_Angle : -peak_Angle), Head.transform.up);
        Head.transform.rotation = rotation * Head.transform.rotation;
        Head.transform.position = Neck.transform.position + Head.transform.rotation * Vector3.forward * distance + (enabled ? Body.transform.right * 0.1f : -Body.transform.right * 0.1f);
    }
}