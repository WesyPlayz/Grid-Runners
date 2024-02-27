using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHandler : MonoBehaviour
{
    public GameObject secondary_Obj;

    private Rigidbody obj_Physics;
    private Transform obj_Transform;

    private bool is_Jumping;

    // Jumping Variables:
    private bool can_Attack = true;

    // Collision Variables:
    public bool on_Floor;
    public bool on_Wall;

    private void Start()
    {
        obj_Physics = GetComponent<Rigidbody>();
        obj_Transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (on_Floor)
        { 
            obj_Physics.AddRelativeForce(Vector3.down * 5);
            obj_Physics.velocity = new Vector3(Input.GetAxis("Horizontal") * 5, obj_Physics.velocity.y, Input.GetAxis("Vertical") * 5);
        }

        if (can_Attack) 
        {
            /*
            if (secondary_Data.collided_Entity != null && Input.GetAxis("RT") != 0)
            {
                Debug.Log("trigger pulled");
                can_Attack = false;
                MeleeAttack();
                StartCoroutine(AttackCooldown(obj_Data.melee_Cooldown));
            }
            */
        }
    }


    /*
    void MeleeAttack()
    {
        if (secondary_Data.collided_Entity.name == "Enemy")
        {
            secondary_Data.collided_Entity.GetComponent<EnemyHandler>().damaged_Particle.GetComponent<ParticleSystem>().Play();
            //secondary_Data.collided_Entity.GetComponent<EnemyHandler>().damaged_Sound.Play(); //needs sxf
        }

        if (secondary_Data.collided_Entity.name == "Dumbie")
        {
            dumby = secondary_Data.collided_Entity.GetComponent<DumbiesHandler>();
            dumby.Begin();
        }

        Obj_State entity_Data = secondary_Data.collided_Entity.GetComponent<Obj_State>();
        entity_Data.Health = Mathf.Max(entity_Data.Health - obj_Data.Damage, 0); // subtracts the damage amount to health, and doesn't allow health to go below 0 if it does.

        obj_Data.Energy = Mathf.Min(obj_Data.Energy +
            (entity_Data.Health <= 0 ? Random.Range(obj_Data.mix_Energy_Increase * 5, obj_Data.max_Energy_Increase * 2) :
            Random.Range(obj_Data.mix_Energy_Increase, obj_Data.max_Energy_Increase)), obj_Data.max_Energy);
    }
    */

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
