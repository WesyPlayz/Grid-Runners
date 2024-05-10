using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Utilities.Collisions;

public class Hitboxhandler : MonoBehaviour
{
    public GameObject Player;

    public UserHandler user_Handler;
    private Obj_State obj_Data;

    [Range(0, 2)]
    public int dmg_Type;

    void Start()
    {
        user_Handler = Player.GetComponent<UserHandler>();
        obj_Data = Player.GetComponent<Obj_State>();
    }

    // Collision System:
    void OnCollisionStay(Collision sender)
    {
        GameObject current_Obj = sender.gameObject;
        if (current_Obj.CompareTag("Kill"))
            user_Handler.Damage(user_Handler.max_Health);

        if (current_Obj.layer == LayerMask.NameToLayer("Projectile")) // Projectile Collision:
        {
            user_Handler.Damage(current_Obj.GetComponent<ProjectileHandler>().dmg);
        }
        print("hurt1");
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject current_Obj = collision.gameObject;
        if (current_Obj.CompareTag("Kill"))
            user_Handler.Damage(user_Handler.max_Health);

        if (current_Obj.tag == ("Projectile")) // Projectile Collision:
        {
            user_Handler.Damage(current_Obj.GetComponent<ProjectileHandler>().dmg);
            print(user_Handler.Health);
        }
        print("hurt2");
    }
}