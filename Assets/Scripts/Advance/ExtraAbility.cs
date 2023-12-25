using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraAbility : MonoBehaviour
{
    private bool canBulletShoot = true;
    private bool canSubBulletShoot = true;


    private bool canLaserShoot = true;
    private PlayerController playerController;
    private Boss boss;
    public bool CanNaturalHealingAbility = true;
    [SerializeField] private List<GameObject> extraEnemy;
    public float hormingEnemyInterval = 1f;
    float healingInterval;

    public bool isHealing = true;

    #region ExtraAbility

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        boss = GetComponent<Boss>();
    }

    public IEnumerator ShootBulletContinuously(float period, int bulletCount, float interval, bool canGatring)
    {
        if (canBulletShoot || canGatring)
        {
            canBulletShoot = false;
            for (int i = 0; i < bulletCount; i++)
            {
                if (playerController)
                {
                    StartCoroutine(playerController.ShootBullet(0f));
                }
                if (boss)
                {
                    boss.ShootBullet();
                }

                yield return new WaitForSeconds(period);
            }
            yield return new WaitForSeconds(interval);

            canBulletShoot = true;
        }
    }
    public IEnumerator SubShootBulletContinuously()
    {
        if (boss?.currentHealth < 0.4f * boss?.maxHealth && canSubBulletShoot)
        {
            canSubBulletShoot = false;
            for (int i = 0; i < 10; i++)
            {
                boss.ShootBulletSub();
                yield return new WaitForSeconds(1f);
            }
            canSubBulletShoot = true;
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

    // public void PhysicalEnhancement(int multiplier)
    // {
    //     if (playerController)
    //     {
    //         playerController.maxHealth *= multiplier;
    //         playerController.moveSpeed *= multiplier;
    //     }
    // }

    public IEnumerator NaturalHealingAbility()
    {
        if (playerController)
        {
            int healAmount = playerController.naturalHealingLevel;

            if (playerController.currentHealth >= playerController.maxHealth) { yield return null; }
            playerController.UpdateHealth(healAmount);
        }
        yield return new WaitForSeconds(healingInterval);
        isHealing = true;
    }

    public void HormingEnemy()
    {
        if (boss)
        {
            StartCoroutine(SpawnEnemiesWithInterval());
        }
    }

    private IEnumerator SpawnEnemiesWithInterval()
    {
        foreach (GameObject enemyPrefab in extraEnemy)
        {
            Instantiate(enemyPrefab, boss.originPoint[0].position, Quaternion.identity);
            yield return new WaitForSeconds(hormingEnemyInterval);
        }
    }
    #endregion

    private void Update()
    {
        if (CanNaturalHealingAbility && playerController && isHealing)
        {
            isHealing = false;
            healingInterval = ((1 + playerController.naturalHealingLevel) / (playerController.naturalHealingLevel * 0.25f)) + 2f;
            StartCoroutine(NaturalHealingAbility());
        }
    }
}
