using UnityEngine;

[CreateAssetMenu(fileName = "My New Dialogue", menuName = "Dialogue System/Dialogue Line")]
public class DialogueSO : ScriptableObject
{
    [Header("캐릭터 이미지")]
    public Sprite[] characterImage;
    [Header("캐릭터 이름")]
    public string[] characterName;
    [Header("캐릭터 목소리")]
    public AudioClip[] sfxClips;
    [Header("대화열 리스트")]
    public string[] DialogueText;
}
