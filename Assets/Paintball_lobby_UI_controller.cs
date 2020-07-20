using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Paintball_lobby_UI_controller : MonoBehaviour
{
    [SerializeField] List<GameObject> pedestals = new List<GameObject>();
    [SerializeField] List<RectTransform> playerInfos = new List<RectTransform>();
    [SerializeField] RectTransform playerInfoRect;
    [SerializeField] GameObject playerInfoPlaceHolder;

    private Dictionary<GameObject, RectTransform> rectToGameobject = new Dictionary<GameObject, RectTransform>();

    private void Start()
    {

    }

    private void OnNewPlayerConnected(int playersAmount)
    {

    }

    private void OnPlayerDisconected()
    {

    }
}