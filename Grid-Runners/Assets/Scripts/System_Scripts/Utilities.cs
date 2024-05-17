using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class Debug_System
    {
        // Script Emergency System :
        private static string ID_01 = "ID : Deb_01";
        /// <summary>
        /// [ ID : Deb_01 ]
        /// Params : (script, error)
        /// </summary>
        public static void Shutdown(MonoBehaviour script, string error)
        {
            Debug.LogError(error);
            script.enabled = false;
        }
    }

    public static class Methods
    {
        // Method Identifier System :
        public static Dictionary<string, string> methods = new Dictionary<string, string>
        {
            {
                Cursor_Handler.ID_01, // Start :
                "Initializes Values : (parameters)\n" +
                "Modified Values : (parameters, game_Manager, player_Obj, user_Handler, uni_User_Handler, Cursor.lockState, cursor_Pos)\n" +
                "Return Type : (void)"
            },
            {
                Cursor_Handler.ID_02, // Update :
                "Initializes Values : (cursor_Horizontal, cursor_Vertical)\n" +
                "Modifies Values : (cursor_Horizontal, cursor_Vertical, cursor_Pos.anchoredPosition)\n" +
                "Return Type : (void)"
            },
            {
                Cursor_Handler.ID_03, // Move_Cursor :
                "Initializes Values : (anchored_Cursor_Pos)\n" +
                "Modifies Values : (anchored_Cursor_Pos, canvas_RectTransform, canvas_Size)\n" +
                "Return Type : (Vector2)"
            },
            {
                Cursor_Handler.ID_04, // Select :
                "Initializes Values : (pointerData, screenPosition, ui_Elements, ui_Element, button)\n" +
                "Modifies Values : (pointerData.position)\n" +
                "Return Type : (void)" 
            },
        };
        public static string ID_01 = "ID : Met_01";
        /// <summary>
        /// [ ID : Met_01 ]
        /// Params : (ID)
        /// </summary>
        public static void Find(string ID)
        {
            Debug.Log(methods[ID]);
        }
    }
}