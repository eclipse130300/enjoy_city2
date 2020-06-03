using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*[CustomEditor(typeof(SaveManager))]
public class Save_editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SaveManager saveManager = (SaveManager)target;

        if (GUILayout.Button("DELETE IMPORTANT DATA(EXP, LVL, MONEY)"))
        {
            saveManager.DeleteImportantDataConfig();
            saveManager.LoadAllConfigs();
            Debug.Log("IMPORTANT DATA DELETED!");
        }

        if (GUILayout.Button("DELETE CHANGABLE DATA(NICK, CURRENT SKINS, GENDER)"))
        {
            SaveManager.Instance.();
            saveManager.LoadAllConfigs();
            Debug.Log("CHANGABLE DATA DELETED!");
        }

        if (GUILayout.Button("DELETE BOUGHT ITEMS(ROOM, SKINS)"))
        {
            saveManager.DeleteShopDataConfig();
            saveManager.LoadAllConfigs();
            Debug.Log("BOUGHT ITEMS DELETED!");
        }

        if (GUILayout.Button("DELETE !ALL! CONFIGS"))
        {
            saveManager.DeleteAll();
            saveManager.LoadAllConfigs();
            Debug.Log("ALL! CONFIGS DELETED!");
        }
    }
}*/
