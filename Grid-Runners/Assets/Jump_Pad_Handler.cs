using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_Pad_Handler : MonoBehaviour
{
    public Jump_Pad_Handler jump_Pad_Handler;

    private bool Disabled;

    [Range(0, 100)]
    public float disable_Time;

    void OnCollisionEnter(Collision sender)
    {
        if (!(Disabled && jump_Pad_Handler.Disabled))
        {
            GameObject current_Obj = sender.gameObject;
            if (gameObject.name == "Pad_HitBox" && current_Obj.CompareTag("Player"))
            {
                UserHandler user_Handler = current_Obj.GetComponent<UserHandler>();
                print("Boosted");
            }
            else if (gameObject.name == "Base" && current_Obj.CompareTag("Projectile"))
            {
                StartCoroutine(Disable(Disabled == jump_Pad_Handler.Disabled == true));
            }
        }
    }
    private IEnumerator Disable(bool state)
    {
        print("Disabled");
        yield return new WaitForSeconds(disable_Time);
        Disabled = jump_Pad_Handler.Disabled == false;
        print("Enabled");
    }
}
