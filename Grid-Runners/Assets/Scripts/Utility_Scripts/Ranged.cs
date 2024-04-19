using UnityEngine;
using UnityEditor;

// Ranged Item System:
[CreateAssetMenu(fileName = "New Ranged", menuName = "Items/Ranged")]
public class Ranged : Item
{
    // Ranged GUI Data:
    public Sprite Ammo;

    [Header("Ranged Attributes")]
    public bool is_Hit_Scan;
    public bool is_Spread;
    public bool is_Auto;

    [Header("Projectile Attributes")]
    public GameObject projectile_Prefab;

    public int max_Ammo;

    [Range(1, 100)] public float bullet_Speed;
    [Range(0.025f, 5)] public float reload_Speed;
    [Range(0.025f, 5)] public float fire_Rate;

    [Header("Action Attributes")]
    [Range(1, 100)] public float ADS_FOV;
    [Range(1, 100)] public float ADS_Speed;

    public override void Attack(UserHandler user_Handler)
    {
        user_Handler.Ammo--;
        if (is_Hit_Scan)
        {
            RaycastHit target;
            if (Physics.Raycast(user_Handler.fire_Point.transform.position, user_Handler.fire_Point.transform.forward, out target, Mathf.Infinity, item_Data.entity_Mask))
            {
                GameObject selected_Target = target.transform.gameObject;
                if (selected_Target.CompareTag("User"))
                {
                    UserHandler target_Handler = selected_Target.GetComponent<UserHandler>();
                    target_Handler.Damage(Damage);

                    if (target_Handler.Health <= 0)
                        user_Handler.points += user_Handler.points_Per_Kill;
                }
                else if (selected_Target.CompareTag("Dummy"))
                {
                    DummyHandler target_Handler = selected_Target.GetComponent<DummyHandler>();
                    target_Handler.StopAllCoroutines();
                    target_Handler.StartCoroutine(target_Handler.Shake(0));
                }
            }
        }
        GameObject new_Projectile = Get_Projectile(user_Handler);

        new_Projectile.transform.SetPositionAndRotation(user_Handler.fire_Point.transform.position, user_Handler.fire_Point.transform.rotation);
        new_Projectile.GetComponent<Rigidbody>().AddForce(user_Handler.fire_Point.transform.forward * bullet_Speed, ForceMode.Impulse);

        user_Handler.muzzle_Flash.Play();

        if (!is_Auto)
            item_Data.StartCoroutine(item_Data.Attack_Rate(user_Handler, fire_Rate));
    }

    public GameObject Get_Projectile(UserHandler user_Handler)
    {
        foreach (Transform projectile in user_Handler.Projectiles.transform)
        {
            if (!projectile.gameObject.activeInHierarchy && projectile.gameObject.name == projectile_Prefab.name)
            {
                projectile.gameObject.SetActive(true);
                projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
                return projectile.gameObject;
            }
        }
        GameObject new_Projectile = Instantiate(projectile_Prefab, user_Handler.Projectiles.transform);
        new_Projectile.name = projectile_Prefab.name;
        return new_Projectile;
    }

    public void Reload(UserHandler user_Handler)
    {
        item_Data.StopCoroutine("Fire_Rate");
        item_Data.StartCoroutine(item_Data.Reload_Cooldown(user_Handler, max_Ammo, reload_Speed));
    }

    public override void Aim(UserHandler user_Handler, bool is_ADSing)
    {
        float direction = is_ADSing ? -1 : 1;
        user_Handler.item_Holder.transform.position += direction * user_Handler.item_Holder.transform.right * 0.4f;
        user_Handler.item_Holder.transform.position -= direction * user_Handler.item_Holder.transform.up * 0.06f;

        user_Handler.obj_Data.walk_Speed *= is_ADSing ? item_Data.min_Speed_Mod : item_Data.max_Speed_Mod;
        user_Handler.camera_Handler.ui_Camera.fieldOfView = Mathf.Lerp(user_Handler.camera_Handler.ui_Camera.fieldOfView, is_ADSing ? ADS_FOV : user_Handler.camera_Handler.origin_FOV, ADS_Speed * Time.deltaTime);
    }

    public void Peek(UserHandler user_Handler, float angle, int side, bool enabled)
    {
        user_Handler.Neck.transform.Rotate(Vector3.forward, (enabled ? -angle : angle), Space.Self);
        if (!enabled)
            user_Handler.Neck.transform.localRotation = Quaternion.Euler(user_Handler.Neck.transform.localEulerAngles.x, 0, 0);
    }

    public override void Action(UserHandler user_Handler)
    {

    }
}