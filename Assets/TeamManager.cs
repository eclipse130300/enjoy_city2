using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IHaveBullet))]
public class TeamManager : MonoBehaviour
{

    public TEAM currentTeam;

    public GameObject bulletPrefab;
    public Color redTeamColor;
    public Color blueTeamColor;

    private IHaveBullet bulletKeeper;

    // Start is called before the first frame update
    void Start()
    {
        bulletKeeper = GetComponent<IHaveBullet>();
        InitializeTeamBullets(currentTeam);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void InitializeTeamBullets(TEAM currentTeam)
    {
        switch(currentTeam)
        {
            case TEAM.BLUE:
                bulletKeeper.InitializeBullet(bulletPrefab, blueTeamColor);
                break;

            case TEAM.RED:
                bulletKeeper.InitializeBullet(bulletPrefab, redTeamColor);
                break;
            
        }
    }
}

public enum TEAM
{ 
RED,
BLUE
}

