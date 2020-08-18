using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IHaveBullet))]
public class PlayerTeam : MonoBehaviour
{
    public GameObject bulletPrefab;

    public TEAM currentTeam;
    public Color teamColor;
    private IHaveBullet bulletKeeper;

    public PaintBallPlayer myPlayer;

    // Start is called before the first frame update
    void Start()
    {
        bulletKeeper = GetComponent<IHaveBullet>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePlayerTeam(TEAM team, string colorHex)
    {
        this.currentTeam = team;
        Color playerCol;
        if (ColorUtility.TryParseHtmlString(colorHex, out playerCol)) teamColor = playerCol;

        bulletKeeper.InitializeBullet(bulletPrefab, teamColor);
    }
}



