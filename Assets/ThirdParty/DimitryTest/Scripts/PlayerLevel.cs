using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour, ILevelable
{
    private int level;
    private float experience;
    private float expreienceToNextLevel;
    private Loader loader;


    [SerializeField] private LevelConfig lvlCFG;

    public int Level { get { return level; } }

    private void Awake()
    {
        InitializeLevelConfig();
        loader = Loader.Instance;
    }

    private void Start()
    {
        loader.AllSceneLoaded += InitializeUI;
    }

    private void InitializeUI()
    {
        Messenger.Broadcast(GameEvents.LVL_CHANGED, level);
        Messenger.Broadcast(GameEvents.EXP_CHANGED, GetNormilizedExperience());
    }

    private void InitializeLevelConfig()
    {
        this.level = lvlCFG.level;
        this.experience = lvlCFG.experience;
        this.expreienceToNextLevel = lvlCFG.experienceToNextLevel;
    }

    public void AddExperience(int amount)
    {
        experience += amount;


        while (experience > expreienceToNextLevel)
        {
            level++;
            experience -= expreienceToNextLevel;
            Messenger.Broadcast(GameEvents.LVL_CHANGED, level);
        }

        Messenger.Broadcast(GameEvents.EXP_CHANGED, GetNormilizedExperience());
    }

    private float GetNormilizedExperience()
    {
        return experience / expreienceToNextLevel;
    }


}
