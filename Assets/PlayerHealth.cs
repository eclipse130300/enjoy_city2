using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int MaxHp;

    [SerializeField] private int currentHP;

    private void OnEnable()
    {
        RecoverHP();
    }

    void RecoverHP()
    {
        currentHP = MaxHp;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        DeathCheck();
    }

    public void TakeDamage(int amount, int fromTeamIndex)
    {
        currentHP -= amount;
        DeathCheck();
    }

    void DeathCheck()
    {
        if (currentHP <= 0)
        {
            //die function
            Debug.Log("I am dead!");
        }
    }
}
