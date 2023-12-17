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
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SelfDestruct", 3f);
    }
    public void SelfDestruct()
    {
        Destroy(gameObject);
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
        transform.position += destination.normalized * speed * Time.deltaTime;
    }
}
