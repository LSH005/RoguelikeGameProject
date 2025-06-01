using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    [Header("대화창 텍스트")]
    public TextMeshProUGUI DialogueText;
    public TextMeshProUGUI NameText;
    [Header("띨빵한 얼굴 스프라이트")]
    public GameObject FaceImage;
    public Sprite[] faceSprites;

    private int ActNumber = 0;
    private float DialogueSpeed = 0.05f;
    private string Dialogue;

    private Image imageComponent;

    private void Awake()
    {
        imageComponent = FaceImage.GetComponent<Image>();
    }

    void Start()
    {

    }

    private void Update()
    {
        
    }

    public void DialogueSet(string name, string dialogue, float speed, int spriteNumber, int soundNumber, bool isAutoSkip)
    {
        imageComponent.sprite = faceSprites[spriteNumber];
        NameText.text = name;
        Dialogue=dialogue;
    }

    public void DialogueClear()
    {
        NameText.text = "";
        DialogueText.text = "";
        imageComponent.sprite = null;
    }

    IEnumerator DialoguePlay()
    {
        yield return Sleep(DialogueSpeed);
    }

    IEnumerator Sleep(double SleepSeconds)
    {
        yield return new WaitForSeconds((float)SleepSeconds);
    }

}