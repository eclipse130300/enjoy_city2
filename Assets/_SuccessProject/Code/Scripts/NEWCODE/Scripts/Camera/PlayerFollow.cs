using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    private Transform player;
    [SerializeField] float Yoffset;

    private void Awake()
    {
        player = FindObjectOfType<PlayerLevel>().transform;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 followXZ = new Vector3(player.position.x, player.position.y + Yoffset, player.position.z);
            transform.position = followXZ;
        }
    }
}
