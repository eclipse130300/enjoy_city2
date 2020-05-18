using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManagerScript : MonoBehaviour
{
    public RoomItemConfig item1;
    public RoomItemConfig item2;
    public RoomItemConfig item3;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            RoomItemFactory.CreateItem(item1, new Vector3(0, 0 , 4), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            RoomItemFactory.CreateItem(item2, new Vector3(0, 0, 7), Quaternion.identity);
        }

    }
}
