using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy1 : MonoBehaviour
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

    private Vector3 directionBullet;
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
            bullet = gameObject.GetComponent<Bullet>();
        }

        gameManagerGameObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gameManagerGameObj?.GetComponent<GameManager>();
        InvokeRepeating("ShootBullet", 0f, 3f);
    }

    void Update()
    {
        // TrackPlayer();
    }
    private void TrackPlayer()
    {

        if (player)
        {
            // Gets the player's position
            playerPos = player.GetComponent<Transform>().position;
            // Gets the direction/distance from the enemy's current pos to the player's pos
            directionBullet = playerPos - transform.position;
            // Normalises the vector, keeping the direction the same but returning a magnitude of 1 
            Vector3 direction = directionBullet.normalized;
            // Rotates the enemy so that the "up arrow" faces the direction of the player
            transform.up = direction;
            if (Vector2.Distance(playerPos, transform.position) > 0.2f)
            {
                // Moves the enemy ship towards the player
                transform.position += (direction * speed * Time.deltaTime);
            }
        }
        else
        {
            this.enabled = false;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // playerController.UpdateHealth(-damage);
            screenShake.isShaking = true;
            // SelfDestruct();
            SoundManager.instance.PlaySE(3);
        }
        else if (col.gameObject.CompareTag("PlayerBullet"))
        {
            {
                if (col.gameObject.GetComponent<Bullet>() != null)
                {
                    col.gameObject.GetComponent<Bullet>().SelfDestruct();
                }
                else
                {
                    Debug.LogError("Bullet component not found on the player bullet!");
                }
                SelfDestruct();
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
        if (bullet)
        {
            GameObject bulletInstance = Instantiate(enemyBullet, transform.position, Quaternion.identity);
            bulletInstance.GetComponent<Bullet>().SetDestination(directionBullet);
        }
    }
}

