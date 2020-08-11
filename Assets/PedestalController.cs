using CMS.Config;
using Photon.Pun;
using SocialGTA;
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
    // current info
    public PaintBallPlayer currentPlayer;
    private GameObject playerPref;
    private GameObject playerInfo;

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
            var bodyPref = ScriptableList<BodyConfig>.instance.GetItemByID(player.bodyConfigID).game_body_prefab;

            /*            var nakedBody = PhotonNetwork.Instantiate(bodyPref.name, spawnPlaceHolder.transform.position, 
                            spawnPlaceHolder.transform.rotation,0,null,spawnPlaceHolder.transform);*/

            var nakedBody = Instantiate(bodyPref, spawnPlaceHolder.transform.position, spawnPlaceHolder.transform.rotation, spawnPlaceHolder.transform);

            //add component SkinsManager
            var skinsManager = nakedBody.AddComponent<SkinsManager>();

            //skinsmanag.initializeFields ????
            skinsManager.InitializeFields(nakedBody.transform);

            //put on clothes from our field 
            skinsManager.PutOnClothes(player.clothesConfig);

            playerPref = nakedBody;
            currentPlayer = player;
        }
    }

    private void SpawnPlayerInfo(PaintBallPlayer player)
    {
        //spawn info in placeholder
        /*var newInfo = PhotonNetwork.Instantiate(playerInfoRectPrefab.name, infoPlaceHolder.transform.position , Quaternion.identity, 0, null, infoCanvas.transform);*/

        var newInfo = Instantiate(playerInfoRectPrefab, infoPlaceHolder.transform.position, Quaternion.identity, infoCanvas.transform);

        AutorizationController autorization = new AutorizationController();
        autorization.Login();
        string playerID = autorization.profile.UserName;

        newInfo.GetComponent<InfoPlayer>().Initialize(player.nickName, player.nickName == SaveManager.Instance.GetNickName()); //we check if our player is target player

        playerInfo = newInfo;
    }
    public void DeletePlayerAndInfo()
    {
        currentPlayer = null;
        Destroy(playerPref);
        Destroy(playerInfo);
        darkBody.SetActive(true);
    }


}
