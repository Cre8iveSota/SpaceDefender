using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 destination;
    private Vector3 direction;
    private float angle;
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject explodeAnimationPrefab;
    // Start is called before the first frame update
    private bool isBulletStop;
    DoneOneShootBulletExplosion doneOneShootBulletExplosion;
    private bool isHit = false;

    void Start()
    {
        Invoke("SelfDestruct", 3f);
        if (explodeAnimationPrefab) doneOneShootBulletExplosion = explodeAnimationPrefab.GetComponent<DoneOneShootBulletExplosion>();
    }
    public void SelfDestruct()
    {
        if (!gameObject.CompareTag("Player") && isHit)
        {
            SoundManager.instance.PlaySE(4);
        }
        Vector3 lastPosition = transform.position;
        if (explodeAnimationPrefab)
        {
            GameObject explodePreTemp = Instantiate(explodeAnimationPrefab, lastPosition, Quaternion.identity);
            if (explodePreTemp) StartCoroutine(WaitForAnimation(0.15f, explodePreTemp));
        }
    }
    public void SetDestination(Vector3 point)
    {
        SoundManager.instance.PlaySE(0);
        destination = point;

        Vector3 offset = new Vector3(100f, 100f, 0f);
        if (destination.x < 0) { offset.x = -offset.x; }
        if (destination.y < 0) { offset.x = -offset.y; }

        // 計算された回転をtransform.upに設定
        angle = Mathf.Atan2(destination.y, destination.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBulletStop) transform.position += destination.normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if
        (
            (other.gameObject.CompareTag("PlayerBullet") && this.gameObject.CompareTag("EnemyBullet"))
                    || (other.gameObject.CompareTag("EnemyBullet") && this.gameObject.CompareTag("PlayerBullet"))
        )
        {
            isHit = true;
            SelfDestruct();
        }
    }

    private IEnumerator WaitForAnimation(float second, GameObject prefab)
    {
        isBulletStop = true;
        yield return new WaitForSeconds(second);
        doneOneShootBulletExplosion.isFinish = true;
        Destroy(prefab);
        Destroy(gameObject);
    }
}