using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용한다면 이 네임스페이스를 추가하세요.
using System.Collections; // Coroutine을 사용하려면 필요합니다.

public class DialogueManager : MonoBehaviour
{
    // 싱글톤 인스턴스 (어디서든 DialogueManager.Instance로 접근 가능)
    public static DialogueManager Instance { get; private set; }

    // 현재 재생 중인 DialogueSO 에셋
    public DialogueSO currentDialogue;

    // UI 요소 연결 (인스펙터에서 할당)
    [Header("UI Components")]
    public GameObject dialogueUIPanel; // Dialogue UI 전체를 감싸는 패널 (활성화/비활성화용)
    public Image characterImageView;   // 캐릭터 이미지를 표시할 Image 컴포넌트
    public TMP_Text characterNameText;  // 캐릭터 이름을 표시할 TextMeshPro 또는 Text 컴포넌트
    public TMP_Text dialogueText;       // 대화 텍스트를 표시할 TextMeshPro 또는 Text 컴포넌트

    [Header("Dialogue Settings")]
    public float textSpeed = 0.05f;   // 텍스트가 한 글자씩 나타나는 속도

    // 내부 상태 관리 변수
    private int currentDialogueIndex = 0; // 현재 대화 배열의 인덱스
    private bool isTyping = false;        // 현재 텍스트가 타이핑 중인지 여부
    private string currentDialogueLine;  // 현재 출력할 대사의 전체 내용 (골뱅이 제외)
    private bool isAutoAdvancing = false; // 현재 대사가 자동 진행되어야 하는지 여부

    // 사운드 재생용 AudioSource 컴포넌트
    private AudioSource dialogueAudioSource;

    private void Awake()
    {
        // 싱글톤 인스턴스 초기화 및 유일성 보장
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 이미 다른 인스턴스가 있으면 자신을 파괴
            return;
        }
        Instance = this; // 현재 인스턴스를 유일한 인스턴스로 설정

        // 씬 전환 시에도 이 오브젝트를 유지할지 여부 (필요에 따라 주석 해제)
        // DontDestroyOnLoad(gameObject);

        // AudioSource 컴포넌트 가져오기 또는 추가
        dialogueAudioSource = GetComponent<AudioSource>();
        if (dialogueAudioSource == null)
        {
            Debug.LogWarning("DialogueManager에 AudioSource 컴포넌트가 없습니다. 자동으로 추가합니다.");
            dialogueAudioSource = gameObject.AddComponent<AudioSource>();
        }
        dialogueAudioSource.playOnAwake = false; // Awake 시 자동 재생 방지
    }

    private void Start()
    {
        // 게임 시작 시 Dialogue UI를 일단 비활성화하여 숨김
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(false);
        }

        // --- 테스트를 위해 게임 시작 시 바로 대화 시작 (실제 게임에서는 특정 트리거로 호출) ---
        // if (currentDialogue != null)
        // {
        //     StartDialogue();
        // }
        // ---
    }

    // 매 프레임마다 입력 확인
    void Update()
    {
        // 스페이스 바 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // 텍스트가 타이핑 중이라면, 즉시 전체 텍스트 출력
                StopAllCoroutines(); // 현재 진행 중인 타이핑 코루틴 중지
                dialogueText.text = currentDialogueLine; // 전체 텍스트 바로 표시
                isTyping = false; // 타이핑 상태 해제
            }
            else
            {
                // 텍스트 출력이 완료되었거나 타이핑 중이 아니라면, 다음 대사로 진행
                DisplayNextLine();
            }
        }
    }

    /// <summary>
    /// 새로운 대화 시퀀스를 시작합니다.
    /// </summary>
    /// <param name="dialogue">재생할 DialogueSO 에셋 (선택 사항, null이면 현재 currentDialogue 사용)</param>
    public void StartDialogue(DialogueSO dialogue = null)
    {
        if (dialogue != null)
        {
            currentDialogue = dialogue;
        }

        // 유효한 DialogueSO가 없거나 대화 내용이 비어 있으면 대화 종료 처리
        if (currentDialogue == null || currentDialogue.DialogueText == null || currentDialogue.DialogueText.Length == 0)
        {
            Debug.LogWarning("유효한 DialogueSO가 없거나 대화 내용이 비어 있습니다. 대화를 종료합니다.");
            EndDialogue();
            return;
        }

        currentDialogueIndex = 0; // 대화 인덱스 초기화

        // 대화 시작 시 Dialogue UI 패널 활성화 (표시)
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(true);
        }

        // 개별 UI 요소들도 활성화 (패널이 활성화되면 보이지만, 명시적으로 제어)
        if (characterImageView != null) characterImageView.gameObject.SetActive(true);
        if (characterNameText != null) characterNameText.gameObject.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);

        // 첫 번째 대사 표시 시작
        DisplayNextLine();
    }

    /// <summary>
    /// 다음 대사 라인을 표시합니다.
    /// </summary>
    public void DisplayNextLine()
    {
        // 현재 진행 중인 모든 코루틴 중지 및 타이핑 상태 해제
        StopAllCoroutines();
        isTyping = false;

        // 더 이상 대사가 없으면 대화 종료 처리
        if (currentDialogueIndex >= currentDialogue.DialogueText.Length)
        {
            EndDialogue();
            return;
        }

        // 원본 대사 문자열 가져오기
        string originalDialogueLine = currentDialogue.DialogueText[currentDialogueIndex];

        // @@ 골뱅이*2 처리 및 자동 진행 설정
        if (originalDialogueLine.StartsWith("@@"))
        {
            isAutoAdvancing = true;
            currentDialogueLine = originalDialogueLine.Substring(2); // 골뱅이 제외한 문자열
        }
        else
        {
            isAutoAdvancing = false;
            currentDialogueLine = originalDialogueLine; // 원본 문자열 그대로 사용
        }

        // --- 캐릭터 목소리(SFX) 클립 가져오기 ---
        AudioClip voiceClipForThisLine = null;
        if (currentDialogue.sfxClips != null && currentDialogue.sfxClips.Length > currentDialogueIndex)
        {
            voiceClipForThisLine = currentDialogue.sfxClips[currentDialogueIndex];
        }
        // ---

        // 텍스트 타이핑 코루틴 시작, 목소리 클립을 함께 전달
        StartCoroutine(TypeDialogue(currentDialogueLine, voiceClipForThisLine));

        // 캐릭터 이미지 업데이트
        if (characterImageView != null)
        {
            if (currentDialogue.characterImage != null && currentDialogue.characterImage.Length > currentDialogueIndex)
            {
                characterImageView.sprite = currentDialogue.characterImage[currentDialogueIndex];
            }
            else
            {
                characterImageView.sprite = null; // 해당 인덱스에 이미지가 없으면 null로 설정
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
                characterNameText.text = ""; // 해당 인덱스에 이름이 없으면 빈 문자열로 설정
            }
        }

        currentDialogueIndex++; // 다음 대사 인덱스로 증가
    }

    /// <summary>
    /// 대화 텍스트를 한 글자씩 타이핑 효과로 표시합니다.
    /// </summary>
    /// <param name="line">표시할 대사 문자열</param>
    /// <param name="voiceClip">한 글자마다 재생할 목소리 클립</param>
    private IEnumerator TypeDialogue(string line, AudioClip voiceClip)
    {
        isTyping = true;
        dialogueText.text = ""; // 텍스트 표시 영역 초기화

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter; // 한 글자씩 추가

            // 전달받은 목소리 클립이 있고 AudioSource가 있으면 재생
            if (voiceClip != null && dialogueAudioSource != null)
            {
                dialogueAudioSource.PlayOneShot(voiceClip);
            }

            yield return new WaitForSeconds(textSpeed); // 글자 속도만큼 대기
        }
        isTyping = false; // 타이핑 완료

        // 현재 대사가 자동 진행 대사라면 (골뱅이로 시작했다면), 바로 다음 대사로 진행
        if (isAutoAdvancing)
        {
            DisplayNextLine();
        }
    }

    /// <summary>
    /// 대화 시퀀스를 종료합니다.
    /// </summary>
    public void EndDialogue()
    {
        Debug.Log("대화 종료");
        // 대화 종료 시 Dialogue UI 패널 비활성화 (숨김)
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(false);
        }

        // 개별 UI 요소들도 상태 초기화 (패널이 비활성화되면 보이지 않지만, 내부 상태 초기화)
        if (characterImageView != null) characterImageView.gameObject.SetActive(false);
        if (characterNameText != null) characterNameText.gameObject.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

        // 상태 변수들 초기화
        currentDialogue = null;
        currentDialogueIndex = 0;
        isAutoAdvancing = false;
        isTyping = false;

        // 대화 종료 후 필요한 추가 로직 (예: 게임 상태 변경, 씬 전환 등)
    }

    // --- 다른 스크립트에서 대화를 시작할 때 예시 ---
    // GameManager.Instance.StartDialogue(mySpecificDialogueSOAsset);
    // 또는
    // if (DialogueManager.Instance != null) {
    //     DialogueManager.Instance.StartDialogue(mySpecificDialogueSOAsset);
    // }
}