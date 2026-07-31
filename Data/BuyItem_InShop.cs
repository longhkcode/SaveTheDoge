using UnityEngine;

[CreateAssetMenu(fileName = "BuyItem_InShop", menuName = "Scriptable Objects/BuyItem_InShop")]
public class BuyItem_InShop : ScriptableObject
{
    [Header("Thông tin cơ bản")] 
    public int itemID; // <-- THÊM DÒNG NÀY (ID duy nhất cho mỗi item: 0, 1, 2, 3...)
    public string itemName;
    [TextArea(3,10)] public string itemDescription;
    public Sprite itemSprite;
    [Min(0)] public int itemPrice;
}