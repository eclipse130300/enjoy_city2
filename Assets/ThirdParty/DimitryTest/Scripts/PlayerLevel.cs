using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
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
        Messenger<int>.Broadcast(GameEvents.LVL_CHANGED, level);
        Messenger<float>.Broadcast(GameEvents.EXP_CHANGED, GetNormilizedExperience());
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
            Messenger<int>.Broadcast(GameEvents.LVL_CHANGED, level);
        }

        Messenger<float>.Broadcast(GameEvents.EXP_CHANGED, GetNormilizedExperience());

        Debug.Log(GetNormilizedExperience());
    }

    private float GetNormilizedExperience()
    {
        return experience / expreienceToNextLevel;
    }

    private void OnDestroy()
    {
        Loader.Instance.AllSceneLoaded -= InitializeUI;
    }
}
