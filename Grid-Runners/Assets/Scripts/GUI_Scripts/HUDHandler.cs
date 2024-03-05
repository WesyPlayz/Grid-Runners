using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    public GameObject Player;
    public GameObject User;
    private UserHandler user_Handler;

    public Image Health_Bar;
    public Image ammo_Counter;

    void Start()
    {
        user_Handler = Player.GetComponent<UserHandler>();

        User = user_Handler.User;
    }
    void Update()
    {
        Health_Bar.fillAmount = user_Handler.Health / user_Handler.max_Health;
        ammo_Counter.fillAmount = (float)user_Handler.Ammo / user_Handler.max_Ammo;
    }
}