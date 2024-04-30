using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(fileName = "New Berserk", menuName = "Abilities/Offensive/Berserk")]
public class Berserk : Offensive
{
    public bool can_Use;

    public float damage_Modifier;
    public float fire_Rate_Modifier;
    public float speed_Modifer;
    public float ability_Time;
    public float ability_Cooldown;
    public override void Use(UserHandler user_Handler)
    {
        ability_Timer(user_Handler, ability_Time);
    }

    public IEnumerator ability_Timer(UserHandler user_Handler, float time)
    {
        yield return new WaitForSeconds(time);
        user_Handler.walk_Speed /= speed_Modifer;
        ability_Cooldown_Timer(ability_Cooldown);
    }

    public IEnumerator ability_Cooldown_Timer(float time)
    {
        yield return new WaitForSeconds(time);
        can_Use = true;
    }
}
