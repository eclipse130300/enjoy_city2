using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImportantDataConfig : IDataConfig
{
   public int softCurrency;
   public int hardCurrency;
   public int exp;
   public int lvl;

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
