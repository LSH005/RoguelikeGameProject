using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TitleManager : MonoBehaviour
{
    public GameObject TitleBox;
    public int numberOfCopies = 50;
    public GameObject backgroundObject;
    private SpriteRenderer backgroundSpriteRenderer;
    public Sprite[] backgroundSprites;
    public GameObject GamefadeoutObject;
    private SpriteRenderer fadeSpriteRenderer;
    public float fadeDuration = 2f;
    public GameObject UI;
    public GameObject wthImage;

    private bool PleaseStopItIAmScaredEveryonesGoingToDieLikeThisEveryoneEveryoneTheyAreGoingToDieIAmSoScaredStopItEveryonesGoingToDieLikeThis = false;
    // ↑ 어짜피 개인 프로젝트인데, 상관 없죠 교수님?
    // 교수님 사랑해요 ♥♥♥
    private int currentIndex = 0;
    private string NextScene;

    void Start()
    {
        wthImage.SetActive(false);

        backgroundSpriteRenderer = backgroundObject.GetComponent<SpriteRenderer>();
        fadeSpriteRenderer= GamefadeoutObject.GetComponent<SpriteRenderer>();

        backgroundSpriteRenderer.sprite = backgroundSprites[currentIndex];
        StartCoroutine(ChangeBackgroundRoutine());
    }

    IEnumerator ChangeBackgroundRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(1f, 5f);
            yield return new WaitForSeconds(waitTime);

            if (PleaseStopItIAmScaredEveryonesGoingToDieLikeThisEveryoneEveryoneTheyAreGoingToDieIAmSoScaredStopItEveryonesGoingToDieLikeThis)
            {
                break;
            }

            currentIndex++;
            if (currentIndex >= backgroundSprites.Length)
            {
                currentIndex = 0;
            }

            backgroundSpriteRenderer.sprite = backgroundSprites[currentIndex];

            for (int i = 0; i < numberOfCopies; i++)
            {
                GameObject spawnedObject = Instantiate(TitleBox);
                spawnedObject.name = TitleBox.name + "_" + i;
            }
        }
    }

    public void StartButton()
    {
        StartCoroutine(SceneFadeIn());
        NextScene = "LevelSelect";
    }

    public void GalleryButton()
    {
        StartCoroutine(SceneFadeIn());
        NextScene = "GalleryScene";
    }

    public void ExitButton()
    {
        StartCoroutine(ExitGame());
    }


    IEnumerator SceneFadeIn()
    {
        UI.SetActive(false);
        PleaseStopItIAmScaredEveryonesGoingToDieLikeThisEveryoneEveryoneTheyAreGoingToDieIAmSoScaredStopItEveryonesGoingToDieLikeThis = true;
        GamefadeoutObject.SetActive(true);
        for (int i = 0; i < 100; i++)
        {
            Color currentColor = fadeSpriteRenderer.color;
            currentColor.a += 0.01f;
            fadeSpriteRenderer.color = currentColor;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(NextScene);
    }
    
    IEnumerator ExitGame()
    {
        wthImage.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        Application.Quit();
        EditorApplication.isPlaying = false;
    }
}