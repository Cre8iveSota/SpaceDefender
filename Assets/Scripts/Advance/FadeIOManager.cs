using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeIOManager : MonoBehaviour
{
    // シングルトン化
    public static FadeIOManager instane;
    private void Awake()
    {
        if (instane == null)
        {
            instane = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float fadeTime = 1f;
    public CanvasGroup canvasGroup;

    void FadeOuut()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1, fadeTime).OnComplete(() => canvasGroup.blocksRaycasts = false);
    }
    void FadeIn()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(0, fadeTime).OnComplete(() => canvasGroup.blocksRaycasts = false);
    }

    public void FadeOutToIn(TweenCallback action)
    {
        Debug.Log("FadeOutToIn called");

        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1, fadeTime).OnKill(() =>
        {
            Debug.Log("FadeOut complete");

            action();
            FadeIn();
        });
    }
}
