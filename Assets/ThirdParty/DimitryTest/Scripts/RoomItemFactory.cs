using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemFactory : MonoBehaviour
{

    public static GameObject CreateItem(RoomItemConfig config)
    {
        var prefab = Instantiate(config.prefab);
        if(prefab.GetComponent<MeshRenderer>() != null)   prefab.GetComponent<MeshRenderer>().material.color = config.color;
        prefab.transform.position = config.position;

        return prefab;
    }
}
