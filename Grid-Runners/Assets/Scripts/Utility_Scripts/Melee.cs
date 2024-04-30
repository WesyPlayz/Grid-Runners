using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Melee", menuName = "Items/Melee")]
public class Melee : Item
{
    public GameObject melee_prefab;

    public bool is_Charging;
    public float charging_Time;
    public int min_dmg;
    public int max_dmg;
    public float range;
    public float fire_Rate;

    public override void Attack(UserHandler user_Handler)
    {
        switch (user_Handler.secondary_Data.collided_Entity.tag)
        {
            case "Dummy":
                DummyHandler dummy_Handler = user_Handler.secondary_Data.collided_Entity.GetComponent<DummyHandler>();
                dummy_Handler.StopAllCoroutines();
                dummy_Handler.StartCoroutine(dummy_Handler.Shake(0f));
                break;
            case "User":
                UserHandler target_Player = user_Handler.secondary_Data.collided_Entity.GetComponent<UserHandler>();
                Damage = Mathf.FloorToInt(Mathf.Lerp(max_dmg, min_dmg, charging_Time));
                if (!target_Player.is_Blocking)
                    target_Player.Damage(Damage);
                break;

        }

        item_Data.StartCoroutine(item_Data.Attack_Rate(user_Handler, fire_Rate));
        charging_Time = 0f;
    }

    public override void Aim(UserHandler user_Handler, bool is_Blocking)
    {
        user_Handler.is_Blocking = is_Blocking;
    }

    public override void Action(UserHandler user_Handler)
    {
        charging_Time = Time.deltaTime;
    }
}