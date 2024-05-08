using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(fileName = "New Teleportation", menuName = "Abilities/Mobility/Teleportation")]
public class Teleportation : Mobility
{
    public float distance;
    public float cooldown;
    public bool can_Use;

    public override void Use(UserHandler user_Handler)
    {
        if (can_Use)
        {
            user_Handler.User.transform.position += Vector3.forward * distance;
        }
    }

    public IEnumerator ability_Cooldown_Timer(float time)
    {
        yield return new WaitForSeconds(time);
        can_Use = true;
    }
}