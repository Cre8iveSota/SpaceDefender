using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    #region member variable
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private Transform originPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private GameObject healthBarExtention;

    public int physicalEnhancementNumber = 1;

    // private float maxHealth = 3f;
    public int maxHealth = 10;

    // private float currentHealth;
    public int currentHealth;

    public Vector3 currentPosition;
    private UIBarScript uIBarScript;
    private Camera cam;
    private Vector3 rotationDirection;

    public UnityEvent beforeGameOver;
    public UnityEvent gameOver;
    private Laser laser;
    // add
    private bool canLaser = true;
    private float screenHeight;
    private float screenWidth;
    // private bool canBulletShoot = true;

    // private bool canLaserShoot = true;
    private bool canSingleBulletShoot = true;
    private bool canSingleLaserShoot = true;

    public float ScreenWidth { get => screenWidth; set => screenWidth = value; }
    public int naturalHealingLevel = 0;
    ExtraAbility extraAbility;


    #endregion

    void Start()
    {
        cam = Camera.main;
        uIBarScript = healthBarExtention.GetComponentInChildren<UIBarScript>();
        uIBarScript.UpdateValue(currentHealth / maxHealth);
        laser = gameObject.GetComponent<Laser>();
        if (laser == null)
        {
            laser = gameObject.GetComponent<Laser>();
        }

        // laserがまだnullなら何か問題があるのでエラーを表示して終了
        if (laser == null)
        {
            Debug.LogError("Laser component not found on the player!");
        }
        screenHeight = Camera.main.orthographicSize;
        ScreenWidth = screenHeight * Camera.main.aspect;

        extraAbility = GetComponent<ExtraAbility>();
        PhysicalEnhancementManager();
        currentHealth = maxHealth;
        uIBarScript.UpdateValue(currentHealth, maxHealth);
        NaturalHealingAbilityManager();
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
        if (deathParticles)
        {
            float duration = deathParticles.GetComponent<ParticleSystem>().main.duration;
            Instantiate(deathParticles, transform.position, Quaternion.identity);
            StartCoroutine(WaitForParticle(duration));
        }
        else
        {
            Debug.Log("Player death particle missing");
        }
        SoundManager.instance.PlaySE(2);
    }

    IEnumerator WaitForParticle(float _duration)
    {
        yield return new WaitForSeconds(_duration);
    }

    void Update()
    {
        #region PLAYER MOVEMENT
        // Move the player w/ keyboard
        Vector3 direction = new Vector3(0, 0, 0);
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction = Vector3.ClampMagnitude(direction, 1f);
        transform.position += direction * moveSpeed * Time.deltaTime;
        Vector3 currentPosition = transform.position;
        if (currentPosition.x > ScreenWidth)
            currentPosition.x = -ScreenWidth;
        else if (currentPosition.x < -ScreenWidth)
            currentPosition.x = ScreenWidth;

        if (currentPosition.y > screenHeight)
            currentPosition.y = -screenHeight;
        else if (currentPosition.y < -screenHeight)
            currentPosition.y = screenHeight;

        // 新しい位置を適用
        transform.position = currentPosition;



        #endregion

        #region ROTATE TO FACE THE MOUSE POSITION
        // Get mouse position
        Vector3 mousePosition = Input.mousePosition;
        // Translate mouse position into world space
        mousePosition = cam.ScreenToWorldPoint(mousePosition);
        // Get mouse position relative to the current ship position
        currentPosition = transform.position;
        // Get the difference between mouse and player
        rotationDirection = new Vector3(mousePosition.x - currentPosition.x, mousePosition.y - currentPosition.y, 0f);
        // chnage the front of the player direction 
        transform.up = rotationDirection;
        #endregion

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootBulletManager();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1) && canLaser)
        {
            ShootLaserManager();
        }
    }

    private void ShootBulletManager()
    {
        bool enable = GameManager.acquiresAbility.Find((i) => i.Item1 == "ShootBulletContinuously").Item2;
        if (enable)
        {
            switch (GameManager.acquiresAbility.Find((i) => i.Item1 == "ShootBulletContinuously").Item3)
            {
                case 3:
                    StartCoroutine(extraAbility.ShootBulletContinuously(0.05f, 10, 0.5f, true));
                    break;
                case 2:
                    StartCoroutine(extraAbility.ShootBulletContinuously(0.05f, 30, 0.5f, false));
                    break;
                default:
                    StartCoroutine(extraAbility.ShootBulletContinuously(0.1f, 10, 0.5f, false));
                    break;
            }
        }
        else
        {
            StartCoroutine(ShootBullet(0.4f));
        }
    }
    private void ShootLaserManager()
    {
        bool enable = GameManager.acquiresAbility.Find((i) => i.Item1 == "ShootLaserBeamAsyncContinuously").Item2;
        if (enable)
        {
            switch (GameManager.acquiresAbility.Find((i) => i.Item1 == "ShootLaserBeamAsyncContinuously").Item3)
            {
                case 3:
                    StartCoroutine(extraAbility.ShootLaserBeamAsyncContinuously(0.1f, 10, 1f, true));
                    break;
                case 2:
                    StartCoroutine(extraAbility.ShootLaserBeamAsyncContinuously(0.08f, 20, 1f, false));
                    break;
                default:
                    StartCoroutine(extraAbility.ShootLaserBeamAsyncContinuously(0.5f, 10, 1f, false));
                    break;
            }
        }
        else
        {
            StartCoroutine(ShootLaserBeamAsync(0.4f));
        }
    }
    private void PhysicalEnhancementManager()
    {
        bool enable = GameManager.acquiresAbility.Find((i) => i.Item1 == "PhysicalEnhancement").Item2;
        if (enable)
        {
            switch (GameManager.acquiresAbility.Find((i) => i.Item1 == "PhysicalEnhancement").Item3)
            {
                case 3:
                    extraAbility?.PhysicalEnhancement(4);
                    break;
                case 2:
                    extraAbility?.PhysicalEnhancement(3);
                    break;
                default:
                    extraAbility?.PhysicalEnhancement(2);
                    break;
            }
        }
        else
        {
            extraAbility?.PhysicalEnhancement(1);
        }

    }

    private void NaturalHealingAbilityManager()
    {
        bool enable = GameManager.acquiresAbility.Find((i) => i.Item1 == "NaturalHealingAbility").Item2;
        if (enable)
        {
            extraAbility.CanNaturalHealingAbility = true;
            switch (GameManager.acquiresAbility.Find((i) => i.Item1 == "NaturalHealingAbility").Item3)
            {
                case 3:
                    naturalHealingLevel = 3;
                    break;
                case 2:
                    naturalHealingLevel = 2;
                    break;
                case 1:
                    naturalHealingLevel = 1;
                    break;
            }
        }
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        Debug.Log("Player health: " + currentHealth);
        uIBarScript.UpdateValue(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            // beforeGameOver.Invoke();
            // await Task.Delay(600); // Wait 600 msec unitl the update hp animation done. Otherwise, if player gets  big damage, the game over screen is showed before Hralth bar 0
            gameOver.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Debug.Log(col);
        if (col.transform.gameObject.CompareTag("EnemyBullet"))
        {
            col.gameObject.GetComponent<Bullet>().SelfDestruct();
            UpdateHealth(-3);
        }
    }
    #region Normal Ability
    public IEnumerator ShootBullet(float interval)
    {
        if (canSingleBulletShoot)
        {
            canSingleBulletShoot = false;
            GameObject bulletInstance = Instantiate(bullet, originPoint.position, Quaternion.identity);
            bulletInstance.GetComponent<Bullet>().SetDestination(rotationDirection);
            yield return new WaitForSeconds(interval);
            canSingleBulletShoot = true;
        }
    }

    public IEnumerator ShootLaserBeamAsync(float interval)
    {
        if (canSingleLaserShoot)
        {
            canSingleLaserShoot = false;
            StartCoroutine(laser.Shoot(rotationDirection));
            yield return new WaitForSeconds(interval);
            canSingleLaserShoot = true;
        }
    }
    #endregion
}
