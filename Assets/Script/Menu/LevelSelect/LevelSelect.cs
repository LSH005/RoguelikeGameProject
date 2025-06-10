using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public GameObject GamefadeoutObject;
    private SpriteRenderer fadeSpriteRenderer;
    public GameObject[] levelImageObjects;

    private int selectedLevel = 0;
    private bool SelectionCompleted = false;
    private bool isOpningEnd = false;
    private string NextScene;
    private Vector3 TargetPosition;
    private float TargetRotationZ;
    private Camera thisCamera;

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
        fadeSpriteRenderer = GamefadeoutObject.GetComponent<SpriteRenderer>();

        TargetPosition = levelImageObjects[0].transform.position;
        TargetPosition = new Vector3(TargetPosition.x, TargetPosition.y, -10f);

        TargetRotationZ = levelImageObjects[0].transform.rotation.eulerAngles.z;
    }

    void Start()
    {
        StartCoroutine(SceneOpening());
    }

    void Update()
    {
        if (isOpningEnd)
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, 12 * Time.deltaTime);

            Quaternion currentRotation = transform.rotation;
            Quaternion targetQuaternion = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, TargetRotationZ);
            transform.rotation = Quaternion.Lerp(currentRotation, targetQuaternion, 12 * Time.deltaTime);

            if (!SelectionCompleted)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedLevel == 0)
                    {
                        return;
                    }
                    selectedLevel--;
                    Relocation(selectedLevel);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedLevel == levelImageObjects.Length - 1)
                    {
                        return;
                    }
                    selectedLevel++;
                    Relocation(selectedLevel);
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    NextScene = "MainMenu";
                    StartCoroutine(SceneEnding());
                }
            }
        }
    }

    void Relocation(int selectedLevel)
    {
        TargetPosition = levelImageObjects[selectedLevel].transform.position;
        TargetPosition = new Vector3(TargetPosition.x, TargetPosition.y, -10f);

        TargetRotationZ = levelImageObjects[selectedLevel].transform.rotation.eulerAngles.z;
    }

    IEnumerator SceneOpening()
    {
        GamefadeoutObject.gameObject.SetActive(true);
        Color startFadeColor = fadeSpriteRenderer.color;
        startFadeColor.a = 1f;
        fadeSpriteRenderer.color = startFadeColor;

        for (int i = 0; i < 100; i++)
        {
            Color currentColor = fadeSpriteRenderer.color;
            currentColor.a -= 0.01f;
            fadeSpriteRenderer.color = currentColor;
            yield return new WaitForSeconds(0.005f);
        }
        Color finalColor = fadeSpriteRenderer.color;
        finalColor.a = 0f;
        fadeSpriteRenderer.color = finalColor;
        GamefadeoutObject.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        float elapsed = 0f;
        float zoomDuration = 0.2f;
        float startSize = thisCamera.orthographicSize;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;
            thisCamera.orthographicSize = Mathf.Lerp(startSize, 7, t);
            yield return null;
        }
        thisCamera.orthographicSize = 7f;
        yield return new WaitForSeconds(0.25f);
        isOpningEnd = true;
    }

    IEnumerator SceneEnding()
    {
        SelectionCompleted=true;

        GamefadeoutObject.gameObject.SetActive(true);
        Color startFadeColor = fadeSpriteRenderer.color;
        startFadeColor.a = 0f;
        fadeSpriteRenderer.color = startFadeColor;

        for (int i = 0; i < 100; i++)
        {
            Color currentColor = fadeSpriteRenderer.color;
            currentColor.a += 0.01f;
            fadeSpriteRenderer.color = currentColor;
            yield return new WaitForSeconds(0.005f);
        }
        Color finalColor = fadeSpriteRenderer.color;
        finalColor.a = 1f;
        fadeSpriteRenderer.color = finalColor;

        SceneManager.LoadScene(NextScene);
    }
}