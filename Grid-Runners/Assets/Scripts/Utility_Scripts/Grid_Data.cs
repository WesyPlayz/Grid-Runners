using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        if (!placement_Bounds.Intersects(user_Handler.user_Spectate.GetComponent<Collider>().bounds) && /*!placement_Bounds.Intersects(user_Handler.body_Hitbox_Bounds) &&*/ Vector3.Distance(placement_Zone.transform.position, user_Handler.user_Spectate.transform.position) <= placement_Range && (user_Handler.player == 1 ? user_Handler.game_manager.P1_Points : user_Handler.game_manager.P1_Points) >= 1)
        {
            GameObject new_Grid_Obj = Instantiate(grid_Col[current_Gird_Obj]);
            new_Grid_Obj.transform.position = placement_Bounds.center;
            if (user_Handler.player == 1)
                user_Handler.game_manager.P1_Points -= 1;
            else if (user_Handler.player == 2)
                user_Handler.game_manager.P2_Points -= 1;
        }
    }
}