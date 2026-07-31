using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Thông tin cơ bản")] 
    public int ID; // <-- THÊM DÒNG NÀY (ID duy nhất cho mỗi item: 0, 1, 2, 3...)
    public string enemyName;
    [TextArea(3,10)] public string enemyDescription;
    public Sprite enemySprite;
    public AnimatorController animatorController;
}
