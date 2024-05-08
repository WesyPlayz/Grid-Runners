using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_Data : MonoBehaviour
{
    [Header("Grid Object Collection")]
    public List<GameObject> grid_Col;

    public int current_Gird_Obj;

    [Header("Grid Variables")]
    public float placement_Range;

    public void PlaceObject(UserHandler user_Handler, GameObject placement_Zone, Vector3 target_Normal) // Grid Placement System:
    {
        Bounds placement_Bounds = new Bounds(placement_Zone.transform.position + Vector3.Scale(target_Normal, placement_Zone.transform.localScale), placement_Zone.transform.localScale);
        if (!placement_Bounds.Intersects(user_Handler.user_Spectate.GetComponent<Collider>().bounds) && /*!placement_Bounds.Intersects(user_Handler.body_Hitbox_Bounds) &&*/ Vector3.Distance(placement_Zone.transform.position, user_Handler.user_Spectate.transform.position) <= placement_Range)
        {
            GameObject new_Grid_Obj = Instantiate(grid_Col[current_Gird_Obj]);
            new_Grid_Obj.transform.position = placement_Bounds.center;
        }
    }
}