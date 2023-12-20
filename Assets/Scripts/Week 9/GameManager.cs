using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private TMP_Text totalSelectivePointText;
    [SerializeField] private TMP_Text yourChosePointText;

    private int sceneIndex;
    private Camera cam;
    private ScreenShake screenShake;
    private GameObject player;
    private PlayerController playerController;
    public static int totalamount = 0;

    public static List<(Sprite, string)> droppedExtraAbility = new List<(Sprite, string)>();
    public static List<(Sprite, string)> pastDroppedExtraAbility = new List<(Sprite, string)>();

    public static List<(string, bool, int)> acquiresAbility = new List<(string, bool, int)>();
    private int level1 = 1;
    private int level2 = 2;
    private int level3 = 3;
    private int yourChosePointNumber = 0;
    private int choosingExtraAbilityNumber = 0;
    private bool isExecuting = false;
    private int previousDroppedExtraAbilityCount = 0;


    int updateOnly = 0;
    int perrfom = 0;
    bool isInitial = true;
    bool isInitial2 = true;

    int countIrekawari = 0;


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

    void Update()
    {
        Debug.Log("perform: " + perrfom);
        Debug.Log("updateOnly: " + updateOnly);

        Debug.Log("1 droppedExtraAbility.Count" + droppedExtraAbility.Count);
        Debug.Log("1 pastDroppedExtraAbility.Count " + pastDroppedExtraAbility.Count);
        if (totalSelectivePointText) totalSelectivePointText.text = $"{totalamount}";
        if (yourChosePointText) yourChosePointText.text = $"{yourChosePointNumber}";
        Debug.Log("yourChosePointNumber " + yourChosePointNumber);

        // isExcuting フラグが解除されていない場合のみ Test メソッドを実行

        if (!AreListsEqual(pastDroppedExtraAbility, droppedExtraAbility) && !isExecuting)
        {
            Test();
        }
    }
    private void Test()
    {
        isExecuting = true;

        Debug.Log("Test start");
        Debug.Log("2 droppedExtraAbility.Count" + droppedExtraAbility.Count);
        Debug.Log("2 pastDroppedExtraAbility.Count " + pastDroppedExtraAbility.Count);

        int listSize = droppedExtraAbility.Count;

        LatestExtraAbilityFilleter(droppedExtraAbility);

        Debug.Log("2 2 droppedExtraAbility.Count" + droppedExtraAbility.Count);

        Debug.Log("Kitenaiyo");

        yourChosePointNumber = 0;
        droppedExtraAbility.ForEach(i => Debug.Log("chuumoku " + i.Item1.texture.name));
        droppedExtraAbility.ForEach(i => UpdatePoints(i.Item1.texture.name, true));
        isExecuting = false;
        pastDroppedExtraAbility = new List<(Sprite, string)>(droppedExtraAbility);

    }
    private bool AreListsEqual(List<(Sprite, string)> list1, List<(Sprite, string)> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i].Item2 != list2[i].Item2 || !list1[i].Item1.Equals(list2[i].Item1))
            {
                return false;
            }

        }

        return true;
    }

    private void LatestExtraAbilityFilleter(List<(Sprite, string)> list)
    {
        List<(Sprite, string)> tmpComp = new List<(Sprite, string)>();
        List<(Sprite, string)> tmp1 = new List<(Sprite, string)>();
        List<(Sprite, string)> tmp2 = new List<(Sprite, string)>();
        List<(Sprite, string)> tmp3 = new List<(Sprite, string)>();

        Debug.Log("2 now: droppedExtraAbility; " + droppedExtraAbility[0].Item2);
        // item2の同じ項目のみfillterをかける
        string filterCondition1 = "Drop Area 1";
        tmp1 = list.Where(item => item.Item2 == filterCondition1).ToList();
        if (tmp1.Count > 0) Debug.Log("2 :tmp1 :" + tmp1[0].Item1.texture.name);
        if (tmp1.Count > 0) tmpComp.Add(tmp1[tmp1.Count - 1]);

        string filterCondition2 = "Drop Area 2";
        tmp2 = list.Where(item => item.Item2 == filterCondition2).ToList();
        if (tmp2.Count > 0) Debug.Log("2 :tmp3 :" + tmp2[0].Item1.texture.name);

        if (tmp2.Count > 0) tmpComp.Add(tmp2[tmp2.Count - 1]);

        string filterCondition3 = "Drop Area 3";
        tmp3 = list.Where(item => item.Item2 == filterCondition3).ToList();
        if (tmp3.Count > 0) Debug.Log("2 :tmp3 :" + tmp3[0].Item1.texture.name);
        if (tmp3.Count > 0) tmpComp.Add(tmp3[tmp3.Count - 1]);

        list.Clear();

        // 絞られた中の、最後の配列をdroppedExtraAiblityに代入する
        list.AddRange(tmpComp);
    }

    private void UpdatePoints(string textureName, bool isAdd)
    {
        int point = 0;

        switch (textureName)
        {
            case "Icon2":
                point = isAdd ? 7000 : -7000;
                Debug.Log("Icon2");
                break;
            case "Icon1":
                point = isAdd ? 8000 : -8000;
                Debug.Log("Icon1");
                break;
            case "Icon3":
                point = isAdd ? 6000 : -6000;
                Debug.Log("Icon3");
                break;
        }

        // ここで直接代入しないように修正
        yourChosePointNumber += point;
        updateOnly++;
        Debug.Log("YourChosePointNumber after addition: " + yourChosePointNumber);
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
            acquiresAbility.Remove(acquiresAbility.Find(item => item.Item1 == "ShootBulletContinuously" && item.Item2 == true));
            acquiresAbility.Remove(acquiresAbility.Find(item => item.Item1 == "ShootLaserBeamAsyncContinuously" && item.Item2 == true));
            acquiresAbility.Remove(acquiresAbility.Find(item => item.Item1 == "PhysicalEnhancement" && item.Item2 == true));
            droppedExtraAbility.ForEach((i) =>
                {
                    switch (i.Item1.texture.name)
                    {
                        case "Icon2":
                            acquiresAbility.Add(("ShootBulletContinuously", true, level2));
                            yourChosePointNumber += 7000;
                            break;
                        case "Icon1":
                            acquiresAbility.Add(("ShootLaserBeamAsyncContinuously", true, level2));
                            yourChosePointNumber += 8000;
                            break;
                        case "Icon3":
                            acquiresAbility.Add(("PhysicalEnhancement", true, level3));
                            yourChosePointNumber += 6000;
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
