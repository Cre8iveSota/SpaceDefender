using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] public float speed = 6;
    public List<Transform> originPoint = new List<Transform>();
    private Vector3 playerPos;
    private GameObject player;
    private PlayerController playerController;
    [SerializeField] public int damage = 1;
    private Camera cam;
    private ScreenShake screenShake;
    [SerializeField] private GameObject enemyBullet;
    [SerializeField] public int enemyScore;
    [SerializeField] private GameObject healthBarExtention;
    private List<(string, bool, int)> acquiresAbility = new List<(string, bool, int)>();
    private int launchedHormingNumber = 0;


    Bullet bullet;
    GameObject gameManagerGameObj;
    GameManager gameManager;
    private float screenHeight;

    public float ScreenWidth { get; private set; }
    public int currentHealth;

    public int maxHealth = 100;
    private Vector3 directionBullet;
    private UIBarScript uIBarScript;
    ExtraAbility extraAbility;


    private int level1 = 1;
    private int level2 = 2;
    private int level3 = 3;
    private bool enabledHorming = false;
    private bool hasLaunchedHorming;
    private bool isDone;

    void Start()
    {
        extraAbility = GetComponent<ExtraAbility>();
        acquiresAbility.Add(("ShootBulletContinuously", true, level1));
        extraAbility.CanNaturalHealingAbility = false;
        acquiresAbility.Add(("HormingEnemy", true, level1));
        // acquiresAbility.Add(("PhysicalEnhancement", false, level3));
        // acquiresAbility.Add(("NaturalHealingAbility", true, level3));
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
        cam = Camera.main;
        if (cam)
        {
            screenShake = cam.GetComponentInChildren<ScreenShake>();
        }
        else
        {
            Debug.LogWarning("screenShake not found!");
        }

        gameManagerGameObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gameManagerGameObj?.GetComponent<GameManager>();
        screenHeight = Camera.main.orthographicSize;
        ScreenWidth = screenHeight * Camera.main.aspect;
        currentHealth = maxHealth;
        uIBarScript = healthBarExtention.GetComponentInChildren<UIBarScript>();
        uIBarScript.UpdateValue(currentHealth, maxHealth);
    }

    void Update()
    {
        EscapePlayer();
        ShootBulletManager();
        if (enabledHorming && !hasLaunchedHorming)
        {
            hasLaunchedHorming = true;
            HormingEnemyManager();
            launchedHormingNumber++;
        }
    }
    private void EscapePlayer()
    {

        if (player)
        {
            playerPos = player.transform.position;
            directionBullet = playerPos - transform.position;
            directionBullet.z = 0;  // 3D回転を防ぐためにz軸をゼロに設定
            Vector3 direction = directionBullet.normalized;
            transform.up = direction;

            // Calculate the farthest corner from the player
            Vector3 farthestCorner = Vector3.zero;
            float maxDistance = 0f;
            Vector3[] screenCorners = new Vector3[4];
            screenCorners[0] = new Vector3(ScreenWidth, screenHeight, 0f);
            screenCorners[1] = new Vector3(-ScreenWidth, screenHeight, 0f);
            screenCorners[2] = new Vector3(-ScreenWidth, -screenHeight, 0f);
            screenCorners[3] = new Vector3(ScreenWidth, -screenHeight, 0f);


            for (int i = 0; i < 4; i++)
            {
                float distance = Vector3.Distance(playerPos, screenCorners[i]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestCorner = screenCorners[i];
                }
            }

            // Move the boss towards the farthest corner
            float step = speed * Time.deltaTime;
            Vector3 targetPosition = farthestCorner;
            if (!ApproximatelyEqual(transform.position, targetPosition))
            {
                Debug.Log("targetPosition:" + targetPosition);
                Debug.Log("transform.position:" + transform.position);
                Vector2 offset = new Vector2(4f, 4f);
                if (transform.position != targetPosition)
                {
                    if (targetPosition.x < 0) { offset.x = -offset.x; }
                    if (targetPosition.y < 0) { offset.y = -offset.y; }
                    if (currentHealth < 0.6 * maxHealth)
                    {
                        transform.position += new Vector3(targetPosition.x - (transform.position.x + offset.x), targetPosition.y - (transform.position.y + offset.y), 0f) * step;
                        Debug.Log("spped up");
                    }
                    else
                    {
                        transform.position += new Vector3(targetPosition.x - (transform.position.x + offset.x), targetPosition.y - (transform.position.y + offset.y), 0f).normalized * step;
                        Debug.Log("spped normal");
                    }

                }
            }
        }
        else
        {
            this.enabled = false;
            Debug.Log($"Playerrrrrr");

        }
    }
    bool ApproximatelyEqual(Vector3 a, Vector3 b, float tolerance = 0.001f)
    {
        return Mathf.Abs(a.x - b.x) < tolerance &&
               Mathf.Abs(a.y - b.y) < tolerance &&
               Mathf.Abs(a.z - b.z) < tolerance;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerController.UpdateHealth(-damage);
            UpdateHealth(-1);

            screenShake.isShaking = true;
            // SelfDestruct();
            // SoundManager.instance.PlaySE(3);
        }
        else if (col.gameObject.CompareTag("PlayerBullet"))
        {
            {
                Debug.Log("PlayerBullet collision detected!");
                if (col.gameObject.GetComponent<Bullet>() != null)
                {
                    col.gameObject.GetComponent<Bullet>().SelfDestruct();
                    UpdateHealth(-1);
                    Debug.Log("Bullet destroyed!");
                }
                else
                {
                    Debug.LogError("Bullet component not found on the player bullet!");
                }
                // SelfDestruct();
            }
        }
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
        gameManager.UpadateScore(enemyScore);
    }
    public void ShootBullet()
    {
        GameObject bulletInstance = Instantiate(enemyBullet, originPoint[0].position, Quaternion.identity);
        bulletInstance.GetComponent<Bullet>().SetDestination(directionBullet);
    }
    public void ShootBulletSub()
    {
        GameObject bulletInstanceSub1 = Instantiate(enemyBullet, originPoint[1].position, Quaternion.identity);
        bulletInstanceSub1.GetComponent<Bullet>().SetDestination(directionBullet);
        GameObject bulletInstanceSub2 = Instantiate(enemyBullet, originPoint[2].position, Quaternion.identity);
        bulletInstanceSub2.GetComponent<Bullet>().SetDestination(directionBullet);
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        Debug.Log("Player health: " + currentHealth);
        uIBarScript.UpdateValue(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            SelfDestruct();
        }
    }

    private void ShootBulletManager()
    {
        if (0.6f * maxHealth < currentHealth && currentHealth < 0.8f * maxHealth)
        {
            int indexToReplace = 0;  // Index of the first item
            // Replace the third argument of the first item with level2
            acquiresAbility[indexToReplace] = (acquiresAbility[indexToReplace].Item1, acquiresAbility[indexToReplace].Item2, level2);
            Debug.Log("replaced to level2 of shoot: " + acquiresAbility[0]);
        }
        else if (0.4f * maxHealth < currentHealth && currentHealth < 0.6f * maxHealth)
        {
            int indexToReplace = 0;  // Index of the first item
            // Replace the third argument of the first item with level2
            acquiresAbility[indexToReplace] = (acquiresAbility[indexToReplace].Item1, acquiresAbility[indexToReplace].Item2, level3);
            Debug.Log("replaced to level3 of shoot:" + acquiresAbility[0]);
        }
        else if (0.2f * maxHealth < currentHealth && currentHealth < 0.4 * maxHealth)
        {
            StartCoroutine(extraAbility.SubShootBulletContinuously());
            enabledHorming = true;
            Debug.Log("first horming");
            isDone = true;
        }
        else if (currentHealth < 0.2f * maxHealth && isDone)
        {
            StartCoroutine(extraAbility.SubShootBulletContinuously());
            enabledHorming = true;
            if (launchedHormingNumber < 2) { hasLaunchedHorming = false; }
            Debug.Log("second horming");
            extraAbility.hormingEnemyInterval = 0f;
        }
        // bool enable = acquiresAbility.Find((i) => i.Item1 == "ShootBulletContinuously").Item2;
        // if (enable)
        // {
        Debug.Log($"Switch Value: {acquiresAbility.Find((i) => i.Item1 == "ShootBulletContinuously").Item3}");

        switch (acquiresAbility.Find((i) => i.Item1 == "ShootBulletContinuously").Item3)
        {
            case 3:
                StartCoroutine(extraAbility.ShootBulletContinuously(0.1f, 30, 1f, false));
                break;
            case 2:
                StartCoroutine(extraAbility.ShootBulletContinuously(0f, 1, 0.2f, false));
                break;
            default:
                StartCoroutine(extraAbility.ShootBulletContinuously(0f, 1, 0.5f, false));
                break;
        }
        // }
    }

    private void HormingEnemyManager()
    {
        enabledHorming = false;
        bool enable = acquiresAbility.Find((i) => i.Item1 == "HormingEnemy").Item2;
        if (enable)
        {
            extraAbility.HormingEnemy();
        }
    }
}

