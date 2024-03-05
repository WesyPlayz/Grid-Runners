using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Utilities.Collisions;
using static Utilities.Generic;

public class UserHandler : MonoBehaviour
{
    [Header("User Variables")]
    public GameObject User, user_Spectate;
    private Rigidbody user_Physics, user_Spectate_Physics;

    public Collider body_Hitbox;
    public Bounds body_Hitbox_Bounds;

    [Header("Data Variables")]
    private Obj_State obj_Data;

    private bool changing_Mode;

    public float Health;

    [Range(0, 1)]
    public int Mode;

    [Header("Camera Variables")]
    public Camera user_Camera, ui_Camera;
    public GameObject Camera_Pos0, Camera_Pos1;

    private float origin_FOV;
    public float ADS_FOV;

    // Movement Variables:
    private bool is_Sprinting;

    [Header("Weapon Variables")]
    public GameObject item_Holder;

    public GameObject fire_Point;
    public GameObject Projectile;
    public GameObject Grenade;

    public ParticleSystem muzzle_Flash;

    public bool Ranged;
    public bool Melee;

    public bool can_Attack = true;
    private bool can_Use_Action = true;
    private bool is_Using_Action;

    public float fire_Rate;
    public float grenade_Rate;
    public float throw_force;

    public int max_Ammo;
    public int Ammo;
    public int grenades, max_Grenades;

    public GameObject secondary_Obj;
    private Transform obj_Transform;
    public GameObject current_Primary, current_Secondary, current_Knife;
    public GameObject new_Weapon;
    public LayerMask enemyTeam;
    public string enemyTeamTag;

    //script data
    private Obj_State NWD; //new weapon data
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

        origin_FOV = ui_Camera.fieldOfView;
        secondary_Data = secondary_Obj.GetComponent<Obj_State>();
        obj_Transform = GetComponent<Transform>();
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
            is_Sprinting = Input.GetKey(KeyCode.LeftShift); // Sprint Check.
            Rigidbody current_Physics = mode_State ? user_Physics : user_Spectate_Physics; // Physics Check.

            Vector3 move_Direction = (mode_State ? User : user_Spectate).transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), (mode_State ? 0 : Input.GetAxis("ThirdAxis")), Input.GetAxis("Vertical"))); // Movement Direction Calculation.
            current_Physics.AddForce(move_Direction * (is_Sprinting ? obj_Data.sprint_Speed : obj_Data.walk_Speed) * 10);

            Vector3 clamped_Velocity = Vector3.ClampMagnitude( // Velocty Clamp Calculation:
                (mode_State ? new Vector3(user_Physics.velocity.x, 0, user_Physics.velocity.z) :
                user_Spectate_Physics.velocity),
                (move_Direction == Vector3.zero ? 0 :
                is_Sprinting ? obj_Data.sprint_Speed :
                obj_Data.walk_Speed));
            current_Physics.velocity = new Vector3(clamped_Velocity.x, (mode_State ? user_Physics.velocity.y : clamped_Velocity.y), clamped_Velocity.z); // Clamps Velocity To Calculations.

            // Jump System:
            if (mode_State && on_Floor && Input.GetKeyDown(KeyCode.Space))
                nonLinearJump(on_Floor, obj_Data.jump_Force, gameObject, User);

            // Action Systems:
            if (mode_State)
            {
                if (Ranged) // Ranged Attack System:
                {
                    if (can_Attack && Input.GetKeyDown(KeyCode.Mouse0) && Ammo > 0)
                    {
                        can_Attack = false;
                        RangedAttack();
                    }
                    else if (Input.GetKeyDown(KeyCode.R) && Ammo != max_Ammo)
                        Ammo = max_Ammo;
                }
                else if (Melee) // Melee Attack System:
                {
                    if (can_Attack && secondary_Data.collided_Entity != null && Input.GetAxis("Knife") != 0)
                    {
                        can_Attack = false;
                        MeleeAttack();
                        StartCoroutine(AttackCooldown(obj_Data.Attack_Cooldown));
                    }
                }

                if (can_Use_Action && !is_Using_Action && Input.GetKeyDown(KeyCode.Mouse1)) // Action System:
                {
                    can_Use_Action = false;
                    if (Ranged) // Ranged Action System:
                        ADS(is_Using_Action = true);
                    else if (Melee) // Melee Action System:
                    {

                    }
                }
                else if (!can_Use_Action && is_Using_Action && Input.GetKeyUp(KeyCode.Mouse1))
                {
                    if (Ranged) // Ranged Action System:
                        ADS(is_Using_Action = false);
                    else if (Melee) // Melee Action System:
                    {

                    }
                    can_Use_Action = true;
                }

                if (Input.GetButtonDown("Grenade"))
                    Throw_Grenade();
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
                ADS(is_Using_Action = false);
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

    // Action Systems:
    void RangedAttack() // Ranged Attack System:
    {
        Ammo--;
        GameObject new_Projectile = Instantiate(Projectile);
        new_Projectile.transform.position = fire_Point.transform.position;
        new_Projectile.GetComponent<Rigidbody>().AddForce(fire_Point.transform.forward * 100, ForceMode.Impulse);
        muzzle_Flash.Play();
        StartCoroutine(FireRate());
    }
    void ADS(bool is_ADSing) // ADSing System:
    {
        float direction = is_ADSing ? -1 : 1;
        item_Holder.transform.position += direction * item_Holder.transform.right * 0.4f;
        item_Holder.transform.position -= direction * item_Holder.transform.up * 0.06f;
        obj_Data.walk_Speed *= is_ADSing ? 0.5f : 2;
        ui_Camera.fieldOfView = Mathf.Lerp(ui_Camera.fieldOfView, is_ADSing ? ADS_FOV : origin_FOV, 25 * Time.deltaTime);
    }
    
    // Action Systems:
    void MeleeAttack()
    {   
        switch(secondary_Data.collided_Entity.tag)
        {
            case "Dummy":
                DummyHandler dummy_Handler = secondary_Data.collided_Entity.GetComponent<DummyHandler>();
                dummy_Handler.StopAllCoroutines();
                dummy_Handler.StartCoroutine(dummy_Handler.Shake(0));
                break;

        }
    }

    void Throw_Grenade()
    {
        can_Attack = false;
        StartCoroutine(AttackCooldown(grenade_Rate));
        grenades--;
        GameObject gre = Instantiate(Grenade, fire_Point.transform.position, user_Camera.transform.rotation);
        Rigidbody GrenadeRB = gre.GetComponent<Rigidbody>();
        LinearJump(user_Camera.transform.forward, throw_force, gre);
    }

    IEnumerator FireRate()
    {
        yield return new WaitForSeconds(fire_Rate);
        can_Attack = true;
    }

    void getNewWeapon()
    {
        NWD = new_Weapon.GetComponent<Obj_State>();

        if (NWD.Primary_ID == 1)
            current_Primary = new_Weapon;
        else if (NWD.Secondary_ID == 2)
            current_Secondary = new_Weapon;
    }

    public IEnumerator AttackCooldown(float length)
    {
        yield return new WaitForSeconds(length);
        can_Attack = true;
    }
}