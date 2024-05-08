using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector3 temp = context.ReadValue<Vector2>();

        temp.z = temp.y;
        temp.y = controller.velocity.y;

        controller.Move(temp);
    }

    public void Jump(InputAction.CallbackContext context)
    {

    }

    public void Shoot(InputAction.CallbackContext context)
    {

    }

    public void ADS(InputAction.CallbackContext context)
    {

    }

    public void Sprint(InputAction.CallbackContext context)
    {

    }

    public void Knife(InputAction.CallbackContext context)
    {

    }

    public void Grenade(InputAction.CallbackContext context)
    {

    }
}
