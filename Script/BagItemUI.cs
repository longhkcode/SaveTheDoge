using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BagItemUI : MonoBehaviour
{
    [HideInInspector] public BuyItem_InShop shopItem;
    [HideInInspector] public CharacterData characterData;
    [HideInInspector] public EnemyData enemyData;

    public bool isEnemyItem = false;

    [Header("UI Component")]
    public Button useButton;
    public Button sellButton;
    public TextMeshProUGUI useButtonText; 
    public Image iconImage; 

    private BagController _bagController;

    public void SetupUI(BuyItem_InShop shopData, CharacterData charSkin, EnemyData enemySkin, BagController controller, bool isEquipped)
    {
        shopItem = shopData;
        characterData = charSkin;
        enemyData = enemySkin;
        _bagController = controller;

        // Xác định là item của Enemy hay Player
        isEnemyItem = (enemyData != null) || (shopData != null && shopData.name.ToLower().Contains("enemy"));

        // 1. GÁN HÌNH ẢNH (Ưu tiên lấy từ Shop Item Sprite)
        if (iconImage != null)
        {
            Sprite spriteToSet = null;

            if (shopData != null && shopData.itemSprite != null)
            {
                spriteToSet = shopData.itemSprite; // Luôn lấy trực tiếp từ Shop Item
            }
            else if (charSkin != null && charSkin.characterSprite != null)
            {
                spriteToSet = charSkin.characterSprite;
            }
            else if (enemySkin != null && enemySkin.enemySprite != null)
            {
                spriteToSet = enemySkin.enemySprite;
            }

            if (spriteToSet != null)
            {
                iconImage.sprite = spriteToSet;
                iconImage.enabled = true;
                iconImage.gameObject.SetActive(true);
            }
        }

        // 2. SỰ KIỆN NÚT
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(OnClickUse);
        }

        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(OnClickSell);
        }

        UpdateUIState(isEquipped);
    }

    public void UpdateUIState(bool isEquipped)
    {
        if (useButtonText != null)
        {
            useButtonText.text = isEquipped ? "EQUIPPED" : "USE";
        }

        if (useButton != null)
        {
            useButton.interactable = !isEquipped;
        }
    }

    private void OnClickUse()
    {
        if (_bagController == null || shopItem == null) return;

        if (isEnemyItem)
        {
            _bagController.EquipEnemySkin(shopItem.itemID);
        }
        else
        {
            _bagController.EquipPlayerSkin(shopItem.itemID);
        }
    }

    private void OnClickSell()
    {
        if (_bagController != null && shopItem != null)
        {
            _bagController.SellSkinFromUI(shopItem, isEnemyItem);
        }
    }
}