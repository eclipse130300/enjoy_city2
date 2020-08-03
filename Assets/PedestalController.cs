using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PedestalController : MonoBehaviour
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

    public void SpawnPlayer(PaintBallPlayer player)
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
            var nakedBody = Instantiate(player.bodyConfig.game_body_prefab, spawnPlaceHolder.transform.position, spawnPlaceHolder.transform.rotation, spawnPlaceHolder.transform);

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
        var newInfo = Instantiate(playerInfoRectPrefab, infoPlaceHolder.transform.position , Quaternion.identity, infoCanvas.transform);

        newInfo.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = player.nickName;

/*        GameObject addTofriendsButton = newInfo.GetComponentInChildren<Button>().gameObject;*/
        if(photon.IsMine)
        {
        }

        //if it's our player hide add button
    }
}
