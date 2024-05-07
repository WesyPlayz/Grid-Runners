using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class User_2 : UserHandler
{
    private PlayerInput playerInput;
    public PlayerInputActions playerInputActions;

    public override void Init()
    {
        playerInput = User.GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }
}
