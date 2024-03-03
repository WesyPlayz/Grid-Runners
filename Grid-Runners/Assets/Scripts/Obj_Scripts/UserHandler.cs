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
    [Range(0, 1)]
    public int Mode;

    [Header("Camera Variables")]
    public Camera user_Camera;
    public GameObject Camera_Pos0, Camera_Pos1;

    // Movement Variables:
    private bool is_Sprinting;

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
    public bool can_Attack = true;

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

        secondary_Data = secondary_Obj.GetComponent<Obj_State>();
        obj_Transform = GetComponent<Transform>();
    }

    private void Update()
    {
        // Camera System:
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Object Swapping System:
            User.SetActive(Mode == 0 ? false : true);
            user_Spectate.SetActive(Mode == 0 ? true : false);

            // User Protection System:
            if (Mode == 0)
                body_Hitbox_Bounds = body_Hitbox.bounds;

            // Camera Mode System:
            user_Camera.transform.position = Mode == 0 ? Camera_Pos0.transform.position : Camera_Pos1.transform.position;
            user_Camera.transform.rotation = Mode == 0 ? Camera_Pos0.transform.rotation : Camera_Pos1.transform.rotation;
            user_Camera.transform.parent = Mode == 0 ? Camera_Pos0.transform : Camera_Pos1.transform;
            // Mode switch:
            Mode = 1 - Mode;
        }

        //Movement_System:
        Vector3 move_Direction = User.transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))); // Basic Move Direction Calculation.
        Vector3 spec_Move_Direction = user_Spectate.transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("ThirdAxis"), Input.GetAxis("Vertical"))); // Spectator Move Direction Calculation.
        if (Input.GetKey(KeyCode.LeftShift)) // Sprint System:
            is_Sprinting = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            is_Sprinting = false;
        if (Mode == 1) // Movement Mode System:
        {
            user_Spectate_Physics.AddForce(spec_Move_Direction * (is_Sprinting ? obj_Data.sprint_Speed : obj_Data.walk_Speed) * 10);
        }
        else if (Mode == 0)
        {
            user_Physics.AddForce(move_Direction * (is_Sprinting ? obj_Data.sprint_Speed : obj_Data.walk_Speed) * 10);
            if (on_Floor && Input.GetKeyDown(KeyCode.Space)) // Jump System:
                nonLinearJump(on_Floor, obj_Data.jump_Force, gameObject, User);
        }
        user_Spectate_Physics.velocity = Vector3.ClampMagnitude(user_Spectate_Physics.velocity, // Max Speed Calculation:
            (spec_Move_Direction == Vector3.zero ? 0 : 
            is_Sprinting ? obj_Data.sprint_Speed : 
            obj_Data.walk_Speed));
        Vector3 horizontalVelocity = new Vector3(user_Physics.velocity.x, 0, user_Physics.velocity.z);
        Vector3 clampedHorizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, 
            (move_Direction == Vector3.zero ? 0 : 
            is_Sprinting ? obj_Data.sprint_Speed : 
            obj_Data.walk_Speed));
        user_Physics.velocity = new Vector3(clampedHorizontalVelocity.x, user_Physics.velocity.y, clampedHorizontalVelocity.z);

        if (can_Attack) //attack system
        {
            
            if (secondary_Data.collided_Entity != null && Input.GetAxis("Knife") != 0)
            {
                can_Attack = false;
                Melee();
                StartCoroutine(AttackCooldown(obj_Data.Attack_Cooldown));
            }
            
        }

        if (Input.GetAxis("Scope") >= .25f && !is_scoping || Input.GetButton("Scope") && !is_scoping)
        {
            is_scoping = true;
            obj_Data.walk_Speed *= .75f;
        }
        else if (Input.GetAxis("Scope") <= .25f && is_scoping && !Input.GetButton("Scope"))
        {
            is_scoping = false;
            obj_Data.walk_Speed /= .75f;
        }
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