using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(fileName = "New Jammer", menuName = "Abilities/Utility/Jammer")]
public class Jammer : Utility
{
    public float distance;
    public float jamming_Time;
    public bool can_Use;
    public float cooldown;

    public override void Use(UserHandler user_Handler)
    {
        if (can_Use)
        {
            ability_Timer(user_Handler, jamming_Time);
            can_Use = false;
        }
    }

    public IEnumerator ability_Timer(UserHandler user_Handler, float time)
    {
        yield return new WaitForSeconds(time);

        ability_Cooldown_Timer(Cooldown);
    }

    public IEnumerator ability_Cooldown_Timer(float time)
    {
        yield return new WaitForSeconds(time);
        can_Use = true;
    }
}
