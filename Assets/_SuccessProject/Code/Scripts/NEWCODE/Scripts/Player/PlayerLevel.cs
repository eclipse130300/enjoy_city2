using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int level;
    [SerializeField] private int experience;
    [SerializeField] private int expreienceToNextLevel = 100; 

    [SerializeField] int lvlDifficultyDenominator = 5;
    [SerializeField] int difficultyMultiplier =2;
    [SerializeField] int addExpPerLvl = 10;
    private Loader loader;
    private SaveManager saveManager;

    private bool isInitialized = false;

    private void Awake()
    {
        loader = Loader.Instance;
        saveManager = SaveManager.Instance;
        loader.AllSceneLoaded += Initialize;
/*        loader.AllSceneUnloaded += SaveData;*/
    }
    private void OnDestroy()
    {
        loader.AllSceneLoaded -= Initialize;
/*        loader.AllSceneUnloaded -= SaveData;*/
    }
/*    private void Start()
    {
        Initialize();
    }*/

    private void Initialize()
    {
        level = saveManager.GetLvl();
        experience = saveManager.GetExp();
        expreienceToNextLevel = saveManager.GetExpToNextLevel();

        isInitialized = true;
/*        Debug.Log("INITIALIZE WITH : " + "LEVEL :" + level + "EXP :" + experience + "expTOnextLVL :" + expreienceToNextLevel);*/
    }

    public void AddExperience(int amount)
    {
        if (!isInitialized) Initialize();

        experience += amount;


        while (experience >= expreienceToNextLevel)
        {
            level++;
            experience -= expreienceToNextLevel;
            AddExpToNextLvl();
            Messenger.Broadcast(GameEvents.LVL_CHANGED, level);
        }

        Messenger.Broadcast(GameEvents.EXP_CHANGED, experience, expreienceToNextLevel);

        SaveData();
    }

    private void AddExpToNextLvl()
    {
        expreienceToNextLevel += addExpPerLvl;
        if(level % lvlDifficultyDenominator == 0)
        {
            addExpPerLvl *= difficultyMultiplier;
        }
    }

    private void SaveData()
    {
        saveManager.SaveLevelData(experience, expreienceToNextLevel, level);
/*        Debug.Log("SAVED WITH : " + "LEVEL :" + level + "EXP :" + experience + "expTOnextLVL :" + expreienceToNextLevel);*/
    }
}
