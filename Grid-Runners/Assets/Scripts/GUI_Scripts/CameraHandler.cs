using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    // user Variables:
    private GameObject User;
    private UserHandler user_Handler;

    public GameObject Body;
    public GameObject Head;
    public GameObject Neck;

    // User Rotation Variables:
    private Quaternion rotation;

    private float orbitRotationX;
    private float orbitRotationY;
    private float rotationX_Spec;
    private float rotationY_Spec;

    private float distance;

    [Range(100, 1000)]
    public float Sensitivity;

    // Variable Initialization System:
    void Start()
    {
        User = GameObject.Find("User");
        user_Handler = User.GetComponent<UserHandler>();

        distance = Vector3.Distance(Head.transform.position, Neck.transform.position);
    }

    // Look Direction System:
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab))
        {
            float lookHorizontal = Sensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
            float lookVertical = -Sensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

            if (user_Handler.Mode == 0 && (lookHorizontal != 0 || lookVertical != 0))
            {
                orbitRotationX += lookVertical;
                orbitRotationY += lookHorizontal;

                orbitRotationX = Mathf.Clamp(orbitRotationX + lookVertical, -150, -30);
                rotation = Quaternion.Euler(orbitRotationX, orbitRotationY, 0);
                Head.transform.position = Neck.transform.position + rotation * Vector3.forward * distance;
                Head.transform.rotation = rotation;
                Body.transform.rotation = Quaternion.Euler(0, orbitRotationY, 0);
            }
            else if (user_Handler.Mode == 1 && (lookHorizontal != 0 || lookVertical != 0))
            {
                rotationX_Spec += lookVertical;
                rotationY_Spec += lookHorizontal;

                gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.rotation = Quaternion.Euler(rotationX_Spec, rotationY_Spec, 0);
            }
        }
    }
}