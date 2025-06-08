using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용한다면 이 네임스페이스를 추가하세요.
using System.Collections; // Coroutine을 사용하려면 필요합니다.

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public DialogueSO currentDialogue;

    // UI 요소 연결 (인스펙터에서 할당)
    [Header("UI Components")]
    public GameObject dialogueUIPanel; // 추가: Dialogue UI의 부모 오브젝트
    public Image characterImageView;
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public float textSpeed = 0.05f;

    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private string currentDialogueLine;
    private bool isAutoAdvancing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 씬 전환 시 유지 (선택 사항)
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 시작 시 Dialogue UI를 일단 비활성화 (대화 시작 전에는 숨겨져 있어야 함)
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(false);
        }

        //테스트를 위해 게임 시작 시 대화 시작(실제 게임에서는 조건부로 호출)
        //if (currentDialogue != null)
        //{
        //    StartDialogue();
        //}
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentDialogueLine;
                isTyping = false;
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    public void StartDialogue(DialogueSO dialogue = null)
    {
        if (dialogue != null)
        {
            currentDialogue = dialogue;
        }

        if (currentDialogue == null || currentDialogue.DialogueText.Length == 0)
        {
            Debug.LogWarning("유효한 DialogueSO가 없거나 대화 내용이 비어 있습니다.");
            EndDialogue(); // 유효하지 않으면 바로 종료 처리
            return;
        }

        currentDialogueIndex = 0;

        // --- 수정된 부분: 대화 시작 시 Dialogue UI 활성화 ---
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(true);
        }
        // ---

        // 나머지 개별 UI 요소들도 활성화 (dialogueUIPanel이 활성화되면 자식들도 보임)
        // 다만, 만약을 위해 캐릭터 이미지나 이름이 없는 대사가 있을 경우를 대비하여 활성화 상태를 유지하는 것이 좋습니다.
        if (characterImageView != null) characterImageView.gameObject.SetActive(true);
        if (characterNameText != null) characterNameText.gameObject.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);

        DisplayNextLine(); // 첫 번째 대사 표시
    }

    public void DisplayNextLine()
    {
        StopAllCoroutines();
        isTyping = false;

        if (currentDialogueIndex >= currentDialogue.DialogueText.Length)
        {
            EndDialogue(); // 더 이상 대사가 없으면 대화 종료
            return;
        }

        string originalDialogueLine = currentDialogue.DialogueText[currentDialogueIndex];

        if (originalDialogueLine.StartsWith("@"))
        {
            isAutoAdvancing = true;
            currentDialogueLine = originalDialogueLine.Substring(1);
        }
        else
        {
            isAutoAdvancing = false;
            currentDialogueLine = originalDialogueLine;
        }

        StartCoroutine(TypeDialogue(currentDialogueLine));

        // 캐릭터 이미지 업데이트
        if (characterImageView != null)
        {
            if (currentDialogue.characterImage != null && currentDialogue.characterImage.Length > currentDialogueIndex)
            {
                characterImageView.sprite = currentDialogue.characterImage[currentDialogueIndex];
            }
            else
            {
                characterImageView.sprite = null;
            }
        }

        // 캐릭터 이름 업데이트
        if (characterNameText != null)
        {
            if (currentDialogue.characterName != null && currentDialogue.characterName.Length > currentDialogueIndex)
            {
                characterNameText.text = currentDialogue.characterName[currentDialogueIndex];
            }
            else
            {
                characterNameText.text = "";
            }
        }

        // 캐릭터 목소리(SFX) 재생
        if (currentDialogue.sfxClips != null && currentDialogue.sfxClips.Length > currentDialogueIndex)
        {
            AudioClip currentVoiceClip = currentDialogue.sfxClips[currentDialogueIndex];
            if (currentVoiceClip != null)
            {
                // AudioSource 컴포넌트를 DialogueManager에 추가한 후 사용
                // GetComponent<AudioSource>().PlayOneShot(currentVoiceClip);
                Debug.Log($"'{currentVoiceClip.name}' 목소리 재생!");
            }
        }

        currentDialogueIndex++;
    }

    private IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;

        if (isAutoAdvancing)
        {
            DisplayNextLine();
        }
    }

    public void EndDialogue()
    {
        Debug.Log("대화 종료");
        // --- 수정된 부분: 대화 종료 시 Dialogue UI 비활성화 ---
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(false);
        }
        // ---

        // 나머지 UI 요소들은 부모가 비활성화되면 자동으로 사라지지만, 상태 초기화를 위해 명시적으로 해두는 것도 좋음.
        if (characterImageView != null) characterImageView.gameObject.SetActive(false);
        if (characterNameText != null) characterNameText.gameObject.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

        currentDialogue = null;
        currentDialogueIndex = 0;
        isAutoAdvancing = false;
        isTyping = false;
        // 대화 종료 후 필요한 로직 추가
    }
}