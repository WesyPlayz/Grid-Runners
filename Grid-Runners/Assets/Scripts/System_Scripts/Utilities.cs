using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    static class Collisions
    {

    }
    static class Generic
    {
        // Jump Systems:
        public static bool LinearJump(Vector3 jump_Direction, float jump_Force, GameObject retriever) // ID : 02 // Parameters are as follows (direction of movement) (force applied) (object being affected)
        {
            CharacterController obj_Physics = retriever.GetComponent<CharacterController>();
            obj_Physics.SimpleMove(jump_Direction.normalized * jump_Force);
            return true;
        }
    }
}