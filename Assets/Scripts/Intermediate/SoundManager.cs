using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{

    public AudioSource audioSourceSE; // SE source
    public AudioClip[] audioClipsSE; // item


    public AudioSource audioSourceBGM; // BGM source
    public AudioClip[] audioClipsBGM; // item of BGM(0:Menu 1:Game)
    public static SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //Ensure that sound is not interrupted when transitioning between scenes
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // If this SoundManager is prefabricated elsewhere, remove it to prevent the same sound from being played back and creating a multiplicity of sounds.
            Destroy(this.gameObject);
        }
    }


    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }
    public void PlayBGM(int sceneIndex)
    {
        float fadeOutDuration = 1f;
        audioSourceBGM.DOFade(0f, fadeOutDuration).OnComplete(() =>
        {
            // BGM 停止
            audioSourceBGM.Stop();

            // 新しい BGM を設定
            switch (sceneIndex)
            {
                default:
                case 0:
                    audioSourceBGM.clip = audioClipsBGM[0];
                    break;
                case 1:
                    audioSourceBGM.clip = audioClipsBGM[1];
                    break;
            }

            // BGM フェードイン
            float fadeInDuration = 1f;
            audioSourceBGM.volume = 0f;
            audioSourceBGM.Play();
            audioSourceBGM.DOFade(1f, fadeInDuration);
        });
    }

    public void PlaySE(int index)
    {
        audioSourceSE.PlayOneShot(audioClipsSE[index]); // SEを一度だけ鳴らす
    }

}
