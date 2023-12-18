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
    public int totalamount = 0;

    public static List<Sprite> droppedExtraAbility = new List<Sprite>();
    public static List<(string, bool, int)> acquiresAbility = new List<(string, bool, int)>();
    private int level1 = 1;
    private int level2 = 2;
    private int level3 = 3;

    // Start is called before the first frame update
    void Start()
    {
        // acquiresAbility.Add(("ShootBulletContinuously", false, level2));
        // acquiresAbility.Add(("ShootLaserBeamAsyncContinuously", false, level2));
        // acquiresAbility.Add(("PhysicalEnhancement", false, level3));
        // acquiresAbility.Add(("NaturalHealingAbility", false, level3));
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
        // foreach (var item in acquiresAbility)
        // {
        //     Debug.Log($"Acquired ability: {item.Item1}, {item.Item2}, {item.Item3}");
        // }


        // if (droppedExtraAbility.Find((i) => i.texture.name == "Icon2"))
        // {
        //     acquiresAbility.Add(("ShootBulletContinuously", true, level2));
        // }

        for (int i = 0; i < acquiresAbility.Count; i++)
        {
            Debug.Log($"ExtraAbility {acquiresAbility[i].Item1}, {acquiresAbility[i].Item2}, {acquiresAbility[i].Item3}");
        }
        if (!isGameOver)
        {

            elapsedTime += Time.deltaTime;
            int min = (int)(elapsedTime / 60);
            int sec = (int)(elapsedTime % 60);
            if (currentTimeText) { currentTimeText.text = $"Time: {min:00}:{sec:00}"; }
        }

        droppedExtraAbility.ForEach((i) => Debug.Log("drpeed: " + i));
        // Debug.Log(droppedExtraAbility);
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
        Debug.Log("SceneManager.sceneCount; " + SceneManager.sceneCount);
        if (sceneIndex < 2)
        {
            sceneIndex++;
            SceneManager.LoadScene(sceneIndex);
            SoundManager.instance.PlayBGM(1);
        }
        else if (sceneIndex == 2)
        {
            acquiresAbility.RemoveAll(item => item.Item1 == "ShootBulletContinuously" && item.Item2 == true);
            acquiresAbility.RemoveAll(item => item.Item1 == "ShootLaserBeamAsyncContinuously" && item.Item2 == true);
            acquiresAbility.RemoveAll(item => item.Item1 == "PhysicalEnhancement" && item.Item2 == true);
            droppedExtraAbility.ForEach((i) =>
                {
                    switch (i.texture.name)
                    {
                        case "Icon2":
                            acquiresAbility.Add(("ShootBulletContinuously", true, level2));
                            break;
                        case "Icon1":
                            acquiresAbility.Add(("ShootLaserBeamAsyncContinuously", true, level2));
                            break;
                        case "Icon3":
                            acquiresAbility.Add(("PhysicalEnhancement", true, level3));
                            break;
                        default:
                            break;
                    }
                }
            );
            for (int i = 0; i < acquiresAbility.Count; i++)
            {
                Debug.Log($"ChoseExtraAbility {acquiresAbility[i].Item1}, {acquiresAbility[i].Item2}, {acquiresAbility[i].Item3}");
            }
            sceneIndex = 0;
            SceneManager.LoadScene(sceneIndex);
            SoundManager.instance.PlayBGM(0);
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
