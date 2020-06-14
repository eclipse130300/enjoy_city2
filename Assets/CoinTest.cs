using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTest : MonoBehaviour
{
    public int expAmount = 100;

    public CurrencyType currType;
    public int currAmount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerLevel>())
        {
            var playerLvl = other.gameObject.GetComponent<PlayerLevel>();
            playerLvl.AddExperience(expAmount);
            ShopManager.Instance.AddCurrency(currAmount, currType);
            Destroy(gameObject);
        }
    }
}
