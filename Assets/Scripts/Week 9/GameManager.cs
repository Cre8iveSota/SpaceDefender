using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool isGameOver;
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text clearTimeText;
    [SerializeField] private TMP_Text clearScoreText;
    [SerializeField] private TMP_Text totalSelectivePointText;
    [SerializeField] private TMP_Text yourChosePointText;
    [SerializeField] private TMP_Text cautionText;
    [SerializeField] private GameObject cautionBelowPointText;
    [SerializeField] private GameObject dropArea1Text;
    [SerializeField] private GameObject dropArea2Text;
    [SerializeField] private GameObject dropArea3Text;
    [SerializeField] private TMP_Text dropArea1Explaination;
    [SerializeField] private TMP_Text dropArea2Explaination;
    [SerializeField] private TMP_Text dropArea3Explaination;
    [SerializeField] private GameObject bossParticle;



    private int sceneIndex;
    private Camera cam;
    private ScreenShake screenShake;
    private GameObject player;
    private PlayerController playerController;
    public static int totalamount = 0;

    public static List<(Sprite, string)> droppedExtraAbility = new List<(Sprite, string)>();
    private List<(Sprite, string)> pastDroppedExtraAbility = new List<(Sprite, string)>();
    public static List<(string, bool, int)> acquiresAbility = new List<(string, bool, int)>();
    private int level1 = 1;
    private int level2 = 2;
    private int level3 = 3;
    private DropMe[] dropMe = new DropMe[3];

    private int yourChosePointNumber;

    private bool isExecuting = false;


    private float elapsedTime;
    public static float clearElapsedTime;
    public bool isGameClear;
    Dictionary<string, int> choseSkillCategory = new Dictionary<string, int>();
    Dictionary<string, string> choseSkillExplainationText = new Dictionary<string, string>();



    // Start is called before the first frame update
    void Start()
    {
        if (dropArea1Text && dropArea2Text && dropArea3Text)
        {
            dropMe[0] = dropArea1Text.GetComponent<DropMe>();
            dropMe[1] = dropArea2Text.GetComponent<DropMe>();
            dropMe[2] = dropArea3Text.GetComponent<DropMe>();
        }

        yourChosePointNumber = 0;
        droppedExtraAbility.Clear();

        choseSkillCategory["PhysicalEnhancement"] = 0;
        choseSkillCategory["NaturalHealing"] = 0;
        choseSkillCategory["Bullet"] = 0;
        choseSkillCategory["Laser"] = 0;
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
            Debug.Log("Player not found!");
        }
        if (sceneIndex == 3)
        {
            Instantiate(bossParticle, Boss.finalBossPosition, Quaternion.identity);
            int min = (int)(clearElapsedTime / 60);
            int sec = (int)(clearElapsedTime % 60);
            if (clearTimeText) { clearTimeText.text = $"Time: {min:00}:{sec:00}"; }
            if (clearScoreText) { clearScoreText.text = $"{totalamount.ToString()} pt"; }
            SoundManager.instance.PlaySE(2);
            Debug.Log("Cleared point: " + totalamount);
            Debug.Log("Cleared time: " + totalamount);
        }
    }

    void Update()
    {
        if (totalSelectivePointText) totalSelectivePointText.text = $"{totalamount}";
        if (yourChosePointText) yourChosePointText.text = $"{yourChosePointNumber}";

        // isExcuting フラグが解除されていない場合のみ Test メソッドを実行

        if (!AreListsEqual(pastDroppedExtraAbility, droppedExtraAbility) && !isExecuting)
        {
            ChooseItemsPointManagement();
        }
        if (!isGameOver)
        {

            elapsedTime += Time.deltaTime;
            int min = (int)(elapsedTime / 60);
            int sec = (int)(elapsedTime % 60);
            if (currentTimeText) { currentTimeText.text = $"Time: {min:00}:{sec:00}"; }
        }
        if (isGameClear)
        {
            clearElapsedTime = elapsedTime;
            SoundManager.instance.StopAllSE();
            SoundManager.instance.PlayClearBGM();
            isGameClear = false;
            StartCoroutine(GameresetIntervel());
        }
    }
    private void ChooseItemsPointManagement()
    {
        isExecuting = true;

        choseSkillCategory["PhysicalEnhancement"] = 0;
        choseSkillCategory["NaturalHealing"] = 0;
        choseSkillCategory["Bullet"] = 0;
        choseSkillCategory["Laser"] = 0;
        acquiresAbility.Clear();

        LatestExtraAbilityFilleter(droppedExtraAbility);


        yourChosePointNumber = 0;

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

        // item2の同じ項目のみfillterをかける
        string filterCondition1 = "Drop Area 1";
        tmp1 = list.Where(item => item.Item2 == filterCondition1).ToList();
        if (tmp1.Count > 0) tmpComp.Add(tmp1[tmp1.Count - 1]);

        string filterCondition2 = "Drop Area 2";
        tmp2 = list.Where(item => item.Item2 == filterCondition2).ToList();
        if (tmp2.Count > 0) tmpComp.Add(tmp2[tmp2.Count - 1]);

        string filterCondition3 = "Drop Area 3";
        tmp3 = list.Where(item => item.Item2 == filterCondition3).ToList();
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

            case "AircraftEnhanceLv1 1":
                point = isAdd ? 1500 : -1500;
                acquiresAbility.Add(("PhysicalEnhancement", true, level1));
                choseSkillCategory["PhysicalEnhancement"] = choseSkillCategory["PhysicalEnhancement"] + 1;
                break;
            case "AircraftEnhanceLv2":
                point = isAdd ? 3000 : -3000;
                acquiresAbility.Add(("PhysicalEnhancement", true, level2));
                choseSkillCategory["PhysicalEnhancement"] = choseSkillCategory["PhysicalEnhancement"] + 1;
                break;
            case "AircraftEnhanceLv3":
                point = isAdd ? 6000 : -6000;
                acquiresAbility.Add(("PhysicalEnhancement", true, level3));
                choseSkillCategory["PhysicalEnhancement"] = choseSkillCategory["PhysicalEnhancement"] + 1;
                break;
            case "SelfRepairLv1":
                point = isAdd ? 1000 : -1000;
                acquiresAbility.Add(("NaturalHealingAbility", true, level2));
                choseSkillCategory["NaturalHealing"] = choseSkillCategory["NaturalHealing"] + 1;
                break;
            case "SelfRepairLv2":
                point = isAdd ? 2500 : -2500;
                acquiresAbility.Add(("NaturalHealingAbility", true, level3));
                choseSkillCategory["NaturalHealing"] = choseSkillCategory["NaturalHealing"] + 1;
                break;
            case "BulletLv1":
                point = isAdd ? 5000 : -5000;
                acquiresAbility.Add(("ShootBulletContinuously", true, level1));
                choseSkillCategory["Bullet"] = choseSkillCategory["Bullet"] + 1;
                //Fires 10 times\nat 0.1-second intervals
                break;
            case "BulletLv2":
                point = isAdd ? 10000 : -10000;
                acquiresAbility.Add(("ShootBulletContinuously", true, level2));
                choseSkillCategory["Bullet"] = choseSkillCategory["Bullet"] + 1;
                //Fires 30 times\nat 0.05-second intervals
                break;
            case "LaserLv1":
                point = isAdd ? 3000 : -3000;
                acquiresAbility.Add(("ShootLaserBeamAsyncContinuously", true, level1));
                choseSkillCategory["Laser"] = choseSkillCategory["Laser"] + 1;
                //Fires 10 times\nat 0.3-second intervals
                break;
            case "LaserLv.2":
                point = isAdd ? 15000 : -15000;
                acquiresAbility.Add(("ShootLaserBeamAsyncContinuously", true, level2));
                choseSkillCategory["Laser"] = choseSkillCategory["Laser"] + 1;
                //Fires 50 times\nat 0.05-second intervals
                break;
        }

        // ここで直接代入しないように修正
        yourChosePointNumber += point;

        SetTextBasedOnDropArea(0, "Drop Area 1");
        SetTextBasedOnDropArea(1, "Drop Area 2");
        SetTextBasedOnDropArea(2, "Drop Area 3");
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
    public void LoadScene()
    {
        Debug.Log("LoadScene called");

        if (sceneIndex == 1 && !isGameClear)
        {
            sceneIndex = 2;
            SceneManager.LoadScene(sceneIndex);
            SoundManager.instance.PlayBGM(1);
        }
        else if (sceneIndex == 0)
        {
            isGameClear = false;
            sceneIndex++;
            FadeIOManager.instane.FadeOutToIn(() => SceneManager.LoadScene(sceneIndex));
            SoundManager.instance.PlayBGM(1);
        }
        else if (sceneIndex == 2)
        {
            if (yourChosePointNumber <= totalamount
             && choseSkillCategory["PhysicalEnhancement"] <= 1
             && choseSkillCategory["NaturalHealing"] <= 1
             && choseSkillCategory["Bullet"] <= 1
             && choseSkillCategory["Laser"] <= 1)
            {
                sceneIndex = 0;
                FadeIOManager.instane.FadeOutToIn(() => SceneManager.LoadScene(1));
                yourChosePointNumber = 0;
            }
            else if (choseSkillCategory["PhysicalEnhancement"] > 1
             || choseSkillCategory["NaturalHealing"] > 1
             || choseSkillCategory["Bullet"] > 1
             || choseSkillCategory["Laser"] > 1)
            {
                StartCoroutine(CautionForSkillTypeDuplicate());
            }
            else
            {
                StartCoroutine(CautionBelowPoints());
            }
        }
        else if (sceneIndex == 1 && isGameClear)
        {
            SoundManager.instance.PlaySE(2);
            FadeIOManager.instane.FadeOutToIn(() => SceneManager.LoadScene(3));
        }
        else if (sceneIndex == 3)
        {
            sceneIndex = 2;
            FadeIOManager.instane.FadeOutToIn(() => SceneManager.LoadScene(sceneIndex));
            SoundManager.instance.PlayClearBGM();
        }
        else
        {
            sceneIndex = 0;
            FadeIOManager.instane.FadeOutToIn(() => SceneManager.LoadScene(sceneIndex));
            SoundManager.instance.PlayBGM(0);
        }
    }

    private IEnumerator CautionForSkillTypeDuplicate()
    {
        Color preCautionText = cautionText.color;
        cautionText.color = new Color(1f, 1f, 0f);
        yield return new WaitForSeconds(3f);
        cautionText.color = preCautionText;
    }

    private IEnumerator CautionBelowPoints()
    {
        Color preColor = cautionText.color;
        cautionText.color = new Color(1f, 1f, 0f, 0f);
        cautionBelowPointText.SetActive(true);
        yield return new WaitForSeconds(3f);
        cautionBelowPointText.SetActive(false);
        cautionText.color = preColor;
    }

    public void ChooseAgainReLoad()
    {
        droppedExtraAbility.Clear();
        sceneIndex = 2;
        SceneManager.LoadScene(sceneIndex);
    }
    public void LoadSceneRestart()
    {
        acquiresAbility.Clear();
        sceneIndex = 0;
        SceneManager.LoadScene(0);
    }

    private void AddAbilities(List<(string, bool, int)> acqwireAbility, string abilityNameToUpdate, bool newBoolValue, int newLevelValue)
    {
        // 条件に合致する要素のインデックスを取得
        int indexToUpdate = acqwireAbility.FindIndex(item => item.Item1 == abilityNameToUpdate);

        // 条件に合致する要素が見つかった場合、その要素を更新
        if (indexToUpdate != -1)
        {
            acqwireAbility[indexToUpdate] = (abilityNameToUpdate, newBoolValue, newLevelValue);
        }
    }


    private void SetTextBasedOnDropArea(int dropAreaIndex, string dropAreaName)
    {
        choseSkillExplainationText["AircraftEnhanceLv1 1"] = "Normal Defence × 2\nNormal Speed × 2";
        choseSkillExplainationText["AircraftEnhanceLv2"] = "Normal Defence × 3\nNormal Speed × 3";
        choseSkillExplainationText["AircraftEnhanceLv3"] = "Normal Defence × 4\nNormal Speed × 4";
        choseSkillExplainationText["SelfRepairLv1"] = "Heals a bit\napproximately 6 seconds";
        choseSkillExplainationText["SelfRepairLv2"] = "Heals significantly\napproximately every 5 seconds";
        choseSkillExplainationText["BulletLv1"] = "Fires 10 times\nat 0.1-second intervals";
        choseSkillExplainationText["BulletLv2"] = "Fires 30 times\nat 0.05-second intervals";
        choseSkillExplainationText["LaserLv1"] = "Fires 10 times\nat 0.3-second intervals";
        choseSkillExplainationText["LaserLv.2"] = "Fires 50 times\nat 0.05-second intervals";

        int seekingIndex = droppedExtraAbility.FindIndex((i) => i.Item2.Contains(dropAreaName));

        if (seekingIndex != -1)
        {
            string targetDropItems = droppedExtraAbility[seekingIndex].Item1.texture.name;
            string afterJudge = GetAfterJudgeValue(targetDropItems);
            dropMe[dropAreaIndex].textObject.text = afterJudge;

            if (dropAreaName == "Drop Area 1") dropArea1Explaination.text = choseSkillExplainationText.FirstOrDefault(i => i.Key == droppedExtraAbility[seekingIndex].Item1.texture.name).Value;
            if (dropAreaName == "Drop Area 2") dropArea2Explaination.text = choseSkillExplainationText.FirstOrDefault(i => i.Key == droppedExtraAbility[seekingIndex].Item1.texture.name).Value;
            if (dropAreaName == "Drop Area 3") dropArea3Explaination.text = choseSkillExplainationText.FirstOrDefault(i => i.Key == droppedExtraAbility[seekingIndex].Item1.texture.name).Value;
        }
    }

    private string GetAfterJudgeValue(string targetDropItems)
    {
        return targetDropItems.Contains("Aircra") ? "AirframeEnhancement" :
               targetDropItems.Contains("Self") ? "Self-Repair" :
               targetDropItems.Contains("Bullet") ? "Bullet" :
               targetDropItems.Contains("Laser") ? "Laser" :
               "not found";
    }
}
