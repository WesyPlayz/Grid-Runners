using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(fileName = "New Speed Boost", menuName = "Abilities/Mobility/Speed Boost")]
public class Speed_Boost : Mobility
{
    public bool can_Use;

    public float speed_modifier;
    public float speed_Boost_length;
    public float ability_Cooldown;
    public override void Use(UserHandler user_Handler)
    {
        if (can_Use)
        {
            can_Use = false;
            user_Handler.walk_Speed *= speed_modifier;
            ability_Timer(user_Handler, speed_Boost_length);
        }
    }

    public IEnumerator ability_Timer(UserHandler user_Handler, float time)
    {
        yield return new WaitForSeconds(time);
        user_Handler.walk_Speed /= speed_modifier;
        ability_Cooldown_Timer(ability_Cooldown);
    }

    public IEnumerator ability_Cooldown_Timer(float time)
    {
        yield return new WaitForSeconds(time);
        can_Use = true;
    }
}
