using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int level;
    private int experience;
    private int expreienceToNextLevel = 100;
    private Loader loader;
    private SaveManager saveManager;

    private void Awake()
    {
        loader = Loader.Instance;
        saveManager = SaveManager.Instance;
        loader.AllSceneLoaded += Initialize;
        loader.StartSceneLoading += Save;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        level = saveManager.GetLvl();
        experience = saveManager.GetExp();

        Messenger.Broadcast(GameEvents.LVL_CHANGED, level);
        Messenger.Broadcast(GameEvents.EXP_CHANGED, GetNormilizedExperience());
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
        return (float)experience / expreienceToNextLevel;
    }

    private void OnApplicationPause(bool pause)
    {
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void Save()
    {
        saveManager.SaveExp(experience);
        saveManager.SaveLvl(level);
        Debug.Log("SAVE CALL");
    }

}
