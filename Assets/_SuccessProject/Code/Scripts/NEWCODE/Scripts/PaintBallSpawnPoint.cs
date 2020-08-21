using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallSpawnPoint : MonoBehaviour
{
    public bool isOccupied = false;
    public TEAM team;
    public int startSpawnIndex;


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerHealth>())
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isOccupied = false;
    }
}
