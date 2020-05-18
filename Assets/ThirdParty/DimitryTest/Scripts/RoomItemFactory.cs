using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemFactory : MonoBehaviour
{

    public static GameObject CreateItem(RoomItemConfig config, Vector3 poistion, Quaternion rotation)
    {
        var prefab = Instantiate(config.prefab);
        if(prefab.GetComponent<MeshRenderer>() != null)   prefab.GetComponent<MeshRenderer>().material.color = config.color;

        return prefab;
    }
}
