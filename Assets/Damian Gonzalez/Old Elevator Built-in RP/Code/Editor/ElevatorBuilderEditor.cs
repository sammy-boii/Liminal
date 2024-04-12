#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 *  All this code does is to provide the "Build" and "Clear" buttons in the inspector for the 
 *  Elevator Builder script. This is purely cosmetic, since you can achieve the same through
 *  the context menu of the script in the inspector (The same menu of "Remove Component"...)
 *  
 *  So, if you eventually get any warnings or errors when building (compiling) the game, 
 *  consider deleting this file althogheter or moving it temporally out of the \Editor folder
 */

namespace OldElevator {
    [CustomEditor(typeof(ElevatorBuilder))]
    public class ElevatorBuilderEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            ElevatorBuilder script = (ElevatorBuilder)target;
            GUILayout.Space(50);
            if (GUILayout.Button("Build Elevator", GUILayout.Width(200), GUILayout.Height(30))) {
                script.Build();
            }
            GUILayout.Space(10);

            if (GUILayout.Button("Clear all", GUILayout.Width(200), GUILayout.Height(30))) {
                script.ClearAll();
            }
            GUILayout.Space(10);

            if (GUILayout.Button("Detach", GUILayout.Width(200), GUILayout.Height(30))) {
                script.Detach();
            }
            GUILayout.Space(10);
        }

    }
}
#endif