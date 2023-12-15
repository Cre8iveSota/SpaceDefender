using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isGameOver;
    private float elapsedTime;
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text finalScoreText;

    private int sceneIndex;
    private Camera cam;
    private ScreenShake screenShake;
    private GameObject player;
    private PlayerController playerController;
    private int totalamount = 0;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        cam = Camera.main;
        if (cam)
        {
            screenShake = cam.GetComponentInChildren<ScreenShake>();
        }
        else
        {
            Debug.LogWarning("screenShake not found!");
        }
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {

            elapsedTime += Time.deltaTime;
            int min = (int)(elapsedTime / 60);
            int sec = (int)(elapsedTime % 60);
            currentTimeText.text = $"Time: {min:00}:{sec:00}";
        }
    }

    public void beforeGameOver()
    {
        // screenShake.isShaking = false;
        playerController.SelfDestruct();
    }
    public void GameOver()
    {
        isGameOver = false;
        finalTimeText.text = currentTimeText.text;
        finalScoreText.text = scoreText.text;
        playerController.SelfDestruct();
        StartCoroutine(GameresetIntervel());
    }

    private IEnumerator GameresetIntervel()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0;
    }

    public void LoadScene()
    {
        if (sceneIndex < SceneManager.sceneCount)
        {
            sceneIndex++;
            SceneManager.LoadScene(sceneIndex);
            SoundManager.instance.PlayBGM(1);
        }
        else
        {
            sceneIndex = 0;
            SceneManager.LoadScene(sceneIndex);
            SoundManager.instance.PlayBGM(0);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit");
    }

    public void UpadateScore(int amount)
    {
        totalamount += amount;
        scoreText.text = $"Score: {totalamount:000,000} pt";
    }
}
