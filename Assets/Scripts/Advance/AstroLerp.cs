using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroLerp : MonoBehaviour
{
    // Where lerpy starts and where thre're going
    private Vector2 startPos;
    private Camera cam;
    private float camWidth;
    private Vector2 targetPos;
    [SerializeField] private float decelerationPram = 5f;
    // How long lerpy has been traveling
    private float elapsedTime;
    // What percentage of Lerpy's journey is complete, display as a slider the Inspector
    [SerializeField][Range(0f, 1f)] private float percentageComplete;
    // Start is called before the first frame update

    [SerializeField] private GameObject astroExplode;
    private GameObject astroExplodePrefab;
    bool CanMove = true;

    void Start()
    {

        startPos = transform.position;
        cam = Camera.main;
        // Orthographic size = cam view height x 2 (-5/5)
        float camHeight = cam.orthographicSize;
        // Cam width = ortho * aspect
        camWidth = camHeight * cam.aspect;
        targetPos = new Vector2(-camWidth, startPos.y);

        // targetPos = new Vector2(-2 * playerController.ScreenWidth, startPos.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            // Increments how long lerpy has been traveling
            elapsedTime += Time.deltaTime;
            // get the progress of Lerpy's journey as a %
            /* 
                In order to give the meteorites different speeds, the speed is changed according to the distance 
                from the initial position of the meteorite to the left edge of the screen
               , which makes the meteorites appear to have an irregular cycle. 
            */
            percentageComplete = elapsedTime / (decelerationPram + startPos.x - camWidth);
            // Use that percentage to get the position of Lerpy between the start and target
            transform.position = Vector2.Lerp(startPos, targetPos, percentageComplete);
            if (percentageComplete >= 1)
            {
                elapsedTime = 0f;
                percentageComplete = 0f;
            }
        }
        if (elapsedTime == 0 && percentageComplete == 0)
        {
            transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * UnityEngine.Random.Range(0.5f, 2f);
        }
    }

    public void SelfDisappear()
    {
        Vector3 lastPostion = transform.position;
        if (astroExplode) { astroExplodePrefab = Instantiate(astroExplode, lastPostion, Quaternion.identity); }
        elapsedTime = 0f;
        percentageComplete = 0f;
        transform.position = startPos;
        CanMove = false;
        if (astroExplodePrefab)
        {
            SoundManager.instance.PlaySE(2);
            StartCoroutine(WaitForExplodeAnimation(astroExplodePrefab));
        }
        StartCoroutine(WaitForReborn());
    }

    private IEnumerator WaitForReborn()
    {
        yield return new WaitForSeconds(5f);
        CanMove = true;
    }

    private IEnumerator WaitForExplodeAnimation(GameObject astroPrefab)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(astroPrefab);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.gameObject.CompareTag("PlayerBullet") || other.transform.gameObject.CompareTag("EnemyBullet"))
        {
            other.transform.gameObject.GetComponent<Bullet>()?.SelfDestruct();
            SelfDisappear();
        }
    }
}
