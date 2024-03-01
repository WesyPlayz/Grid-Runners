using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    // Object Variables:
    private GameObject grid_Tile;
    private GameObject User;

    // Grid Data Variables:
    private bool isSelected;

    public int Direction;

    void Start()
    {
        grid_Tile = gameObject.transform.parent.gameObject;
        User = GameObject.Find("Human_GR");
    }

    // Mouse Detection System:
    private void OnMouseEnter()
    {
        isSelected = true;
    }
    private void OnMouseExit()
    {
        isSelected = false;
    }

    // Grid Placement System:
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isSelected)
        {
            float distance = Vector3.Distance(transform.position, User.transform.position);
            if (distance <= 6 && distance > 3)
            {
                GameObject new_Grid_Obj = Instantiate(grid_Tile);
                switch (Direction)
                {
                    case 0:
                        new_Grid_Obj.transform.position = new Vector3(grid_Tile.transform.position.x, grid_Tile.transform.position.y + 2, grid_Tile.transform.position.z);
                        break;
                    case 1:
                        new_Grid_Obj.transform.position = new Vector3(grid_Tile.transform.position.x, grid_Tile.transform.position.y - 2, grid_Tile.transform.position.z);
                        break;
                    case 2:
                        new_Grid_Obj.transform.position = new Vector3(grid_Tile.transform.position.x + 2, grid_Tile.transform.position.y, grid_Tile.transform.position.z);
                        break;
                    case 3:
                        new_Grid_Obj.transform.position = new Vector3(grid_Tile.transform.position.x - 2, grid_Tile.transform.position.y, grid_Tile.transform.position.z);
                        break;
                    case 4:
                        new_Grid_Obj.transform.position = new Vector3(grid_Tile.transform.position.x, grid_Tile.transform.position.y, grid_Tile.transform.position.z + 2);
                        break;
                    case 5:
                        new_Grid_Obj.transform.position = new Vector3(grid_Tile.transform.position.x, grid_Tile.transform.position.y, grid_Tile.transform.position.z - 2);
                        break;
                }
            }
        }
    }
}