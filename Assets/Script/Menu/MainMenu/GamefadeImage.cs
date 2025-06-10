using System.Collections;
using UnityEngine;

public class GamefadeImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject UI;

    void Start()
    {
        UI.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();

        Color currentColor = spriteRenderer.color;
        currentColor.a = 1f;
        spriteRenderer.color = currentColor;

        StartCoroutine(fadeout());
    }

    IEnumerator fadeout()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 100; i++)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a -= 0.01f;
            spriteRenderer.color = currentColor;

            yield return new WaitForSeconds(0.02f);
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;
        gameObject.SetActive(false);
        UI.SetActive(true);
    }
}