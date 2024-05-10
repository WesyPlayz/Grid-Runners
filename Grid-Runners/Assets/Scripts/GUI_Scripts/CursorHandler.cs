using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

using TMPro;
using System.Security.Cryptography.X509Certificates;

public class CursorHandler : MonoBehaviour
{
    public GameObject user_Cursor;

    private UserHandler user_Handler;
    private GameObject player_Obj;

    public float cursor_Sensitivity;

    public RectTransform cursorRectTransform;

    [Header("user Variables")]
    [Range(1, 2)]
    public int Player;

    void Start()
    {
        player_Obj = GameObject.Find((Player == 1 ? "Player_1" : "Player_2")); // Finds which player this script is assigned to

        user_Handler = player_Obj.GetComponent<UserHandler>();

        Cursor.lockState = CursorLockMode.Locked;
        cursorRectTransform = GetComponent<RectTransform>();
    }

    // Cursor Movement System:
    void Update()
    {
        if (user_Handler.current_Mode == UserHandler.Mode.Menu)
        {
            float lookHorizontal = cursor_Sensitivity * user_Handler.GetInputAxis(UserHandler.User_Axis.Horizontal); // Y-Axis Rotational Value
            float lookVertical = cursor_Sensitivity * user_Handler.GetInputAxis(UserHandler.User_Axis.Vertical); // X-Axis Rotational Value

            if (lookHorizontal != 0 || lookVertical != 0)
            {
                Vector2 cursorPosition = cursorRectTransform.anchoredPosition;
                cursorPosition.x += lookHorizontal;
                cursorPosition.y += lookVertical;

                RectTransform canvasRectTransform = cursorRectTransform.parent as RectTransform;
                Vector2 canvasSize = canvasRectTransform.sizeDelta;

                cursorPosition.x = Mathf.Clamp(cursorPosition.x, -(canvasSize.x / 2), (canvasSize.x / 2));
                cursorPosition.y = Mathf.Clamp(cursorPosition.y, -(canvasSize.y / 2), (canvasSize.y / 2));

                cursorRectTransform.anchoredPosition = cursorPosition;
            }
        }
    }

    // Cursor Select System:
    public void Select(InputAction.CallbackContext phase)
    {
        print("hi");
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, cursorRectTransform.position);
        pointerData.position = screenPosition;

        List<RaycastResult> ui_Elements = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, ui_Elements);

        if (ui_Elements.Count > 0)
        {
            foreach (RaycastResult ui_Element in ui_Elements)
            {
                print(ui_Element);
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