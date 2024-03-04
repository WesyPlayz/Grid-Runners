using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    // user Variables:
    private GameObject Player;
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
        Player = GameObject.Find("Player_1");
        user_Handler = Player.GetComponent<UserHandler>();

        distance = Vector3.Distance(Head.transform.position, Neck.transform.position);
    }

    // Look Direction System:
    void Update()
    {
        if (!Input.GetKey(KeyCode.Tab))
        {
            float lookHorizontal = Sensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
            float lookVertical = -Sensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

            if ((lookHorizontal != 0 || lookVertical != 0))
            {
                switch(user_Handler.Mode)
                {
                    case 0:
                        orbitRotationX += lookVertical;
                        orbitRotationY += lookHorizontal;
                        orbitRotationX = Mathf.Clamp(orbitRotationX + lookVertical, -170, -15);
                        rotation = Quaternion.Euler(orbitRotationX, orbitRotationY, 0);
                        Head.transform.position = Neck.transform.position + rotation * Vector3.forward * distance;
                        Head.transform.rotation = rotation;
                        Body.transform.rotation = Quaternion.Euler(0, orbitRotationY, 0);
                        break;
                    case 1:
                        rotationX_Spec += lookVertical;
                        rotationY_Spec += lookHorizontal;
                        user_Handler.user_Spectate.transform.rotation = Quaternion.Euler(rotationX_Spec, rotationY_Spec, 0);
                        break;
                }
            }
        }
    }
}