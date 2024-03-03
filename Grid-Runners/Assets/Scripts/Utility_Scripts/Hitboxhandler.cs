using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Utilities.Collisions;

public class Hitboxhandler : MonoBehaviour
{
    public GameObject Player;

    private UserHandler user_Handler;
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
        if (current_Obj.layer == LayerMask.NameToLayer("Terrain")) // Terrain Collision:
        {
            bool found_Floor = FindSurfaceType("Floor", sender, Player);
            bool found_Wall = FindSurfaceType("Wall", sender, Player);
            if (found_Floor)
                user_Handler.on_Floor = true;
            else if (found_Wall)
                user_Handler.on_Wall = true;
        }
        else if (current_Obj.layer == LayerMask.NameToLayer("Projectile")) // Projectile Collision:
        {
            // put damage here
        }
    }
    void OnCollisionExit(Collision sender)
    {
        user_Handler.on_Floor = false;
        user_Handler.on_Wall = false;
    }
}