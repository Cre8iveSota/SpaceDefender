using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float speed = 6;
    private Vector3 playerPos;
    private GameObject player;
    private PlayerController playerController;
    [SerializeField] public int damage = 1;
    private Camera cam;
    private ScreenShake screenShake;
    [SerializeField] private GameObject bullet;
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
        // childTransform = transform.Find("Origin Point");
        // if (childTransform)
        // {
        //     originPosition = childTransform.position;
        InvokeRepeating("ShootBullet", 2f, 3f);

    }

    void TrackPlayer()
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

    void Update()
    {
        TrackPlayer();
    }
    private async void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            await playerController.UpdateHealth(-damage);
            screenShake.isShaking = true;
            SelfDestruct();
            SoundManager.instance.PlaySE(3);
        }
        else if (col.gameObject.CompareTag("PlayerBullet"))
        {
            // col.gameObject.GetComponent<Bullet>().SelfDestruct();
            // Debug.Log(("OOps"));
            // SelfDestruct();
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
                SelfDestruct();
            }
        }
    }

    public void SelfDestruct()
    {
        if (!gameObject) { return; }
        Destroy(gameObject);
    }
    private void ShootBullet()
    {
        if (gameObject.GetComponent<Bullet>())
        {
            // GameObject bulletInstance = Instantiate(bullet, originPosition, Quaternion.identity);
            GameObject bulletInstance = Instantiate(bullet, transform.position, Quaternion.identity);
            bulletInstance.GetComponent<Bullet>().SetDestination(directionBullet);
        }
    }

}
