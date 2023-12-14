using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 1f;
    public bool isShaking = false;
    SpriteRenderer spriteRenderer;
    private Color damageColor = new Color(1f, 0.647f, 0.647f, 0.784f);
    private Color initialColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            Debug.LogWarning("spriteRenderer not found!");
        }
        else
        {
            initialColor = spriteRenderer.color;
        }
    }
    void Update()
    {
        if (isShaking)
        {
            isShaking = false;
            StartCoroutine(DamageShakeCamera());
        }

    }

    private IEnumerator DamageShakeCamera()
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration && Time.timeScale != 0)
        {
            elapsedTime += Time.deltaTime;
            transform.position = startPos + Random.insideUnitSphere;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = damageColor;
            }
            else
            {
                Debug.LogWarning("SpriteRenderer component not found. Color change failed.");
            }

            yield return null;
        }
        transform.position = startPos;
        spriteRenderer.color = initialColor;

    }
}
