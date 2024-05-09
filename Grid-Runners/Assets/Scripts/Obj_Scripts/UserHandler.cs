using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Utilities.Collisions;
using static Utilities.Generic;
using UnityEngine.InputSystem;

public abstract class UserHandler : MonoBehaviour
{
    [Header("User Variables")]
    public GameObject User;
    public GameObject user_Spectate;

    [HideInInspector] public GameObject Neck;

    // User Physics Variables:
    private CharacterController
        user_Controller,
        spectator_Controller;

    private Collider body_Hitbox;
    private Bounds body_Hitbox_Bounds;

    // User Script Variables:
    public CameraHandler camera_Handler;

    private UIHandler ui_Handler;
    private HUDHandler hud_Handler;

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
    protected virtual void Awake()
    {

    }

    // Variable Initialization System:
    private void Start()
    {
        // Initiate User Variables:
        Neck = User.transform.Find("Neck").gameObject;

        // Initiate User Physics Variables:
        user_Controller = User.GetComponent<CharacterController>();
        spectator_Controller = user_Spectate.GetComponent<CharacterController>();
        body_Hitbox = User.GetComponent<CharacterController>();

        // Initiate User Script Variables:
        camera_Handler = user_Camera.GetComponent<CameraHandler>();

        grid_Data = GameObject.Find("Grid").GetComponent<Grid_Data>();
        item_Data = GameObject.Find("Items").GetComponent<new_Item_Data>();

        Health = max_Health;

        secondary_Data = secondary_Obj.GetComponent<Obj_State>();

        // Initiate User Input Variables:
        //playerInput = User.GetComponent<PlayerInput>();
        //playerInputActions = new PlayerInputActions();
        //playerInputActions.Player.Enable();

        //Action<InputAction.CallbackContext> build_Action = Build;
        //playerInputActions.Player.Attack.performed += build_Action;

        //playerInputActions.Player.Grenade.performed += phase => ControlledUpdate(phase, User_Input.Attack); // Ordinance Active
        //playerInputActions.Player.Grenade.canceled += phase => ControlledUpdate(phase, User_Input.Attack); // Ordinance Inactive
    }

    // Current Binding Collection:
    public Dictionary<string, Action<InputAction.CallbackContext>> bound_Actions = new Dictionary<string, Action<InputAction.CallbackContext>>();


    protected virtual void BindActions(Mode mode)
    {

    }

    // Mode Systems:
    public Mode SwapMode(Mode mode_State)
    {
        if (!changing_Mode)
        {
            changing_Mode = true;

            // User Protection System:
            if (mode_State != Mode.Play)
            {
                body_Hitbox_Bounds = body_Hitbox.bounds;
                if (!can_Use_Action && is_Using_Action)
                {
                    Item current_Item = item_Data.Items[(selected_Weapon == 0 ? primary_Weapon : secondary_Weapon)];
                    if (current_Item is Ranged ranged_Item)
                        ranged_Item.Aim(this, is_Using_Action = false);
                    can_Use_Action = true;
                }
            }

            // Control Rebinding System:
            BindActions(mode_State);

            // Object Swapping System:
            User.SetActive(mode_State == Mode.Play);
            user_Spectate.SetActive(mode_State == Mode.Build);

            // Camera Repositioning System:
            Transform cam_Pos = mode_State == Mode.Build ? camera_Handler.spec_Cam_Pos.transform : mode_State == Mode.Play ? camera_Handler.user_Cam_Pos.transform : camera_Handler.menu_Cam_Pos.transform;
            camera_Handler.user_Camera.transform.SetPositionAndRotation(cam_Pos.position, cam_Pos.rotation);
            camera_Handler.user_Camera.transform.parent = cam_Pos;

            changing_Mode = false;
            return mode_State;
        }
        return Mode.Idle;
    }

    // Inventory System:
    public void EquipWeapon(int weapon_Slot, int weapon, InputAction.CallbackContext phase)
    {
        if (Weapon != null)
            Destroy(Weapon);

        Item selected_Weapon = item_Data.Items[weapon];
        current_Weapon = selected_Weapon;
        GameObject new_Weapon = Instantiate(selected_Weapon.weapon_Prefab);
        new_Weapon.transform.parent = item_Holder.transform;
        new_Weapon.transform.SetPositionAndRotation(item_Holder.transform.position, item_Holder.transform.rotation);

        Weapon = new_Weapon;
        hud_Handler.current_Weapon_Name.text = selected_Weapon.weapon_Prefab.name;

        if (weapon_Slot == 0 && selected_Weapon.Icon != hud_Handler.primary_Weapon_Icon)
            hud_Handler.primary_Weapon_Icon = selected_Weapon.Icon;
        else if (weapon_Slot == 1 && selected_Weapon.Icon != hud_Handler.secondary_Weapon_Icon)
            hud_Handler.secondary_Weapon_Icon = selected_Weapon.Icon;
        else
        {
            hud_Handler.holstered_Weapon_Icon.sprite = hud_Handler.current_Weapon_Icon.sprite;
            hud_Handler.current_Weapon_Icon.sprite = selected_Weapon.Icon;
        }
        if (selected_Weapon is Ranged ranged_Item)
        {
            foreach (Transform child in new_Weapon.transform)
            {
                switch (child.name)
                {
                    case "Fire_Point":
                        fire_Point = child.gameObject;
                        break;
                    case "Muzzle_Flash":
                        muzzle_Flash = child.GetComponent<ParticleSystem>();
                        break;
                }
            }
            Ammo =
            weapon == primary_Weapon && weapon_Slot == 0 ? primary_Ammo :
            weapon == secondary_Weapon && weapon_Slot == 1 ? secondary_Ammo :
            ranged_Item.max_Ammo;
        }
    }

    // Control Terminal:
    public void ControlledUpdate(InputAction.CallbackContext phase, User_Input input)
    {
        switch (input) // Filter
        {
            case User_Input.Move:
                if (phase.performed && !is_Walking) // Move Active:
                    StartCoroutine(Move(is_Walking = true));
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
                            ranged_Item.Attack(this);
                    }
                    else if (current_Weapon is Melee melee_Item) // Melee Check:
                    {
                        if (phase.performed)
                            melee_Item.Action(this);
                        else if (phase.canceled)
                            melee_Item.Attack(this);
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
            verticalVelocity = jumpForce;

    }

    // Action Systems:
    public void Use_Ability(InputAction.CallbackContext phase)
    {
        
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