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

    }

    public void SelfDisappear()
    {
        Debug.Log("move");
        elapsedTime = 0f;
        percentageComplete = 0f;
        transform.position = startPos;
        CanMove = false;
        StartCoroutine(WaitForReborn());
    }

    private IEnumerator WaitForReborn()
    {
        yield return new WaitForSeconds(5f);
        CanMove = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.gameObject.CompareTag("PlayerBullet") || other.transform.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log(other.transform.gameObject);
            other.transform.gameObject.GetComponent<Bullet>()?.SelfDestruct();
            SelfDisappear();
        }
    }
}
