using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownSystem : MonoBehaviour
{
    public readonly List<CoolDownData> coolDowns = new List<CoolDownData>();

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

    public bool DecrementCooldown(float deltaTime)
    {
        RemainingTime = Mathf.Max(RemainingTime - deltaTime, 0f);
        return RemainingTime == 0f;
    }
}
