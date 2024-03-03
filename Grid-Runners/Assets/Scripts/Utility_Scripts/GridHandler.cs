using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    // User Variables:
    private GameObject Player;
    private GameObject User;
    private GameObject Spectator;

    private UserHandler user_handler;

    // Grid Variables:
    private GameObject Grid;
    private Grid_Data grid_Data;

    // Object Variables:
    private GameObject grid_Tile;

    public GameObject placement_Zone;
    private Bounds placement_Bounds;

    public Material run_Material;

    // Grid Data Variables:
    private bool is_Selected;
    private bool can_Select = true;

    // Variable Initialization System:
    void Start()
    {
        // Initiate User ID:
        Player = GameObject.Find("Player_1");
        user_handler = Player.GetComponent<UserHandler>();

        User = user_handler.User;
        Spectator = user_handler.user_Spectate;

        // Initiate Grid ID:
        Grid = GameObject.Find("Grid");
        grid_Data = Grid.GetComponent<Grid_Data>();

        // Initiate Grid State:
        grid_Tile = gameObject.transform.parent.gameObject;
        placement_Bounds = placement_Zone.GetComponent<Renderer>().bounds;

        // Disable Detection Visibility:
        GetComponent<MeshRenderer>().material = run_Material;
    }

    // Mouse Detection System:
    private void OnMouseEnter()
    {
        is_Selected = true;
    }
    private void OnMouseExit()
    {
        is_Selected = false;
    }

    // Grid Placement System:
    private void Update()
    {
        if (can_Select && Input.GetMouseButtonDown(0) && is_Selected)
        {
            float distance = user_handler.Mode == 0 ? Vector3.Distance(transform.position, User.transform.position) : Vector3.Distance(transform.position, Spectator.transform.position);
            if (!placement_Bounds.Intersects((user_handler.Mode == 0 ? User : Spectator).GetComponent<Collider>().bounds) && distance <= 10)
            {
                can_Select = false;
                GameObject new_Grid_Obj = Instantiate(grid_Data.grid_Col[grid_Data.current_Gird_Obj]);
                new_Grid_Obj.transform.position = placement_Zone.transform.position;
            }
        }
    }
}