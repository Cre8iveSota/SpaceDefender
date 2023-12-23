using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoneOneShootBulletExplosion : MonoBehaviour
{
    public bool isFinish;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isFinish) StartCoroutine(WaitAnimation(0.1f));
    }

    private IEnumerator WaitAnimation(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
