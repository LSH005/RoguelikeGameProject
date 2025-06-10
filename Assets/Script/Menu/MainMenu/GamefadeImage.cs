using System.Collections;
using UnityEngine;

public class GamefadeImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject UI;

    private void Awake()
    {
        UI.SetActive(false);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Color currentColor = spriteRenderer.color;
        currentColor.a = 1f;
        spriteRenderer.color = currentColor;

        StartCoroutine(fadeout());
    }

    IEnumerator fadeout()
    {
        yield return new WaitForSeconds(1.0f);

        float duration = 1.5f;
        float startAlpha = spriteRenderer.color.a;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Color currentColor = spriteRenderer.color;
            currentColor.a = Mathf.Lerp(startAlpha, 0f, t);
            spriteRenderer.color = currentColor;

            yield return null;
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;

        gameObject.SetActive(false);
        UI.SetActive(true);
    }
}