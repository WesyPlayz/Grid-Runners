using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

using System.Security.Cryptography.X509Certificates;

public class CursorHandler : MonoBehaviour
{
    [SerializeField] private List<Button> Buttons = new List<Button>();

    public GameObject main_Menu;
    public GameObject load_Menu;
    public GameObject pause_Menu;
    private UserHandler user_Handler;
    private GameObject player_Obj;

    public float cursor_Speed;
    public float cursor_Sensitivity;

    public float screen_Border;

    public bool in_Menu;

    private Vector3 right_Joystick_Input;
    private Vector3 current_Cursor_Pos;

    [Header("user Variables")]
    [Range(1, 2)]
    public int Player;

    void Start()
    {
        player_Obj = GameObject.Find((Player == 1 ? "Player_1" : "Player_2")); // Finds which player this script is assigned to

        user_Handler = player_Obj.GetComponent<UserHandler>();

        Cursor.lockState = CursorLockMode.Locked;

        Buttons[0].onClick.AddListener(() => Play_Game()); // Play Button - Main Menu
        Buttons[1].onClick.AddListener(() => Load_Menu_State("Open")); // Load Button - Main Menu
        Buttons[2].onClick.AddListener(() => Load_Menu_State("Close")); // Exit Button - Load Menu

        Buttons[3].onClick.AddListener(() => Quit_Game()); // Quit Button - Main Menu
    }
    void Update()
    {
        if(in_Menu)
        {
            float lookHorizontal = cursor_Sensitivity * user_Handler.playerInputActions.Player.MouseX.ReadValue<float>(); // Y-Axis Rotational Value
            float lookVertical = -cursor_Sensitivity * user_Handler.playerInputActions.Player.MouseY.ReadValue<float>(); // X-Axis Rotational Value
            if (right_Joystick_Input.magnitude > cursor_Sensitivity)
                current_Cursor_Pos = transform.position + new Vector3(lookHorizontal, lookVertical, 0) * cursor_Speed * Time.deltaTime;

            Vector3 clamped_Position = Camera.main.WorldToScreenPoint(current_Cursor_Pos);
            clamped_Position.x = Mathf.Clamp(clamped_Position.x, screen_Border, Screen.width - screen_Border);
            clamped_Position.y = Mathf.Clamp(clamped_Position.y, screen_Border, Screen.height - screen_Border);
            transform.position = Camera.main.ScreenToWorldPoint(clamped_Position);

            if (Input.GetButtonDown("Start"))
            {
                PauseGame();
            }
            if (Time.timeScale == 0)
            {
                if (Input.GetAxis("Horizontal Dpad") < .0)
                {
                    Time.timeScale = 1;
                    MainMenu();
                }
                else if (Input.GetAxis("Horizontal Dpad") > .0)
                    Quit_Game();
            }

            if (Input.GetButtonDown("Jump"))
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = clamped_Position;

                List<RaycastResult> ui_Elements = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, ui_Elements);

                if (ui_Elements.Count > 0)
                {
                    foreach (RaycastResult ui_Element in ui_Elements)
                    {
                        Button button = ui_Element.gameObject.GetComponent<Button>();
                        if (button != null)
                        {
                            End_Action();
                            button.onClick.Invoke();
                            continue;
                        }

                        TMP_Dropdown dropdown = ui_Element.gameObject.GetComponent<TMP_Dropdown>();
                        if (dropdown != null)
                        {
                            End_Action();
                            dropdown.onValueChanged.Invoke(dropdown.value);
                            dropdown.Show();
                            continue;
                        }

                        TMP_InputField inputField = ui_Element.gameObject.GetComponent<TMP_InputField>();
                        if (inputField != null)
                        {
                            End_Action();
                            inputField.ActivateInputField();
                            continue;
                        }
                    }
                }
                else
                    End_Action();
            }
        }
    }

    void PauseGame()
    {
        if (Time.timeScale != 0)
        {
            Cursor.lockState = CursorLockMode.None;
            pause_Menu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            pause_Menu.SetActive(false);
            Time.timeScale = 1;
        }
    }
    void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    void End_Action()
    {
        TMP_Dropdown dropdown = FindObjectOfType<TMP_Dropdown>();
        TMP_InputField inputField = FindObjectOfType<TMP_InputField>();

        if (dropdown != null && dropdown.IsExpanded)
            dropdown.Hide();
        if (inputField != null && inputField.isFocused)
            inputField.DeactivateInputField();
    }
    void Play_Game()
    {
        SceneManager.LoadScene("Start");
    }
    void Load_Menu_State(string state)
    {
        main_Menu.SetActive(state == "Open" ? false : true);
        load_Menu.SetActive(state == "Open" ? true : false);
    }
    void Quit_Game()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
