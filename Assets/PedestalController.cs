using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PedestalController : MonoBehaviourPunCallbacks
{

    public GameObject darkBody;
    public GameObject spawnPlaceHolder;

    public GameObject infoCanvas;
    public GameObject playerInfoRectPrefab;
    public GameObject infoPlaceHolder;

    private PhotonView photon;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();
    }

    public void SpawnPlayerAndInfo(PaintBallPlayer player)
    {
        SpawnBody(player);
        SpawnPlayerInfo(player);
    }

    private void SpawnBody(PaintBallPlayer player)
    {
        {
            //turn off dark body
            darkBody.SetActive(false);

            //instattiate body from dobyConf
            var nakedBody = PhotonNetwork.Instantiate(player.bodyConfig.game_body_prefab.name, spawnPlaceHolder.transform.position, spawnPlaceHolder.transform.rotation,0,null,spawnPlaceHolder.transform);

            //add component SkinsManager
            var skinsManager = nakedBody.AddComponent<SkinsManager>();

            //skinsmanag.initializeFields ????
            skinsManager.InitializeFields(nakedBody.transform);

            //put on clothes from our field 
            skinsManager.PutOnClothes(player.clothesConfig);
        }
    }

    private void SpawnPlayerInfo(PaintBallPlayer player)
    {
        //spawn info in placeholder
        var newInfo = PhotonNetwork.Instantiate(playerInfoRectPrefab.name, infoPlaceHolder.transform.position , Quaternion.identity, 0, null, infoCanvas.transform);

        newInfo.GetComponent<InfoPlayer>().Initialize(player.nickName, photon.IsMine);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
