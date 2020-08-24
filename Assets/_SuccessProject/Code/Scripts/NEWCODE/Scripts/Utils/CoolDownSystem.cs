using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownSystem : MonoBehaviour
{
    public List<CoolDownData> coolDowns = new List<CoolDownData>();

    private void Update()
    {
        ProcessCooldowns();
    }

    private void ProcessCooldowns()
    {
        float deltaTime = Time.deltaTime;

        for (int i = coolDowns.Count - 1; i >= 0; i--)
        {
            if(coolDowns[i].DecrementCooldown(deltaTime))
            {
                coolDowns.RemoveAt(i);
            }
        }
    }

    public bool IsOnCoolDown(int Id)
    {
        foreach (CoolDownData cooldown in coolDowns)
        {
            if(cooldown.Id == Id)
            {
                return true;
            }
        }
        return false;
    }

    public float GetRemainingTime(int Id)
    {
        foreach (CoolDownData cooldown in coolDowns)
        {
            if (cooldown.Id != Id) continue;

            return cooldown.RemainingTime;
        }

        return 0f;
    }

    public void PutOnCooldown(IHaveCooldown cooldown)
    {
        coolDowns.Add(new CoolDownData(cooldown));
    }

    public void PutOnCooldown(CoolDownData data)
    {
        coolDowns.Add(data);
    }

}

[System.Serializable]
public class CoolDownData
{
    public int Id { get; }
    public float RemainingTime { get; private set; }

    public CoolDownData(IHaveCooldown cooldown)
    {
        Id = cooldown.CoolDownId;
        RemainingTime = cooldown.CoolDownDuration;
    }
    public CoolDownData(int iD, float timeToTick)
    {
        Id = iD;
        RemainingTime = timeToTick;
    }

    public bool DecrementCooldown(float deltaTime)
    {
        RemainingTime = Mathf.Max(RemainingTime - deltaTime, 0f);
        return RemainingTime == 0f;
    }
}

/*[System.Serializable]
public class ComplexCoolDownSystem
{
    public List<CoolDownSystem> systems;

    public void PutOnCoolDown(int cDSystemID)
    {

    }
}*/




