using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool on_Wall;

    private bool is_scoping = false;

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
            obj_Physics.AddRelativeForce(Vector3.down * obj_Data.gravity);
            obj_Physics.velocity = new Vector3(Input.GetAxis("Horizontal") * obj_Data.Speed, obj_Physics.velocity.y, Input.GetAxis("Vertical") * obj_Data.Speed);
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

}