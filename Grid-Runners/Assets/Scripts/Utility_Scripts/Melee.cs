using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Melee", menuName = "Items/Melee")]
public class Melee : Item
{
    public GameObject melee_prefab;

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

        }

        item_Data.StartCoroutine(item_Data.Attack_Rate(user_Handler, fire_Rate));
    }

    public override void Aim(UserHandler user_Handler, bool is_ADSing)
    {

    }

    public override void Action(UserHandler user_Handler)
    {

    }
}