using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    // user Variables:
    private GameObject player_Obj;

    private UserHandler user_Handler;

    [Header("user Variables")]
    [Range(1, 2)]
    public int Player;

    public GameObject Body;
    public GameObject Head;
    public GameObject Neck;

    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    // User Rotation Variables:
    private float rotationX_Spec;
    private float rotationY_Spec;

    private float distance;

    [Header("Rotation Variables")]
    [Range(100, 1000)]
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

        // Head & Neck DIstance Calculation:
        distance = Vector3.Distance(Head.transform.position, Neck.transform.position);

        playerInput = player_Obj.GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Look Direction System:
    void Update()
    {
        if (!Input.GetKey(KeyCode.Tab)) // Checks to ensure mode change is not in progress (will be changed)
        {
            // Rotation Value Calculation:
            float lookHorizontal = Sensitivity * playerInputActions.Player.MouseX.ReadValue<float>() * Time.deltaTime;
            float lookVertical = -Sensitivity * playerInputActions.Player.MouseY.ReadValue<float>() * Time.deltaTime;

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
    public void Peak(bool enabled)
    {
        Quaternion rotation = Quaternion.AngleAxis((enabled ? peak_Angle : -peak_Angle), Head.transform.up);
        Head.transform.rotation = rotation * Head.transform.rotation;
        Head.transform.position = Neck.transform.position + Head.transform.rotation * Vector3.forward * distance + (enabled ? Body.transform.right * 0.1f : -Body.transform.right * 0.1f);
    }
}