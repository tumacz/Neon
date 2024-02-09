using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;
using Unity.AI.Navigation;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;
        NavMeshSurface nav = map.GetComponent<NavMeshSurface>();

        if (DrawDefaultInspector())
        {
            map.GenerateMap();
            if (nav != null)
            {
                nav.BuildNavMesh();
            }
        }
        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
        if (GUILayout.Button("Build NavMesh"))
        {
            if (nav != null)
            {
                Debug.LogWarning("Kliknij w wygenerowaniej mapie przed zapisaniem");
            }
        }
    }
}