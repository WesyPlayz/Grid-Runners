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

    public float Health;

    [Range(0, 1)]
    public int Mode;

    [Header("Camera Variables")]
    public Camera user_Camera, ui_Camera;
    public GameObject Camera_Pos0, Camera_Pos1;

    private float origin_FOV;

    // Movement Variables:
    private bool is_Sprinting;

    [Header("Weapon Variables")]
    public GameObject item_Holder;

    public GameObject fire_Point;
    public GameObject Projectile;

    public ParticleSystem muzzle_Flash;

    public bool can_Attack = true;
    public bool is_Scoped;

    public float fire_Rate;

    public int max_Ammo;
    public int Ammo;

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

    private bool is_scoping;

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
        // Camera Systems:
        if (Input.GetKeyDown(KeyCode.Tab)) // Will be changed.
            SwapMode();

        bool mode_State = Mode == 0; // Mode Check.

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
        current_Physics.velocity = new Vector3(clamped_Velocity.x, (mode_State ? user_Physics.velocity.y : clamped_Velocity.y), clamped_Velocity.z);

        // Jump System:
        if (mode_State && on_Floor && Input.GetKeyDown(KeyCode.Space))
            nonLinearJump(on_Floor, obj_Data.jump_Force, gameObject, User);

        // Attack Systems:
        if (Mode == 0)
        {
            if (can_Attack && Input.GetKeyDown(KeyCode.Mouse0) && Ammo > 0) // Ranged System:
            {
                can_Attack = false;
                Range();
            }
            else if (Input.GetKeyDown(KeyCode.R) && Ammo != max_Ammo)
                Ammo = max_Ammo;
            if (Input.GetKeyDown(KeyCode.Mouse1)) // Zoom System:
            {
                is_Scoped = true;
                item_Holder.transform.position -= item_Holder.transform.right * 0.4f;
                item_Holder.transform.position += item_Holder.transform.up * 0.06f;
                ui_Camera.fieldOfView = Mathf.Lerp(ui_Camera.fieldOfView, 40, 25 * Time.deltaTime);
                obj_Data.walk_Speed *= 0.75f;
            }
            else if(Input.GetKeyUp(KeyCode.Mouse1))
            {
                is_Scoped = false;
                item_Holder.transform.position += item_Holder.transform.right * 0.4f;
                item_Holder.transform.position -= item_Holder.transform.up * 0.06f;
                ui_Camera.fieldOfView = Mathf.Lerp(ui_Camera.fieldOfView, origin_FOV, 25 * Time.deltaTime);
                obj_Data.walk_Speed /= .75f;
            }
        }
        else
        {
            if (is_Scoped)
            {
                is_Scoped = false;
                item_Holder.transform.position += item_Holder.transform.right * 0.4f;
                item_Holder.transform.position -= item_Holder.transform.up * 0.06f;
                ui_Camera.fieldOfView = Mathf.Lerp(ui_Camera.fieldOfView, origin_FOV, 25 * Time.deltaTime);
                obj_Data.walk_Speed /= .75f;
            }
        }

        if (can_Attack) //attack system
        {
            
            if (secondary_Data.collided_Entity != null && Input.GetAxis("Knife") != 0)
            {
                can_Attack = false;
                Melee();
                StartCoroutine(AttackCooldown(obj_Data.Attack_Cooldown));
            }
            
        }
    }

    // Mode Systems:
    public void SwapMode()
    {
        // Mode State ID:
        bool mode_State = Mode == 0;
        Transform cam_Pos = mode_State ? Camera_Pos0.transform : Camera_Pos1.transform;
        Mode = 1 - Mode;

        // Object Swapping System:
        User.SetActive(!mode_State);
        user_Spectate.SetActive(mode_State);

        // User Protection System: (Tutorial Only)
        if (mode_State)
            body_Hitbox_Bounds = body_Hitbox.bounds;

        // Camera Repositioning System:
        user_Camera.transform.SetPositionAndRotation(cam_Pos.position, cam_Pos.rotation);
        user_Camera.transform.parent = cam_Pos;
    }

    public void hit(int dmg)
    {
        obj_Data.Health -= (dmg);
        if (obj_Data.Health <= 0)
        {

        }
    }
    
    // Action Systems:
    void Melee()
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
    void Range()
    {
        Ammo--;
        GameObject new_Projectile = Instantiate(Projectile);
        new_Projectile.transform.position = fire_Point.transform.position;
        new_Projectile.GetComponent<Rigidbody>().AddForce(fire_Point.transform.forward * 100, ForceMode.Impulse);
        muzzle_Flash.Play();
        StartCoroutine(FireRate());
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
        Debug.Log("can attack again");
    }
}