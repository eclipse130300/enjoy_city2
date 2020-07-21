using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTest : MonoBehaviour
{
    public int expAmount = 100;

    public AudioClip coinPickup;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerLevel>())
        {
            var playerLvl = other.gameObject.GetComponent<PlayerLevel>();
            playerLvl.AddExperience(expAmount);
            var randomNum = Random.Range(30, 100);
            var randomType = Random.Range(0, 2);
            ShopManager.Instance.AddCurrency(randomNum, (CurrencyType)randomType);
            Messenger.Broadcast(GameEvents.REFRESH_SANDBOX_UI);
            AudioSource.PlayClipAtPoint(coinPickup, transform.position, 2f);
            Destroy(gameObject);
        }
    }
}
