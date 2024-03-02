using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    // User Variables:
    private GameObject User;

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

    // Variable Initialization System:
    void Start()
    {
        // Initiate User ID:
        User = GameObject.Find("User");

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
        if (Input.GetMouseButtonDown(0) && is_Selected)
        {
            if (!placement_Bounds.Intersects(User.GetComponent<Collider>().bounds) && Vector3.Distance(transform.position, User.transform.position) <= 10)
            {
                GameObject new_Grid_Obj = Instantiate(grid_Data.grid_Col[grid_Data.current_Gird_Obj]);
                new_Grid_Obj.transform.position = placement_Zone.transform.position;
            }
        }
    }
}