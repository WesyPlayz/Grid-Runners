using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    static class Collisions
    {
        // Find Collided Surface:
        public static bool FindSurfaceType(string surface_Type, Collision sender, GameObject retriever = null) // ID : 01 // Parameters are as follows (type of surface to look for) (object causing the collision) (object being affected)
        {
            if (surface_Type == "Wall" || surface_Type == "Floor" || surface_Type == "Ceiling")
            {
                Obj_State obj_Data = null;
                if (retriever != null)
                    obj_Data = retriever.GetComponent<Obj_State>();
                foreach (ContactPoint contact in sender.contacts)
                {
                    GameObject current_obj = sender.gameObject;
                    if (surface_Type == "Wall" && current_obj.CompareTag("Wall") && Vector3.Dot(contact.normal, Vector3.up) < 0.7071f && Vector3.Dot(contact.normal, Vector3.down) < 0.7071f)
                    {
                        if (obj_Data != null)
                            obj_Data.last_Wall_Contact = contact.normal;
                        return true;
                    }
                    else if (surface_Type == "Floor" && current_obj.CompareTag("Floor") && Vector3.Dot(contact.normal, Vector3.up) > 0.7071f)
                    {
                        if (obj_Data != null)
                            obj_Data.last_Floor_Contact = contact.normal;
                        return true;
                    }
                    else if (surface_Type == "Ceiling" && current_obj.CompareTag("Ceiling") && Vector3.Dot(contact.normal, Vector3.down) > 0.7071f)
                    {
                        if (obj_Data != null)
                            obj_Data.last_Ceiling_Contact = contact.normal;
                        return true;
                    }
                }
            }
            else
                Debug.LogWarning("[Utilities.Collision - ID : 01] surface_Type parameter does not contain a definition for: " + surface_Type);
            return false;
        }
    }
    static class Generic
    {
        // Jump Systems:
        public static bool nonLinearJump(bool on_Floor, float jump_Force, GameObject retriever) // ID : 01 // Parameters are as follows (current surface collision for direction calculation) (force applied) (object being affected)
        {
            Obj_State obj_Data = retriever.GetComponent<Obj_State>();
            Rigidbody obj_Physics = retriever.GetComponent<Rigidbody>();
            Vector3 jump_Direction =
                on_Floor ? obj_Data.last_Floor_Contact :
                obj_Data.last_Floor_Contact != Vector3.zero ? obj_Data.last_Floor_Contact :
                Vector3.up;
            obj_Physics.AddForce(jump_Direction * jump_Force, ForceMode.Force);
            return true;
        }
        public static bool LinearJump(Vector3 jump_Direction, float jump_Force, GameObject retriever) // ID : 02 // Parameters are as follows (direction of movement) (force applied) (object being affected)
        {
            Rigidbody obj_Physics = retriever.GetComponent<Rigidbody>();
            obj_Physics.AddForce(jump_Direction.normalized * jump_Force, ForceMode.Impulse);
            return true;
        }
    }
}