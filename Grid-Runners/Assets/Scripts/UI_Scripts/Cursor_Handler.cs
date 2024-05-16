// System Libraries :
using System.Collections;
using System.Collections.Generic;

// Unity Engine Libraries :
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Other Package Libraries :
using TMPro;

// Script Class :
public class Cursor_Handler : MonoBehaviour
{
    private Game_Manager game_Manager; // [ Game_Manager Reference ]

    [Header("User Variables")] // Variables Associated With The User :
    //{
        public Player player;
        public enum Player
        {
            Player,
            Player_1,
            Player_2
        }
        private GameObject player_Obj;

        private User_Handler user_Handler;
        private Uni_User_Handler uni_User_Handler;
    //}

    [Header("Cursor Variables")] // Variables Associated With The Cursor :
    //{
        public Input_Type direct_Input;
        public enum Input_Type
        {
            None,
            Singular,
            Universal
        }
        public GameObject user_Cursor;

        [Range(0, 5)] public float cursor_Sensitivity;

        [HideInInspector] public RectTransform cursor_Pos;
    //}

    // Initialization System :
    private string ID_01 = "ID : CH_01";
    /// <summary>
    /// [ ID : CH_01 ]
    /// Params : (None)
    /// </summary>
    public void Start()
    {
        string parameters = string.Empty;

        // Game State Assignment :
        game_Manager = GameObject.Find("Game_Manager").GetComponent<Game_Manager>(); // [ Assigns The Reference Of Game_Manager ]

        // User Assignment :
        player_Obj = GameObject.Find // Finds The Player Which This Script Is Assigned To :
            ((
                player != Player.Player ? // [ Checks If Player Is Not Universal ]
                (
                    player == Player.Player_1 ? "Player_1" : // [ Checks If Player Is FIrst Player ]
                    "Player_2" // [ Assigns To Second Player If All Other's Are False ]
                ) : 
                "Player" // [ Assigns To Universal Player If All Other's Are False ]
            ));
        parameters = "(Player_1 | Player_2 | Player)";
        if (player_Obj == null) // Existential Check For player_obj :
            debug.LogError(ID_01 + "| Null Reference : player_obj was not defined.\nParameters : " + parameters);

        if (game_Manager != null) // Existential Check For game_Manager :
        {
            if (game_Manager.game_State != Game_Manager.State.Linked) // Checks If Game State Is Linked Or Not :
            {
                user_Handler = player_Obj.GetComponent<User_Handler>(); // [ Assigns The Reference Of User_Handler ]
                parameters = "(User_Handler)";
                if (user_Handler == null) // Existential Check For user_Handler :
                {
                    debug.LogError(ID_01 + "| Null Reference : user_Handler was not defined.\nParameters : " + parameters);
                    direct_Input = Input_Type.None; // [ Sets Input To No Players ]
                }
                else
                    direct_Input = Input_Type.Singular; // [ Sets Input To Handle Players Seperately ]
            }
            else
            {
                uni_User_Handler = player_Obj.GetComponent<Uni_User_Handler>(); // [ Assigns The Reference Of Uni_User_Handler ]
                parameters = "(Uni_User_Handler)";
                if (uni_User_Handler == null) // Existential Check For uni_User_Handler :
                {
                    debug.LogError(ID_01 + "| Null Reference : uni_User_Handler was not defined.\nParameters : " + parameters);
                    direct_Input = Input_Type.None; // [ Sets Input To No Players ]
                }
                else
                    direct_Input = Input_Type.Universal; // [ Sets Input To Handle Players Universally ]
            }
        }
        else
        {
            parameters = "(Game_Manager)";
            debug.LogError(ID_01 + "| Null Reference : game_Manager was not defined.\nParameters : " + parameters);
            direct_Input = Input_Type.None; // [ Sets Input To No Players ]
        }

        // Cursor Assignment :
        Cursor.lockState = CursorLockMode.Locked; // [ Locks The Default Desktop Cursor To The Center Of The Screen ]
        cursor_Pos = GetComponent<RectTransform>(); // [ Assigns The Reference Of RectTransform ]
        parameters = "(RectTransform)";
        if (cursor_Pos == null) // Existential Check For cursor_Pos :
        {
            debug.LogError(ID_01 + "| Null Reference : cursor_Pos was not defined.\nParameters : " + parameters);
            direct_Input = Input_Type.None; // [ Sets Input To No Players ]
        }
    }

    // Cursor Input System :
    private string ID_02 = "ID : CH_02";
    /// <summary>
    /// [ ID : CH_02 ]
    /// Params : (None)
    /// </summary>
    public void Update()
    {
        // Initialize Cursor Values :
        float cursor_Horizontal = 0;
        float cursor_Vertical = 0;

        // Direct Input :
        switch (direct_Input)
        {
            // Singular Player Input :
            case Input_Type.Singular:
                if (user_Handler.current_Mode == User_Handler.Mode.Menu) // [ Checks If Player Is Currently In A Menu ]
                {
                    cursor_Horizontal = cursor_Sensitivity * user_Handler.GetInputAxis(UserHandler.User_Axis.Horizontal); // [ Y-Axis Movement Value ]
                    cursor_Vertical = cursor_Sensitivity * user_Handler.GetInputAxis(UserHandler.User_Axis.Vertical); // [ X-Axis Movement Value ]
                }
                break;

            // Universal Player Input :
            case Input_Type.Universal:
                if (uni_User_Handler.current_Mode == Uni_User_Handler.Mode.Menu) // [ Checks If The Players Are Currently In A Menu ]
                {
                    cursor_Horizontal = cursor_Sensitivity * uni_User_Handler.GetInputAxis(Uni_User_Handler.User_Axis.Horizontal); // [ Y-Axis Movement Value ]
                    cursor_Vertical = cursor_Sensitivity * uni_User_Handler.GetInputAxis(Uni_User_Handler.User_Axis.Vertical); // [ X-Axis Movement Value ]
                }
                break;
        }

        // Cursor Movement System :
        if (direct_Input != Input_Type.None && cursor_Horizontal + cursor_Vertical != 0) // [ Checks If Input Is Active ]
            cursor_Pos.anchoredPosition = Move_Cursor(cursor_Horizontal, cursor_Vertical); // [ Run Cursor Movement ]
    }

    // Cursor Movement System :
    private string ID_03 = "ID : CH_03";
    /// <summary>
    /// [ ID : CH_03 ]
    /// Params : (x | y)
    /// </summary>
    public Vector2 Move_Cursor(float x, float y)
    {
        Vector2 anchored_Cursor_Pos = cursor_Pos.anchoredPosition; // [ Transcribes The Current Cursor Position ]

        // Applies Movement To The Cursor's Transcribed Value :
        anchored_Cursor_Pos.x += x;
        anchored_Cursor_Pos.y += y;

        // Finds The Camera Bounds And Limits The Cursor To Them :
        RectTransform canvas_RectTransform = cursor_Pos.parent as RectTransform;
        Vector2 canvas_Size = canvas_RectTransform.sizeDelta;
        anchored_Cursor_Pos.x = Mathf.Clamp(anchored_Cursor_Pos.x, -(canvas_Size.x / 2), (canvas_Size.x / 2));
        anchored_Cursor_Pos.y = Mathf.Clamp(anchored_Cursor_Pos.y, -(canvas_Size.y / 2), (canvas_Size.y / 2));

        return anchored_Cursor_Pos; // [ Returns The Movement Value ]
    }

    // Cursor Select System:
    private string ID_04 = "ID : CH_04";
    /// <summary>
    /// [ ID : CH_04 ]
    /// Params : (phase)
    /// </summary>
    public void Select(InputAction.CallbackContext phase)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, cursor_Pos.position);
        pointerData.position = screenPosition;

        List<RaycastResult> ui_Elements = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, ui_Elements);

        if (ui_Elements.Count > 0)
        {
            foreach (RaycastResult ui_Element in ui_Elements)
            {
                Button button = ui_Element.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                    continue;
                }
            }
        }
    }
}