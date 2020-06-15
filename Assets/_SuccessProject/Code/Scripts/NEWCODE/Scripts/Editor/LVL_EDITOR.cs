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

        GUILayout.Label("Each level adds _add exp per lvl_ to _experience to the next lvl_.");

        GUILayout.Label("if(level % lvlDifficultyDenominator == 0)  addExpPerLvl *= difficultyMultiplier ");

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
