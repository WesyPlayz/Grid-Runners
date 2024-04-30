using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Heal", menuName = "Abilities/Defensive/Heal")]
public class Heal : Defensive
{
    [Header("Healing Attributes")]
    public float heal_Value;

    public override void Use(UserHandler user_Handler)
    {
        user_Handler.Health = Mathf.Max(user_Handler.Health + heal_Value, user_Handler.max_Health);
    }
}
