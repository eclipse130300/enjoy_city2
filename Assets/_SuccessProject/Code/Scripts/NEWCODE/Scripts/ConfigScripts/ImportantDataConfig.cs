using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImportantDataConfig : IDataConfig
{
   public int softCurrency = 100;
   public int hardCurrency = 100;
   public int exp = 0;
   public int lvl = 1;
   public int expToNextLvl = 100;

    public void SetHardCurrency(int hardC)
    {
        hardCurrency = hardC;
    }

    public void SetSoftCurrency(int softC)
    {
        softCurrency = softC;
    }

    public void SetExperience(int experience)
    {
        exp = experience;
    }

    public void SetLvl(int lev)
    {
        lvl = lev;
    }
}
