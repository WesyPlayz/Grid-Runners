using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    // user Variables:
    public GameObject Body;
    public GameObject Head;
    public GameObject Neck;

    // User Rotation Variables:
    private Quaternion rotation;

    private float lastHorizontal;
    private float lastVertical;
    private float orbitRotationX;
    private float orbitRotationY;

    private float distance;

    [Range(100, 1000)]
    public float Sensitivity;

    void Start()
    {
        distance = Vector3.Distance(Head.transform.position, Neck.transform.position);
    }
    void Update()
    {
        float lookHorizontal = Sensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        float lookVertical = -Sensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

        if (lookHorizontal != lastHorizontal || lookVertical != lastVertical)
        {
            lastHorizontal = lookHorizontal;
            lastVertical = lookVertical;

            orbitRotationX += lookVertical;
            orbitRotationY += lookHorizontal;
            orbitRotationX = Mathf.Clamp(orbitRotationX + lookVertical, -150, -30);
            rotation = Quaternion.Euler(orbitRotationX, orbitRotationY, 0);
            Head.transform.position = Neck.transform.position + rotation * Vector3.forward * distance;
            Head.transform.rotation = rotation;
            Body.transform.rotation = Quaternion.Euler(0, orbitRotationY, 0);
        }
    }
}
