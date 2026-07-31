using UnityEngine;
using UnityEditor.Animations;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Thông tin cơ bản")] 
    public int ID; // <-- THÊM DÒNG NÀY (ID duy nhất cho mỗi item: 0, 1, 2, 3...)
    public string characterName;
    [TextArea(3,10)] public string characterDescription;
    public Sprite characterSprite;
    public AnimatorController animatorController;
}
