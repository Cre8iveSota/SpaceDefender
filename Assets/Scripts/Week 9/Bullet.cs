using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 destination;
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
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += destination.normalized * speed * Time.deltaTime;
    }
}
