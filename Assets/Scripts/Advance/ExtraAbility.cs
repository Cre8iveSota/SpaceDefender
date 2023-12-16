using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraAbility : MonoBehaviour
{
    private bool canBulletShoot = true;

    private bool canLaserShoot = true;
    private PlayerController playerController;
    public bool CanNaturalHealingAbility = true;

    #region ExtraAbility

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (CanNaturalHealingAbility)
        {
            InvokeRepeating("NaturalHealingAbility", 0f, ((1 + playerController.NaturalHealingLevel) / (playerController.NaturalHealingLevel * 0.25f)) + 2f);
        }
    }

    public IEnumerator ShootBulletContinuously(float period, int bulletCount, float interval, bool canGatring)
    {
        if (canBulletShoot || canGatring)
        {
            canBulletShoot = false;
            for (int i = 0; i < bulletCount; i++)
            {
                StartCoroutine(playerController.ShootBullet(0f));
                yield return new WaitForSeconds(period);
            }
            yield return new WaitForSeconds(interval);
            canBulletShoot = true;
        }
    }
    public IEnumerator ShootLaserBeamAsyncContinuously(float period, int laseCount, float interval, bool canGatring)
    {
        if (canLaserShoot || canGatring)
        {
            canLaserShoot = false;
            for (int i = 0; i < laseCount; i++)
            {
                StartCoroutine(playerController.ShootLaserBeamAsync(0f));
                yield return new WaitForSeconds(period);
            }
            yield return new WaitForSeconds(interval);
            canLaserShoot = true;
        }
    }

    public void PhysicalEnhancement(int multiplier)
    {
        if (playerController)
        {
            playerController.maxHealth *= multiplier;
            playerController.moveSpeed *= multiplier;
        }
    }

    public void NaturalHealingAbility()
    {
        int healAmount = playerController.NaturalHealingLevel;
        if (playerController.currentHealth >= playerController.maxHealth) { return; }
        playerController.UpdateHealth(healAmount);
    }
    #endregion
}
