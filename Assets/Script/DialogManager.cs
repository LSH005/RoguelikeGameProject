using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����Ѵٸ� �� ���ӽ����̽��� �߰��ϼ���.
using System.Collections; // Coroutine�� ����Ϸ��� �ʿ��մϴ�.

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public DialogueSO currentDialogue;

    // UI ��� ���� (�ν����Ϳ��� �Ҵ�)
    [Header("UI Components")]
    public GameObject dialogueUIPanel; // �߰�: Dialogue UI�� �θ� ������Ʈ
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
            //DontDestroyOnLoad(gameObject); // �� ��ȯ �� ���� (���� ����)
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // ���� �� Dialogue UI�� �ϴ� ��Ȱ��ȭ (��ȭ ���� ������ ������ �־�� ��)
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(false);
        }

        //�׽�Ʈ�� ���� ���� ���� �� ��ȭ ����(���� ���ӿ����� ���Ǻη� ȣ��)
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
            Debug.LogWarning("��ȿ�� DialogueSO�� ���ų� ��ȭ ������ ��� �ֽ��ϴ�.");
            EndDialogue(); // ��ȿ���� ������ �ٷ� ���� ó��
            return;
        }

        currentDialogueIndex = 0;

        // --- ������ �κ�: ��ȭ ���� �� Dialogue UI Ȱ��ȭ ---
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(true);
        }
        // ---

        // ������ ���� UI ��ҵ鵵 Ȱ��ȭ (dialogueUIPanel�� Ȱ��ȭ�Ǹ� �ڽĵ鵵 ����)
        // �ٸ�, ������ ���� ĳ���� �̹����� �̸��� ���� ��簡 ���� ��츦 ����Ͽ� Ȱ��ȭ ���¸� �����ϴ� ���� �����ϴ�.
        if (characterImageView != null) characterImageView.gameObject.SetActive(true);
        if (characterNameText != null) characterNameText.gameObject.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);

        DisplayNextLine(); // ù ��° ��� ǥ��
    }

    public void DisplayNextLine()
    {
        StopAllCoroutines();
        isTyping = false;

        if (currentDialogueIndex >= currentDialogue.DialogueText.Length)
        {
            EndDialogue(); // �� �̻� ��簡 ������ ��ȭ ����
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

        // ĳ���� �̹��� ������Ʈ
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

        // ĳ���� �̸� ������Ʈ
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

        // ĳ���� ��Ҹ�(SFX) ���
        if (currentDialogue.sfxClips != null && currentDialogue.sfxClips.Length > currentDialogueIndex)
        {
            AudioClip currentVoiceClip = currentDialogue.sfxClips[currentDialogueIndex];
            if (currentVoiceClip != null)
            {
                // AudioSource ������Ʈ�� DialogueManager�� �߰��� �� ���
                // GetComponent<AudioSource>().PlayOneShot(currentVoiceClip);
                Debug.Log($"'{currentVoiceClip.name}' ��Ҹ� ���!");
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
        Debug.Log("��ȭ ����");
        // --- ������ �κ�: ��ȭ ���� �� Dialogue UI ��Ȱ��ȭ ---
        if (dialogueUIPanel != null)
        {
            dialogueUIPanel.SetActive(false);
        }
        // ---

        // ������ UI ��ҵ��� �θ� ��Ȱ��ȭ�Ǹ� �ڵ����� ���������, ���� �ʱ�ȭ�� ���� ��������� �صδ� �͵� ����.
        if (characterImageView != null) characterImageView.gameObject.SetActive(false);
        if (characterNameText != null) characterNameText.gameObject.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

        currentDialogue = null;
        currentDialogueIndex = 0;
        isAutoAdvancing = false;
        isTyping = false;
        // ��ȭ ���� �� �ʿ��� ���� �߰�
    }
}