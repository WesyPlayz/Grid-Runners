using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHandler : MonoBehaviour
{
    //objects
    public GameObject secondary_Obj;
    private Rigidbody obj_Physics;
    private Transform obj_Transform;

    //script data
    private Obj_State obj_Data;
    private Obj_State secondary_Data;
    private DumbiesHandler dumby;

    private bool is_Jumping;

    // Jumping Variables:
    private bool can_Attack = true;

    // Collision Variables:
    public bool on_Floor;
    public bool on_Wall;

    private void Start()
    {
        obj_Data = GetComponent<Obj_State>();
        secondary_Data = secondary_Obj.GetComponent<Obj_State>();

        obj_Physics = GetComponent<Rigidbody>();
        obj_Transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (on_Floor)
        { 
            obj_Physics.AddRelativeForce(Vector3.down * 5);
            obj_Physics.velocity = new Vector3(Input.GetAxis("Horizontal") * obj_Data.Speed, obj_Physics.velocity.y, Input.GetAxis("Vertical") * obj_Data.Speed);
        }

        if (can_Attack) 
        {
            
            if (secondary_Data.collided_Entity != null && Input.GetAxis("Knife") != 0)
            {
                can_Attack = false;
                MeleeAttack();
                StartCoroutine(AttackCooldown(obj_Data.Attack_Cooldown));
            }
            
        }
    }


    
    void MeleeAttack()
    {
        /*
        if (secondary_Data.collided_Entity.name == "Enemy")
        {
            secondary_Data.collided_Entity.GetComponent<EnemyHandler>().damaged_Particle.GetComponent<ParticleSystem>().Play();
            //secondary_Data.collided_Entity.GetComponent<EnemyHandler>().damaged_Sound.Play(); //needs sxf
        }
        */

        if (secondary_Data.collided_Entity.name == "Dumbie")
        {
            dumby = secondary_Data.collided_Entity.GetComponent<DumbiesHandler>();
            dumby.Begin();
        }


    }
    

    /*
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(obj_Data.jump_Cooldown);
        obj_Data.jumps_Occurred++;
        is_Jumping = false;
    }
    */
    IEnumerator AttackCooldown(float length)
    {
        yield return new WaitForSeconds(length);
        can_Attack = true;
        Debug.Log("can attack again");
    }
}
