using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Utilities.Collisions;
using static Utilities.Generic;
using UnityEngine.InputSystem;

public class UserHandler : MonoBehaviour
{
    [Header("User Variables")]
    public GameObject User, user_Spectate;
    private Rigidbody user_Physics, user_Spectate_Physics;

    public Collider body_Hitbox;
    public Bounds body_Hitbox_Bounds;

    private PlayerInput playerInput;
    public PlayerInputActions playerInputActions;

    [Header("Data Variables")]
    public GameObject Spawn;

    public Obj_State obj_Data;
    private HUDHandler hud_Handler;

    private bool changing_Mode;

    public float max_Health;
    public float Health;

    [Range(0, 1)]
    public int Mode;

    [Header("Camera Variables")]
    public Camera user_Camera, ui_Camera;
    public GameObject Camera_Pos0, Camera_Pos1;

    public float origin_FOV;
    public float ADS_FOV;

    // Movement Variables:
    private bool is_Sprinting;

    [Header("Weapon Variables")]
    public GameObject item_Holder;
    public GameObject Weapon;
    public GameObject Projectiles;

    public new_Item_Data item_Data;

    public int selected_Weapon;

    public int primary_Weapon;
    public int secondary_Weapon;

    public GameObject fire_Point;
    public GameObject Projectile;
    public GameObject Grenade;
    private GameObject held_Grenade;

    public ParticleSystem muzzle_Flash;
    public AudioSource pewpew;
    public AudioSource ReloadSound;

    public bool Ranged;
    public bool Melee;
    public bool hit_Scan;

    public bool can_Attack = true;
    public bool can_Use_Action = true;
    public bool is_Using_Action;

    public float fire_Rate;
    public float grenade_Rate;
    public float throw_force;
    public float bullet_Speed;

    public int max_Ammo;
    public int Ammo;

    public int primary_Ammo;
    public int secondary_Ammo;

    public int grenades;
    public int max_Grenades;

    [Header("Sound Resources")]
    public float volume;
    public AudioSource mySpeaker;
    public AudioClip laser_Reload;
    public AudioSource laser_Fire;
    public AudioClip hit_SFX;
    public AudioClip melee;
    public AudioSource walkSFX;
    private float WalkSoundTimer;

    // Grid Variables:
    private Grid_Data grid_Data;

    public GameObject secondary_Obj;
    private Transform obj_Transform;
    private Obj_State secondary_Data;

    // Jumping Variables:

    // Collision Variables:
    public bool on_Floor;
    public bool on_Wall;

    // Variable Initialization System:
    private void Start()
    {
        // Initiate User Variables:
        user_Physics = User.GetComponent<Rigidbody>();
        user_Spectate_Physics = user_Spectate.GetComponent<Rigidbody>();

        obj_Data = GetComponent<Obj_State>();
        hud_Handler = GetComponent<HUDHandler>();
        Health = max_Health;

        origin_FOV = ui_Camera.fieldOfView;
        secondary_Data = secondary_Obj.GetComponent<Obj_State>();
        obj_Transform = GetComponent<Transform>();

        grid_Data = GameObject.Find("Grid").GetComponent<Grid_Data>();
        item_Data = GameObject.Find("Items").GetComponent<new_Item_Data>();

        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();

        // Movement Systems:
        playerInputActions.Player.Sprint.performed += Sprint; // Sprint Active
        playerInputActions.Player.Sprint.canceled += Sprint; // Sprint Inactive

        playerInputActions.Player.Movement.performed += Move;

        if (gameObject.name == "Player_1")
        {
            playerInputActions.Player.Enable();
            playerInputActions.Player.Jump.performed += Jump;
            playerInputActions.Player.Grenade.performed += Throw_Grenade;
        }
        else
        {
            playerInputActions.Player1.Enable();
            playerInputActions.Player1.Jump.performed += Jump;
            playerInputActions.Player1.Grenade.performed += Hold_Grenade;
            playerInputActions.Player1.Grenade.canceled += Throw_Grenade;
        }
    }

    private void Update()
    {
        bool mode_State = Mode == 0; // Mode Check.

        if (!changing_Mode)
        {
            if (!Damage()) // Health Check:
                Respawn();

            // Camera Systems:
            if (Input.GetKeyDown(KeyCode.Tab)) // Will be changed.
                SwapMode(mode_State);

            // Movement_Systems:
            /*
            Rigidbody current_Physics = mode_State ? user_Physics : user_Spectate_Physics; // Physics Check.

            Vector3 inputVector = playerInputActions.Player.Movement.ReadValue<Vector3>();
            Vector3 move_Direction = (mode_State ? User : user_Spectate).transform.TransformDirection(new Vector3(inputVector.x, (mode_State ? 0 : inputVector.y), inputVector.z)); // Movement Direction Calculation.
            current_Physics.AddForce(move_Direction * (is_Sprinting ? obj_Data.sprint_Speed : obj_Data.walk_Speed) * 10);

            Vector3 clamped_Velocity = Vector3.ClampMagnitude( // Velocty Clamp Calculation:
                (mode_State ? new Vector3(user_Physics.velocity.x, 0, user_Physics.velocity.z) :
                user_Spectate_Physics.velocity),
                (move_Direction == Vector3.zero ? 0 :
                is_Sprinting ? obj_Data.sprint_Speed :
                obj_Data.walk_Speed));
            current_Physics.velocity = new Vector3(clamped_Velocity.x, (mode_State ? user_Physics.velocity.y : clamped_Velocity.y), clamped_Velocity.z); // Clamps Velocity To Calculations.
            */
            if (mode_State)
            {
                // Walk Sounds
                WalkSoundTimer -= Time.deltaTime;
                if (playerInputActions.Player.Movement.inProgress)
                {
                    if (WalkSoundTimer <= 0 && on_Floor)
                    {
                        WalkSoundTimer = .65f;
                        walkSFX.volume = Vector3.Magnitude(user_Physics.velocity) * .35f;
                        walkSFX.Play();
                    }
                }
            }
            else
                walkSFX.loop = false;

            // Action Systems:
            if (mode_State)
            {
                if (selected_Weapon != 0 && Input.GetKeyDown(KeyCode.Alpha1))
                {
                    selected_Weapon = 0;
                    secondary_Ammo = Ammo;
                    EquipWeapon(selected_Weapon, primary_Weapon);
                }
                else if (selected_Weapon != 1 && Input.GetKeyDown(KeyCode.Alpha2))
                {
                    selected_Weapon = 1;
                    primary_Ammo = Ammo;
                    EquipWeapon(selected_Weapon, secondary_Weapon);
                }
                Item current_Item = item_Data.Items[(selected_Weapon == 0 ? primary_Weapon : secondary_Weapon)];
                if (current_Item is Ranged ranged_Item) // Ranged Attack System:
                {
                    if (can_Attack && playerInputActions.Player.Shooting.inProgress && Ammo > 0)
                    {
                        can_Attack = false;
                        ranged_Item.Attack(this);
                    }
                    else if (can_Attack && playerInputActions.Player.Reload.IsPressed() && Ammo < max_Ammo)
                    {
                        mySpeaker.PlayOneShot(laser_Reload, volume);
                        can_Attack = false;
                        ReloadSound.Play();
                        ranged_Item.Reload(this);
                    }
                }
                else if (Melee) // Melee Attack System:
                {
                    if (can_Attack && secondary_Data.collided_Entity != null && playerInputActions.Player.Knife.IsPressed())
                    {
                        mySpeaker.PlayOneShot(melee, volume);
                        can_Attack = false;
                        MeleeAttack();
                        StartCoroutine(AttackCooldown(obj_Data.Attack_Cooldown));
                    }
                }
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.Mouse0)) // Build System:
                {
                    RaycastHit target;
                    if (Physics.Raycast(user_Camera.ScreenPointToRay(Input.mousePosition), out target))
                    {
                        GameObject selected_Target = target.transform.gameObject;
                        if (selected_Target.CompareTag("Grid_Tile"))
                            PlaceObject(selected_Target, target.normal);
                    }
                }
            }
        }
    }

    // Health System:
    public bool Damage(float dmg = 0) // Damage System:
    {
        if (Health <= 0)
            return false;
        else if (dmg > 0)
            Health = Mathf.Max(Health - dmg, 0);
        return true;
    }
    public void Respawn() // Respawn System:
    {
        User.transform.position = Spawn.transform.position;
        Health = max_Health;
    }

    // Mode Systems:
    public void SwapMode(bool mode_State)
    {
        changing_Mode = true;

        // Mode State ID:
        Transform cam_Pos = mode_State ? Camera_Pos0.transform : Camera_Pos1.transform;

        // User Protection System:
        if (mode_State)
        {
            body_Hitbox_Bounds = body_Hitbox.bounds; // (Tutorial Only)
            if (!can_Use_Action && is_Using_Action)
            {
                Item current_Item = item_Data.Items[(selected_Weapon == 0 ? primary_Weapon : secondary_Weapon)];
                if (current_Item is Ranged ranged_Item)
                    ranged_Item.Aim(this, is_Using_Action = false);
                can_Use_Action = true;
            }
        }

        // Object Swapping System:
        User.SetActive(!mode_State);
        user_Spectate.SetActive(mode_State);

        // Camera Repositioning System:
        user_Camera.transform.SetPositionAndRotation(cam_Pos.position, cam_Pos.rotation);
        user_Camera.transform.parent = cam_Pos;

        Mode = 1 - Mode; // Mode Swapping.
        changing_Mode = false;
    }

    // Inventory System:
    public void EquipWeapon(int weapon_Slot, int weapon)
    {
        if (Weapon != null)
            Destroy(Weapon);

        Item selected_Weapon = item_Data.Items[weapon];
        GameObject new_Weapon = Instantiate(selected_Weapon.weapon_Prefab);
        new_Weapon.transform.parent = item_Holder.transform;
        new_Weapon.transform.SetPositionAndRotation(item_Holder.transform.position, item_Holder.transform.rotation);

        Weapon = new_Weapon;
        hud_Handler.current_Weapon_Name.text = selected_Weapon.weapon_Prefab.name;

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
            Ammo =
            weapon == primary_Weapon && weapon_Slot == 0 ? primary_Ammo :
            weapon == secondary_Weapon && weapon_Slot == 1 ? secondary_Ammo :
            ranged_Item.max_Ammo;
        }
    }

    // Action Systems:
    private void PlaceObject(GameObject placement_Zone, Vector3 target_Normal) // Grid Placement System:
    {
        Bounds placement_Bounds = new Bounds(placement_Zone.transform.position + Vector3.Scale(target_Normal, placement_Zone.transform.localScale), placement_Zone.transform.localScale);
        if (!placement_Bounds.Intersects(user_Spectate.GetComponent<Collider>().bounds) && !placement_Bounds.Intersects(body_Hitbox_Bounds) && Vector3.Distance(placement_Zone.transform.position, user_Spectate.transform.position) <= 10)
        {
            GameObject new_Grid_Obj = Instantiate(grid_Data.grid_Col[grid_Data.current_Gird_Obj]);
            new_Grid_Obj.transform.position = placement_Bounds.center;
        }
    }

    // Action Systems:
    void MeleeAttack()
    {
        mySpeaker.PlayOneShot(melee, volume);
        switch (secondary_Data.collided_Entity.tag)
        {
            case "Dummy":
                DummyHandler dummy_Handler = secondary_Data.collided_Entity.GetComponent<DummyHandler>();
                dummy_Handler.StopAllCoroutines();
                StartCoroutine(dummy_Handler.Shake(0f));
                break;

        }
    }

    public IEnumerator AttackCooldown(float length)
    {
        yield return new WaitForSeconds(length);
        can_Attack = true;
    }

    public void hitSFX()
    {
        mySpeaker.PlayOneShot(hit_SFX, volume);
    }

    void Hold_Grenade(InputAction.CallbackContext phase)
    {
        print(phase);
        if (phase.performed)
        {
            if (grenades > 0)
            {
                GameObject gre = Instantiate(Grenade, fire_Point.transform.position, user_Camera.transform.rotation);
                held_Grenade = gre;
                can_Attack = false;
                grenades--;

            }
        }
    }

    void Throw_Grenade(InputAction.CallbackContext phase)
    {
        print(phase);
        if (phase.canceled)
        {
            StartCoroutine(AttackCooldown(grenade_Rate));
            LinearJump(user_Camera.transform.forward, throw_force, held_Grenade);
        }
    }
    // Movement Systems:
    public void Move(InputAction.CallbackContext phase)
    {
        
    }

    public void Sprint(InputAction.CallbackContext phase)
    {
        is_Sprinting = phase.performed ? true : false; // Sprint Check:
    }

    private void Jump(InputAction.CallbackContext phase)
    {
        if (on_Floor && phase.performed) // Jump Active:
            nonLinearJump(on_Floor, obj_Data.jump_Force, gameObject, User);
    }
}