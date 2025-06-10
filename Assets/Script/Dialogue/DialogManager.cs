using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����Ѵٸ� �� ���ӽ����̽��� �߰��ϼ���.
using System.Collections; // Coroutine�� ����Ϸ��� �ʿ��մϴ�.

public class DialogueManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ� (��𼭵� DialogueManager.Instance�� ���� ����)
    public static DialogueManager Instance { get; private set; }

    // ���� ��� ���� DialogueSO ����
    public DialogueSO currentDialogue;

    // UI ��� ���� (�ν����Ϳ��� �Ҵ�)
    [Header("UI Components")]
    public GameObject dialogueUIPanel; // Dialogue UI ��ü�� ���δ� �г� (Ȱ��ȭ/��Ȱ��ȭ��)
    public Image characterImageView;   // ĳ���� �̹����� ǥ���� Image ������Ʈ
    public TMP_Text characterNameText;  // ĳ���� �̸��� ǥ���� TextMeshPro �Ǵ� Text ������Ʈ
    public TMP_Text dialogueText;       // ��ȭ �ؽ�Ʈ�� ǥ���� TextMeshPro �Ǵ� Text ������Ʈ

    [Header("Dialogue Settings")]
    public float textSpeed = 0.05f;   // �ؽ�Ʈ�� �� ���ھ� ��Ÿ���� �ӵ�

    // ���� ���� ���� ����
    private int currentDialogueIndex = 0; // ���� ��ȭ �迭�� �ε���
    private bool isTyping = false;        // ���� �ؽ�Ʈ�� Ÿ���� ������ ����
    private string currentDialogueLine;  // ���� ����� ����� ��ü ���� (����� ����)
    private bool isAutoAdvancing = false; // ���� ��簡 �ڵ� ����Ǿ�� �ϴ��� ����

    // ���� ����� AudioSource ������Ʈ
    private AudioSource dialogueAudioSource;

    private void Awake()
    {
        // �̱��� �ν��Ͻ� �ʱ�ȭ �� ���ϼ� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �̹� �ٸ� �ν��Ͻ��� ������ �ڽ��� �ı�
            return;
        }
        Instance = this; // ���� �ν��Ͻ��� ������ �ν��Ͻ��� ����

        // �� ��ȯ �ÿ��� �� ������Ʈ�� �������� ���� (�ʿ信 ���� �ּ� ����)
        // DontDestroyOnLoad(gameObject);

        // AudioSource ������Ʈ �������� �Ǵ� �߰�
        dialogueAudioSource = GetComponent<AudioSource>();
        if (dialogueAudioSource == null)
        {
            Debug.LogWarning("DialogueManager�� AudioSource ������Ʈ�� �����ϴ�. �ڵ����� �߰��մϴ�.");
            dialogueAudioSource = gameObject.AddComponent<AudioSource>();
        }
        dialogueAudioSource.playOnAwake = false; // Awake �� �ڵ� ��� ����
    }

    private void Start()
    {
        // ���� ���� �� Dialogue UI�� �ϴ� ��Ȱ��ȭ�Ͽ� ����
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(false);
        }

        // --- �׽�Ʈ�� ���� ���� ���� �� �ٷ� ��ȭ ���� (���� ���ӿ����� Ư�� Ʈ���ŷ� ȣ��) ---
        // if (currentDialogue != null)
        // {
        //     StartDialogue();
        // }
        // ---
    }

    // �� �����Ӹ��� �Է� Ȯ��
    void Update()
    {
        // �����̽� �� �Է� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // �ؽ�Ʈ�� Ÿ���� ���̶��, ��� ��ü �ؽ�Ʈ ���
                StopAllCoroutines(); // ���� ���� ���� Ÿ���� �ڷ�ƾ ����
                dialogueText.text = currentDialogueLine; // ��ü �ؽ�Ʈ �ٷ� ǥ��
                isTyping = false; // Ÿ���� ���� ����
            }
            else
            {
                // �ؽ�Ʈ ����� �Ϸ�Ǿ��ų� Ÿ���� ���� �ƴ϶��, ���� ���� ����
                DisplayNextLine();
            }
        }
    }

    /// <summary>
    /// ���ο� ��ȭ �������� �����մϴ�.
    /// </summary>
    /// <param name="dialogue">����� DialogueSO ���� (���� ����, null�̸� ���� currentDialogue ���)</param>
    public void StartDialogue(DialogueSO dialogue = null)
    {
        if (dialogue != null)
        {
            currentDialogue = dialogue;
        }

        // ��ȿ�� DialogueSO�� ���ų� ��ȭ ������ ��� ������ ��ȭ ���� ó��
        if (currentDialogue == null || currentDialogue.DialogueText == null || currentDialogue.DialogueText.Length == 0)
        {
            Debug.LogWarning("��ȿ�� DialogueSO�� ���ų� ��ȭ ������ ��� �ֽ��ϴ�. ��ȭ�� �����մϴ�.");
            EndDialogue();
            return;
        }

        currentDialogueIndex = 0; // ��ȭ �ε��� �ʱ�ȭ

        // ��ȭ ���� �� Dialogue UI �г� Ȱ��ȭ (ǥ��)
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(true);
        }

        // ���� UI ��ҵ鵵 Ȱ��ȭ (�г��� Ȱ��ȭ�Ǹ� ��������, ��������� ����)
        if (characterImageView != null) characterImageView.gameObject.SetActive(true);
        if (characterNameText != null) characterNameText.gameObject.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);

        // ù ��° ��� ǥ�� ����
        DisplayNextLine();
    }

    /// <summary>
    /// ���� ��� ������ ǥ���մϴ�.
    /// </summary>
    public void DisplayNextLine()
    {
        // ���� ���� ���� ��� �ڷ�ƾ ���� �� Ÿ���� ���� ����
        StopAllCoroutines();
        isTyping = false;

        // �� �̻� ��簡 ������ ��ȭ ���� ó��
        if (currentDialogueIndex >= currentDialogue.DialogueText.Length)
        {
            EndDialogue();
            return;
        }

        // ���� ��� ���ڿ� ��������
        string originalDialogueLine = currentDialogue.DialogueText[currentDialogueIndex];

        // @@ �����*2 ó�� �� �ڵ� ���� ����
        if (originalDialogueLine.StartsWith("@@"))
        {
            isAutoAdvancing = true;
            currentDialogueLine = originalDialogueLine.Substring(2); // ����� ������ ���ڿ�
        }
        else
        {
            isAutoAdvancing = false;
            currentDialogueLine = originalDialogueLine; // ���� ���ڿ� �״�� ���
        }

        // --- ĳ���� ��Ҹ�(SFX) Ŭ�� �������� ---
        AudioClip voiceClipForThisLine = null;
        if (currentDialogue.sfxClips != null && currentDialogue.sfxClips.Length > currentDialogueIndex)
        {
            voiceClipForThisLine = currentDialogue.sfxClips[currentDialogueIndex];
        }
        // ---

        // �ؽ�Ʈ Ÿ���� �ڷ�ƾ ����, ��Ҹ� Ŭ���� �Բ� ����
        StartCoroutine(TypeDialogue(currentDialogueLine, voiceClipForThisLine));

        // ĳ���� �̹��� ������Ʈ
        if (characterImageView != null)
        {
            if (currentDialogue.characterImage != null && currentDialogue.characterImage.Length > currentDialogueIndex)
            {
                characterImageView.sprite = currentDialogue.characterImage[currentDialogueIndex];
            }
            else
            {
                characterImageView.sprite = null; // �ش� �ε����� �̹����� ������ null�� ����
            }
        }

        // ĳ���� �̸� ������Ʈ
        if (characterNameText != null)
        {
            if (currentDialogue.characterName != null && currentDialogue.characterName.Length > currentDialogueIndex)
            {
                characterNameText.text = currentDialogue.characterName[currentDialogueIndex];
            }
            else
            {
                characterNameText.text = ""; // �ش� �ε����� �̸��� ������ �� ���ڿ��� ����
            }
        }

        currentDialogueIndex++; // ���� ��� �ε����� ����
    }

    /// <summary>
    /// ��ȭ �ؽ�Ʈ�� �� ���ھ� Ÿ���� ȿ���� ǥ���մϴ�.
    /// </summary>
    /// <param name="line">ǥ���� ��� ���ڿ�</param>
    /// <param name="voiceClip">�� ���ڸ��� ����� ��Ҹ� Ŭ��</param>
    private IEnumerator TypeDialogue(string line, AudioClip voiceClip)
    {
        isTyping = true;
        dialogueText.text = ""; // �ؽ�Ʈ ǥ�� ���� �ʱ�ȭ

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter; // �� ���ھ� �߰�

            // ���޹��� ��Ҹ� Ŭ���� �ְ� AudioSource�� ������ ���
            if (voiceClip != null && dialogueAudioSource != null)
            {
                dialogueAudioSource.PlayOneShot(voiceClip);
            }

            yield return new WaitForSeconds(textSpeed); // ���� �ӵ���ŭ ���
        }
        isTyping = false; // Ÿ���� �Ϸ�

        // ���� ��簡 �ڵ� ���� ����� (����̷� �����ߴٸ�), �ٷ� ���� ���� ����
        if (isAutoAdvancing)
        {
            DisplayNextLine();
        }
    }

    /// <summary>
    /// ��ȭ �������� �����մϴ�.
    /// </summary>
    public void EndDialogue()
    {
        Debug.Log("��ȭ ����");
        // ��ȭ ���� �� Dialogue UI �г� ��Ȱ��ȭ (����)
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(false);
        }

        // ���� UI ��ҵ鵵 ���� �ʱ�ȭ (�г��� ��Ȱ��ȭ�Ǹ� ������ ������, ���� ���� �ʱ�ȭ)
        if (characterImageView != null) characterImageView.gameObject.SetActive(false);
        if (characterNameText != null) characterNameText.gameObject.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

        // ���� ������ �ʱ�ȭ
        currentDialogue = null;
        currentDialogueIndex = 0;
        isAutoAdvancing = false;
        isTyping = false;

        // ��ȭ ���� �� �ʿ��� �߰� ���� (��: ���� ���� ����, �� ��ȯ ��)
    }

    // --- �ٸ� ��ũ��Ʈ���� ��ȭ�� ������ �� ���� ---
    // GameManager.Instance.StartDialogue(mySpecificDialogueSOAsset);
    // �Ǵ�
    // if (DialogueManager.Instance != null) {
    //     DialogueManager.Instance.StartDialogue(mySpecificDialogueSOAsset);
    // }
}