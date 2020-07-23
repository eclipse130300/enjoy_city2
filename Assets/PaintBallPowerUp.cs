using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallPowerUp : MonoBehaviour, IHaveCooldown
{
    [SerializeField] float powerUpTime;
    [SerializeField] int percentBoost;

    [SerializeField] int iD;
    [SerializeField] float cD;

    [SerializeField] CoolDownSystem coolDownSystem;
    public int CoolDownId => iD;
    public float CoolDownDuration => cD;

    [SerializeField] private ThirdPersonInput personInput;
    [SerializeField] private ShootAbility shootAbility;

    private void Awake()
    {
        if(personInput == null)
        {
            personInput = GetComponent<ThirdPersonInput>();
        }

        if(shootAbility == null)
        {
            shootAbility = GetComponent<ShootAbility>();
        }

        Messenger.AddListener(GameEvents.PAINTBALL_POWER_UP_PRESSED, PowerUp);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvents.PAINTBALL_POWER_UP_PRESSED, PowerUp);
    }

    private void PowerUp()
    {
        if (!coolDownSystem.IsOnCoolDown(iD))
        {
            StartCoroutine(PowerUpEffect());
            Debug.Log("POWER-UP STARTED");
            coolDownSystem.PutOnCooldown(this);
        }
    }

    IEnumerator PowerUpEffect()
    {
        var startingSpd = personInput.speed;
        var startingShootingDelay = shootAbility.shootingDelay;

        personInput.speed += CalculateBoostValue(personInput.speed, percentBoost);
        shootAbility.shootingDelay -= CalculateBoostValue(shootAbility.shootingDelay, percentBoost);

        yield return new WaitForSeconds(powerUpTime);

        personInput.speed = startingSpd;
        shootAbility.shootingDelay = startingShootingDelay;

    }

    private float CalculateBoostValue(float currentValue, int percentBoost)
    {
        float percentsToValue = (float) (currentValue / 100f) * percentBoost; //percent from current value

        return percentsToValue;
    }
}
