using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private Transform originPoint;
    [SerializeField] private LineRenderer lineRenderer;


    public IEnumerator Shoot(Vector3 rotationDirection)
    {
        SoundManager.instance.PlaySE(1);
        Vector2 laserDirection = new Vector2(rotationDirection.x, rotationDirection.y).normalized;
        // Create a variable that stores a raycast and  shoots it from origin in a forward direction
        RaycastHit2D hitInfo = Physics2D.Raycast(originPoint.position, laserDirection);
        if (hitInfo)
        {
            lineRenderer.SetPosition(0, originPoint.position);
            lineRenderer.SetPosition(1, hitInfo.point);
            GameObject hitObject = hitInfo.collider.gameObject;

            // Check if the object hit has an Enemy component
            Enemy enemy = hitObject.GetComponent<Enemy>();
            // bullet check
            Bullet bullet = hitObject.GetComponent<Bullet>();

            Boss boss = hitObject.GetComponent<Boss>();


            AstroLerp astroLerp = hitObject.GetComponent<AstroLerp>();
            enemy?.SelfDestruct();
            bullet?.SelfDestruct();
            astroLerp?.SelfDisappear();
            boss?.UpdateHealth(-2);
        }
        else
        {
            lineRenderer.SetPosition(0, originPoint.position);
            lineRenderer.SetPosition(1, (Vector2)originPoint.position + laserDirection * 100);
        }
        lineRenderer.enabled = true;
        // yield return new WaitForSeconds(0.02f);
        yield return new WaitForSeconds(0.02f);
        lineRenderer.enabled = false;
    }

}
