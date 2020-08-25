using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RotateToTheLocalPlayerCam : MonoBehaviour
{

    public Transform localPlayerCam;

    private void OnEnable()
    {
        GameObject playerGO = (GameObject)PhotonNetwork.LocalPlayer.TagObject;
        localPlayerCam = playerGO.GetComponentInChildren<PlayerCamera>().transform;
    }

    private void Update()
    {
        if (localPlayerCam == null) return;

        Vector3 reversedToCamVec = - (localPlayerCam.position - transform.position);

        Quaternion rotation = Quaternion.LookRotation(reversedToCamVec, Vector3.up);

        transform.rotation = rotation;
    }
}
