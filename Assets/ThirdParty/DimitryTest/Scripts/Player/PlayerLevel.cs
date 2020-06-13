using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int level;
    [SerializeField] private int experience; //dbug
    [SerializeField] private int expreienceToNextLevel = 100; //dbug

    [SerializeField] int lvlDifficultyDenominator = 5;
    [SerializeField] int difficultyMultiplier =2;
    [SerializeField] int addExpPerLvl = 10;
    private Loader loader;
    private SaveManager saveManager;

    private void Awake()
    {
        loader = Loader.Instance;
        saveManager = SaveManager.Instance;
        loader.AllSceneLoaded += Initialize;
        loader.AllSceneUnloaded += Save;
    }
    private void OnDestroy()
    {

        loader.AllSceneLoaded -= Initialize;
        loader.AllSceneUnloaded -= Save;
    }
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        level = saveManager.GetLvl();
        experience = saveManager.GetExp();
        expreienceToNextLevel = saveManager.GetExpToNextLevel();
    }

    public void AddExperience(int amount)
    {
        experience += amount;


        while (experience >= expreienceToNextLevel)
        {
            level++;
            experience -= expreienceToNextLevel;
            AddExpToNextLvl();
            Messenger.Broadcast(GameEvents.LVL_CHANGED, level);
        }

        Messenger.Broadcast(GameEvents.EXP_CHANGED, experience, expreienceToNextLevel);
    }

    private void AddExpToNextLvl()
    {
        expreienceToNextLevel += addExpPerLvl;
        if(level % lvlDifficultyDenominator == 0)
        {
            addExpPerLvl *= difficultyMultiplier;
        }
    }

    private void Save()
    {
        saveManager.SaveExp(experience);
        saveManager.SaveLvl(level);
        saveManager.SaveExpToNextLvl(expreienceToNextLevel);
        Debug.Log("SAVE CALL");
    }

    private void OnApplicationQuit()
    {
        Save();
    }

}
