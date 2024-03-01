using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Utilities.Collisions;
using static Utilities.Generic;

public class UserHandler : MonoBehaviour
{
    //objects
    public GameObject secondary_Obj;
    private Rigidbody obj_Physics;
    private Transform obj_Transform;
    public GameObject current_Primary, current_Secondary, current_Knife;
    public GameObject new_Weapon;
    public Camera fpsCam;
    public LayerMask enemyTeam;
    public string enemyTeamTag;

    //script data
    private Obj_State obj_Data;
    private Obj_State NWD; //new weapon data
    private Obj_State secondary_Data;
    private DummyHandler dumby;
    private GameManager gm;

    private bool is_Jumping;

    // Jumping Variables:
    public bool can_Attack = true;

    // Collision Variables:
    public bool on_Floor;

    private bool is_scoping;

    private void Start()
    {
        obj_Data = GetComponent<Obj_State>();
        secondary_Data = secondary_Obj.GetComponent<Obj_State>();
        gm  = GetComponent<GameManager>();

        obj_Physics = GetComponent<Rigidbody>();
        obj_Transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (on_Floor) //movement system
        {
            float current_Speed_V = Input.GetAxis("Vertical") * obj_Data.Speed;
            float current_Speed_H = Input.GetAxis("Horizontal") * obj_Data.Speed;

            Vector3 forwardMovement = transform.forward * current_Speed_V;
            Vector3 rightMovement = transform.right * current_Speed_H;

            obj_Physics.velocity = forwardMovement + rightMovement;

            if (Input.GetKeyDown(KeyCode.Space))
                nonLinearJump(on_Floor, obj_Data.jump_Force, gameObject);
        }

        if (can_Attack) //attack system
        {
            
            if (secondary_Data.collided_Entity != null && Input.GetAxis("Knife") != 0)
            {
                can_Attack = false;
                MeleeAttack();
                StartCoroutine(AttackCooldown(obj_Data.Attack_Cooldown));
            }
            
        }

        if (Input.GetAxis("Scope") >= .25f && !is_scoping || Input.GetButton("Scope") && !is_scoping)
        {
            is_scoping = true;
            obj_Data.Speed *= .75f;
        }
        else if (Input.GetAxis("Scope") <= .25f && is_scoping && !Input.GetButton("Scope"))
        {
            is_scoping = false;
            obj_Data.Speed /= .75f;
        }
    }

    public void hit(int dmg)
    {
        obj_Data.Health -= (dmg);
        if (obj_Data.Health <= 0)
            died();
    }
    
    void MeleeAttack()
    {    
        if (secondary_Data.collided_Entity.name == "Body")
        {
            dumby = secondary_Data.collided_Entity.GetComponent<DummyHandler>();
            dumby.StopAllCoroutines();
            StartCoroutine(dumby.Shake(0));
        }


    }

    void died()
    {
        gm.players_Alive--; //determines if the round ends or not
        if (gm.players_Alive == 1)
        {
            lost();
            gm.EndRound();
        }
    }

    void lost()
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
    
    // Collision System:
    void OnCollisionStay(Collision sender)
    {
        GameObject current_Obj = sender.gameObject;
        obj_Data.total_Collisions++;
        if (current_Obj.layer == LayerMask.NameToLayer("Terrain") && current_Obj != obj_Data.collided_Floor) // Terrain Collision:
        {
            bool foundFloor = FindSurfaceType("Floor", sender, gameObject);
            if (foundFloor)
            {
                on_Floor = true;
                obj_Data.collided_Floor = current_Obj;
            }
        }
    }
    void OnCollisionExit(Collision sender)
    {
        if (sender.gameObject == obj_Data.collided_Floor)
            obj_Data.collided_Floor = null;

        on_Floor = obj_Data.collided_Floor == null ? false : true;
    }
}