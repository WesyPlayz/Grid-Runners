using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using static Utilities.Collisions;
using static Utilities.Generic;

public abstract class UserHandler : MonoBehaviour
{
    [Header("User Variables")]
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

    private UIHandler ui_Handler;
    public HUDHandler hud_Handler;
    public GameManager game_manager;
    public int player;

    private Grid_Data grid_Data;
    [HideInInspector] public new_Item_Data item_Data;

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
    public int points_Per_Kill;

    private bool changing_Mode;

    public float max_Health;
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
    public GameObject Grenade;
    private GameObject held_Grenade;

    public ParticleSystem muzzle_Flash;
    public AudioSource pewpew;
    public AudioSource ReloadSound;
    public AudioSource WalkSFX;
    public AudioSource climb1;
    public AudioSource climb2;
    public AudioSource climb3;
    public AudioSource Slashy;
    public AudioSource jumpy;

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

    // Input Initiation System:
    protected virtual void Awake(){}

    // Variable Initialization System:
    private void Start()
    {
        // Initiate User Variables:
        Neck = User.transform.Find("Neck").gameObject;

        // Initiate User Physics Variables:
        user_Controller = User.GetComponent<CharacterController>();
        spectator_Controller = user_Spectate.GetComponent<CharacterController>();
        body_Hitbox = User.GetComponent<CharacterController>();
        //game_manager = Gm_OBJ.GetComponent<GameManager>();

        // Initiate User Script Variables:
        camera_Handler = user_Camera.GetComponent<CameraHandler>();

        grid_Data = GameObject.Find("Grid").GetComponent<Grid_Data>();
        item_Data = GameObject.Find("Items").GetComponent<new_Item_Data>();

        Health = max_Health;

        secondary_Data = secondary_Obj.GetComponent<Obj_State>();
    }

    // Current Binding Collection:
    public Dictionary<string, Action<InputAction.CallbackContext>> bound_Actions = new Dictionary<string, Action<InputAction.CallbackContext>>();
    protected virtual void BindActions(Mode mode){}

    // Mode Systems:
    public Mode SwapMode(Mode mode_State)
    {
        if (!(changing_Mode && mode_State == Mode.Idle))
        {
            changing_Mode = true;

            if (mode_State != Mode.Play)
            {
                // User Protection System:
                body_Hitbox_Bounds = body_Hitbox.bounds;
                if (!can_Use_Action && is_Using_Action)
                {
                    Item current_Item = item_Data.Items[(selected_Weapon == 0 ? primary_Weapon : secondary_Weapon)];
                    if (current_Item is Ranged ranged_Item)
                        ranged_Item.Aim(this, is_Using_Action = false);
                    can_Use_Action = true;
                }

                // Menu Activation System:
                if (mode_State == Mode.Build)
                    hud_Handler.HUD_Menus[0].SetActive(true);
                else
                    hud_Handler.HUD_Menus[1].SetActive(true);
            }
            else
            {
                // Menu Deactivation System:
                foreach (GameObject menus in hud_Handler.HUD_Menus)
                {
                    menus.SetActive(false);
                    cursor_Handler.user_Cursor.SetActive(false);
                }
            }

            // Control Rebinding System:
            BindActions(mode_State);

            // Object Swapping System:
            User.SetActive(mode_State == Mode.Play);
            user_Spectate.SetActive(mode_State == Mode.Build);
            cursor_Handler.user_Cursor.SetActive(mode_State != Mode.Play);

            // Camera Repositioning System:
            Transform cam_Pos = mode_State == Mode.Build ? camera_Handler.spec_Cam_Pos.transform : mode_State == Mode.Play ? camera_Handler.user_Cam_Pos.transform : camera_Handler.menu_Cam_Pos.transform;
            camera_Handler.user_Camera.transform.SetPositionAndRotation(cam_Pos.position, cam_Pos.rotation);
            camera_Handler.user_Camera.transform.parent = cam_Pos;

            changing_Mode = false;
            return mode_State;
        }
        return Mode.Idle;
    }

    // Control Terminal:
    public void ControlledUpdate(InputAction.CallbackContext phase, User_Input input)
    {
        switch (input) // Filter
        {
            case User_Input.Move:
                if (phase.performed && !is_Walking) // Move Active:
                {
                    StartCoroutine(Move(is_Walking = true));
                    if (!WalkSFX.isPlaying) WalkSFX.Play();
                }
                break;
            case User_Input.Attack:
                if (phase.performed && can_Attack && !is_Attacking) // Attack Active:
                {
                    can_Attack = false;
                    if (current_Weapon is Ranged ranged_Item && Ammo > 0) // Range Check:
                    {
                        if (ranged_Item.is_Auto) // Auto Check:
                            item_Data.StartCoroutine(item_Data.Fire_Rate(this, ranged_Item, is_Attacking = true));
                        else
                        { ranged_Item.Attack(this);
                            pewpew.Play();}
                    }
                    else if (current_Weapon is Melee melee_Item) // Melee Check:
                    {
                        if (phase.performed)
                            melee_Item.Action(this);
                        else if (phase.canceled)
                        {
                            melee_Item.Attack(this);
                            Slashy.Play();
                        }
                        if (Actions.user_Input.Scope.inProgress)
                            melee_Item.Aim(this, true);
                    }
                    else if (current_Weapon is Ordinance ordinance)
                        ordinance.Attack(this);
                    else
                        can_Attack = true;
                }
                else if (phase.canceled && is_Attacking) // Attack Inactive:
                    is_Attacking = false;
                break;
            case User_Input.Reload:
                if (current_Weapon is Ranged ranged_Weapon) // Range Check:
                {
                    if (phase.performed && can_Attack && Ammo < ranged_Weapon.max_Ammo) // Reload Active:
                    {
                        can_Attack = false;
                        ranged_Weapon.Reload(this);
                        ReloadSound.Play();
                    }
                }
                break;
        }
    }
    public void FixedUpdate()
    {
        if (current_Mode == Mode.Play)
        {
            // Gravity System:
            if (wasGrounded && !ground && verticalVelocity < 0) // Unlock Gravity:
                verticalVelocity = 0;
            if (ground && verticalVelocity != jumpForce) // Lock Gravity:
                verticalVelocity = -(gravity + 1);
            else if (verticalVelocity >= -gravity) // Apply Gravity:
                verticalVelocity = Mathf.Clamp(verticalVelocity - gravity * Time.deltaTime, -gravity, Mathf.Infinity);

            // Velocity System:
            Vector3 velocity = user_Controller.velocity;
            velocity.y = verticalVelocity;
            user_Controller.Move(velocity * Time.deltaTime);

            // Floor Collision Check:
            wasGrounded = ground;
            ground = user_Controller.isGrounded;
        }
    }

    // Damage System:
    public bool Damage(float dmg = 0) // Damage System:
    {
        if (Health <= 0) // Health Check:
            return false;
        else if (dmg > 0) // Damage Check:
        {
            Health = Mathf.Max(Health - dmg, 0);
            if (Health <= 0)
                Respawn();
        }
        return true;
    }

    public void Respawn() // Respawn System:
    {
        User.transform.position = Spawn.transform.position; // Relocate User
        Health = max_Health;
        game_manager.P1_Points += player == 1 ? 2 : 0;
        game_manager.P2_Points += player == 1 ? 0 : 2;
    }

    // Movement Systems:
    public IEnumerator Move(bool state) // Movement System:
    {
        while (is_Walking) // Movement Active:
        {
            Vector3 inputVector = GetInputVector(); // Axis Values
            if (inputVector == Vector3.zero) // Input Check
                is_Walking = false;
            Vector3 move_Direction = (current_Mode == Mode.Play ? User : user_Spectate).transform.TransformDirection(new Vector3(inputVector.x, current_Mode == Mode.Play ? 0 : inputVector.y, inputVector.z)); // Movement Calculation
            (current_Mode == Mode.Play ? user_Controller : spectator_Controller).Move(move_Direction * (is_Sprinting && ground ? sprint_Speed : walk_Speed)); // Move System & Speed Check
            yield return new WaitForSeconds(0.001f);
        }
    }

    public void Sprint(InputAction.CallbackContext phase) // Sprint System:
    {
        is_Sprinting = phase.performed ? true : false; // Sprint Check
    }
    public void Jump(InputAction.CallbackContext phase) // Jump System:
    {
        if (verticalVelocity <= -(gravity + 1) && phase.performed) // Jump Active:
        { verticalVelocity = jumpForce;
            jumpy.Play();
        }

    }

    // Action Systems:
    public void Use_Ability(InputAction.CallbackContext phase)
    {
        print("lol");
    }

    public void Build(InputAction.CallbackContext phase)
    {
        RaycastHit target;
        if (Physics.Raycast(camera_Handler.user_Camera.ScreenPointToRay(Input.mousePosition), out target))
        {
            GameObject selected_Target = target.transform.gameObject;
            if (selected_Target.CompareTag("Grid_Tile")) { }
                grid_Data.PlaceObject(this, selected_Target, target.normal);
        }
    }

    // Camera SYstems:
    public virtual Vector3 GetInputVector()
    {
        return Vector3.zero;
    }
    public virtual float GetInputAxis(User_Axis axis)
    {
        return 0;
    }

    // Inventory Systems:
    public void Switch_Weapons(InputAction.CallbackContext phase)
    {
        if (current_Slot == Slot.Primary)
            primary_Ammo = Ammo;
        else
            secondary_Ammo = Ammo;
        current_Slot = (current_Slot == Slot.Primary ? Slot.Secondary : Slot.Primary);
        item_Data.Equip_Weapon(this, (current_Slot == Slot.Primary ? 0 : 1), (current_Slot == Slot.Primary ? primary_Weapon : secondary_Weapon));
    }

    public void Switch_Ability(GameObject ability)
    {
        current_ability = ability;
    }
}