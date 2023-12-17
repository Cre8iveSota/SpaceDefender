using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] public float speed = 6;
    private Vector3 playerPos;
    private GameObject player;
    private PlayerController playerController;
    [SerializeField] public int damage = 1;
    private Camera cam;
    private ScreenShake screenShake;
    [SerializeField] private GameObject enemyBullet;
    [SerializeField] public int enemyScore;
    Bullet bullet;
    GameObject gameManagerGameObj;
    GameManager gameManager;
    private float screenHeight;

    public float ScreenWidth { get; private set; }

    private Vector3 directionBullet;
    private Vector3 pastposition;

    void Start()
    {
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

        if (enemyBullet)
        {
            InvokeRepeating("ShootBullet", 0f, 3f);
        }

        gameManagerGameObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gameManagerGameObj?.GetComponent<GameManager>();
        screenHeight = Camera.main.orthographicSize;
        ScreenWidth = screenHeight * Camera.main.aspect;
    }

    void Update()
    {
        EscapePlayer();
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
                Vector2 offset = new Vector2(2f, 2f);
                if (transform.position != targetPosition)
                {
                    if (targetPosition.x < 0) { offset.x = -offset.x; }
                    if (targetPosition.y < 0) { offset.y = -offset.y; }
                    transform.position += new Vector3(targetPosition.x - (transform.position.x + offset.x), targetPosition.y - (transform.position.y + offset.y), 0f).normalized * step;
                    // transform.position += new Vector3(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y, 0f) * step;

                }
            }
            pastposition = transform.position;
            Debug.Log($"Player Position: {playerPos}");
            Debug.Log($"Farthest Corner: {farthestCorner}");
            Debug.Log($"Target Position: {targetPosition}");
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
    private void ShootBullet()
    {
        GameObject bulletInstance = Instantiate(enemyBullet, transform.position, Quaternion.identity);
        bulletInstance.GetComponent<Bullet>().SetDestination(directionBullet);
    }
}

