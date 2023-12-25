using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanelController : MonoBehaviour
{

    [SerializeField] private Sprite[] titleImages = new Sprite[7];
    [SerializeField] private float[] eachIntervalManagerBySprite = { 2f, 0.2f, 0.2f, 0.2f, 0.1f, 0.1f, 0.1f };
    private Image displayImage;
    private bool isPlay = false;

    void Start()
    {
        displayImage = GetComponent<Image>();
    }
    void Update()
    {
        if (!isPlay)
        {
            StartCoroutine(PlayImagesLikeGif());
        }
    }

    private IEnumerator PlayImagesLikeGif()
    {
        isPlay = true;
        Debug.Log("Start Invoke");
        for (int i = 0; i < titleImages.Length; i++)
        {
            ImagePlayer(titleImages[i]);
            yield return new WaitForSeconds(eachIntervalManagerBySprite[i]);
        }

        yield return new WaitForSeconds(3f);
        isPlay = false;
    }

    private void ImagePlayer(Sprite image)
    {
        displayImage.sprite = image;
    }
}
