using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    // private float maxHealth = 3f;
    private int maxHealth = 10;

    // private float currentHealth;
    private int currentHealth;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform originPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private GameObject healthBarExtention;
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

    public float ScreenWidth { get => screenWidth; set => screenWidth = value; }

    void Start()
    {
        currentHealth = maxHealth;
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
            ShootBullet();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1) && canLaser)
        {
            StartCoroutine(ShootLaserBeamAsync());
        }
    }

    private void ShootBullet()
    {
        GameObject bulletInstance = Instantiate(bullet, originPoint.position, Quaternion.identity);
        bulletInstance.GetComponent<Bullet>().SetDestination(rotationDirection);
    }

    private IEnumerator ShootLaserBeamAsync()
    {
        canLaser = false;
        StartCoroutine(laser.Shoot(rotationDirection));
        yield return new WaitForSeconds(0.4f);
        canLaser = true;
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
        Debug.Log(col);
        if (col.transform.gameObject.CompareTag("EnemyBullet"))
        {
            col.gameObject.GetComponent<Bullet>().SelfDestruct();
            UpdateHealth(-3);
        }
    }
}
