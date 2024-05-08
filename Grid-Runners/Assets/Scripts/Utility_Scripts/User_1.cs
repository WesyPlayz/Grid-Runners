using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class User_1 : UserHandler
{
    protected override void Awake()
    {
        base.Awake();
        Actions = new PlayerInputActions();
        Gamepad[] gamepads = Gamepad.all.ToArray();
        if (gamepads.Length > 1)
            player_Input.SwitchCurrentControlScheme("Controller", gamepads[1]);
        else
            player_Input.SwitchCurrentControlScheme("Computer", Keyboard.current, Mouse.current);
        Actions.user_Input_1.Enable();
    }

    // Control Binding System:
    protected override void BindActions(Mode mode)
    {
        // Movement Unbindings:
        if (bound_Actions.ContainsKey("Move"))
        {
            Actions.user_Input_1.Move.performed -= bound_Actions["Move"];
            bound_Actions.Remove("Move");
        }
        if (bound_Actions.ContainsKey("Sprint"))
        {
            Actions.user_Input_1.Sprint.performed -= bound_Actions["Sprint"];
            Actions.user_Input_1.Sprint.canceled -= bound_Actions["Sprint"];
            bound_Actions.Remove("Sprint");
        }
        if (bound_Actions.ContainsKey("Jump"))
        {
            Actions.user_Input_1.Jump.performed -= bound_Actions["Jump"];
            bound_Actions.Remove("Jump");
        }

        // Action Unbindings
        if (bound_Actions.ContainsKey("Attack"))
        {
            Actions.user_Input_1.Attack.performed -= bound_Actions["Attack"];
            Actions.user_Input_1.Attack.canceled -= bound_Actions["Attack"];
            bound_Actions.Remove("Attack");
        }
        else if (bound_Actions.ContainsKey("Build"))
        {
            Actions.user_Input_1.Attack.performed -= bound_Actions["Build"];
            bound_Actions.Remove("Build");
        }
        if (bound_Actions.ContainsKey("Reload"))
        {
            Actions.user_Input_1.Reload.performed -= bound_Actions["Reload"];
            bound_Actions.Remove("Reload");
        }

        // Camera Bindings:
        if (bound_Actions.ContainsKey("ADS"))
        {
            Actions.user_Input_1.Scope.performed -= bound_Actions["ADS"];
            Actions.user_Input_1.Scope.canceled -= bound_Actions["ADS"];
            bound_Actions.Remove("ADS");
        }
        if (bound_Actions.ContainsKey("Peek Right"))
        {
            Actions.user_Input_1.Peek_Right.performed -= bound_Actions["Peek Right"];
            Actions.user_Input_1.Peek_Right.canceled -= bound_Actions["Peek Right"];
            bound_Actions.Remove("Peek Right");
        }
        if (bound_Actions.ContainsKey("Peek Left"))
        {
            Actions.user_Input_1.Peek_Left.performed -= bound_Actions["Peek Left"];
            Actions.user_Input_1.Peek_Left.canceled -= bound_Actions["Peek Left"];
            bound_Actions.Remove("Peek Left");
        }

        // Inventory Unbindings:
        if (bound_Actions.ContainsKey("Switch Weapons"))
        {
            Actions.user_Input_1.Switch_Weapon.performed -= bound_Actions["Switch Weapons"];
            bound_Actions.Remove("Switch Weapons");
        }

        // Bind Common Actions
        if (mode != Mode.Menu)
        {
            // Movement Bindings:
            if (!bound_Actions.ContainsKey("Move"))
            {
                Action<InputAction.CallbackContext> move_Action = phase => ControlledUpdate(phase, User_Input.Move);
                Actions.user_Input_1.Move.performed += move_Action;
                bound_Actions.Add("Move", move_Action);
            }
            if (!bound_Actions.ContainsKey("Sprint"))
            {
                Action<InputAction.CallbackContext> sprint_Action = Sprint;
                Actions.user_Input_1.Sprint.performed += sprint_Action;
                Actions.user_Input_1.Sprint.canceled += sprint_Action;
                bound_Actions.Add("Sprint", sprint_Action);
            }
        }

        // Bind Specific Actions
        if (mode == Mode.Play)
        {
            // Movement Bindings:
            if (!bound_Actions.ContainsKey("Jump"))
            {
                Action<InputAction.CallbackContext> jump_Action = Jump;
                Actions.user_Input_1.Jump.performed += jump_Action;
                bound_Actions.Add("Jump", jump_Action);
            }

            // Action Bindings:
            if (!bound_Actions.ContainsKey("Attack"))
            {
                Action<InputAction.CallbackContext> attack_Action = phase => ControlledUpdate(phase, User_Input.Attack);
                Actions.user_Input_1.Attack.performed += attack_Action;
                Actions.user_Input_1.Attack.canceled += attack_Action;
                bound_Actions.Add("Attack", attack_Action);
            }
            if (!bound_Actions.ContainsKey("Reload"))
            {
                Action<InputAction.CallbackContext> reload_Action = phase => ControlledUpdate(phase, User_Input.Reload);
                Actions.user_Input_1.Reload.performed += reload_Action;
                bound_Actions.Add("Reload", reload_Action);
            }

            // Camera Bindings:
            if (!bound_Actions.ContainsKey("ADS"))
            {
                Action<InputAction.CallbackContext> ads_Action = camera_Handler.ADS;
                Actions.user_Input_1.Scope.performed += ads_Action;
                Actions.user_Input_1.Scope.canceled += ads_Action;
                bound_Actions.Add("ADS", ads_Action);
            }
            if (!bound_Actions.ContainsKey("Peek Right"))
            {
                Action<InputAction.CallbackContext> peek_Action = phase => camera_Handler.Peek(phase, CameraHandler.Side.Right);
                Actions.user_Input_1.Peek_Right.performed += peek_Action;
                Actions.user_Input_1.Peek_Right.canceled += peek_Action;
                bound_Actions.Add("Peek Right", peek_Action);
            }
            if (!bound_Actions.ContainsKey("Peek Left"))
            {
                Action<InputAction.CallbackContext> peek_Action = phase => camera_Handler.Peek(phase, CameraHandler.Side.Left);
                Actions.user_Input_1.Peek_Left.performed += peek_Action;
                Actions.user_Input_1.Peek_Left.canceled += peek_Action;
                bound_Actions.Add("Peek Left", peek_Action);
            }

            // Inventory Bindings:
            if (!bound_Actions.ContainsKey("Switch Weapons"))
            {
                Action<InputAction.CallbackContext> switch_Weapons_Action = phase => Switch_Weapons(phase);
                Actions.user_Input_1.Switch_Weapon.performed += switch_Weapons_Action;
                bound_Actions.Add("Switch Weapons", switch_Weapons_Action);
            }
        }
        else if (mode == Mode.Build)
        {
            // Action Bindings:
            if (!bound_Actions.ContainsKey("Build"))
            {
                Action<InputAction.CallbackContext> build_Action = Build;
                Actions.user_Input_1.Attack.performed += build_Action;
                bound_Actions.Add("Build", build_Action);
            }
        }
    }

    public override Vector3 GetInputVector()
    {
        return Actions.user_Input_1.Move.ReadValue<Vector3>(); // Axis Values
    }
    public override float GetInputAxis(User_Axis axis)
    {
        if (axis == User_Axis.Horizontal)
            return Actions.user_Input_1.MouseX.ReadValue<float>();
        else
            return Actions.user_Input_1.MouseY.ReadValue<float>();
    }
}