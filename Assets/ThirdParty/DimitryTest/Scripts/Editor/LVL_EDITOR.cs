using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

[CustomEditor(typeof(PlayerLevel))]
public class LVL_EDITOR : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerLevel plvl = (PlayerLevel)target;

        if (GUILayout.Button("ADD 15 exp"))
        {
            plvl.AddExperience(15);
        }

        if (GUILayout.Button("ADD 100 exp"))
        {
            plvl.AddExperience(100);
        }

        if (GUILayout.Button("ADD 300 exp"))
        {
            plvl.AddExperience(300);
        }
    }
}
