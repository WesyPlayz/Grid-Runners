using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionZone : MonoBehaviour
{
    // Object State Variables:
    public GameObject Entity;

    private Obj_State obj_Data;

    // Initiate State:
    void Start()
    {
        obj_Data = GetComponent<Obj_State>();
    }

    // Detection System:
    void Update()
    {
        obj_Data.collided_Entity = null;

        // finds entities:
        Collider[] entities = Physics.OverlapSphere(transform.position, obj_Data.Reach / 2);
        //OverlapCircleAll(transform.position, obj_Data.Reach / 2);

        GameObject closest_Entity = null;
        float closest_Distance = Mathf.Infinity;

        foreach (Collider entity in entities)
        {
            GameObject entity_Obj = entity.gameObject;
            if (entity_Obj.layer == LayerMask.NameToLayer("Entity"))
            {
                float distance = Vector2.Distance(Entity.transform.position, entity_Obj.transform.position);
                if (distance < closest_Distance)
                {
                    closest_Entity = entity_Obj;
                    closest_Distance = distance;
                }
                obj_Data.collided_Entity = closest_Entity;
            }
        }
    }
}