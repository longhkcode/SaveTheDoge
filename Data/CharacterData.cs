using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public int ID;
    public string characterName;
    [TextArea(3, 10)] public string characterDescription;
    public Sprite characterSprite;
    
    // Đổi kiểu dữ liệu tại đây
    public RuntimeAnimatorController animatorController; 
}