using System.Collections.Generic;
using UnityEngine;

public class BagController : MonoBehaviour
{
    [Header("Shop Data List")]
    public List<BuyItem_InShop> shopItems;

    [Header("Skin Data Lists")]
    public List<CharacterData> characterSkins; 
    public List<EnemyData> enemySkins;         

    [Header("UI References")]
    public Transform contentContainer;    
    public GameObject bagItemPrefab;       

    void OnEnable()
    {
        LoadBagItems();
    }

    public void LoadBagItems()
    {
        // Xóa các ô cũ
        for (int i = contentContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(contentContainer.GetChild(i).gameObject);
        }

        int equippedPlayerID = PlayerPrefs.GetInt("Equipped_Item", -1);
        int equippedEnemyID = PlayerPrefs.GetInt("Equipped_Enemy_Item", -1);

        foreach (var item in shopItems)
        {
            if (item == null) continue;

            // Kiểm tra xem item đã được mua chưa
            bool isBought = PlayerPrefs.GetInt("Bought_Item_" + item.itemID, 0) == 1;

            if (isBought)
            {
                CharacterData charSkin = characterSkins.Find(s => s != null && s.ID == item.itemID);
                EnemyData enemySkin = enemySkins.Find(e => e != null && e.ID == item.itemID);

                GameObject newSlot = Instantiate(bagItemPrefab, contentContainer);
                BagItemUI itemUI = newSlot.GetComponent<BagItemUI>();

                if (itemUI != null)
                {
                    bool isEnemy = (enemySkin != null) || item.name.ToLower().Contains("enemy") || item.name.ToLower().Contains("dark") || item.name.ToLower().Contains("phihanhgia");
                
                    bool isEquipped = isEnemy ? (equippedEnemyID == item.itemID) : (equippedPlayerID == item.itemID);

                    // Gọi duy nhất hàm SetupUI mới
                    itemUI.SetupUI(item, charSkin, enemySkin, this, isEquipped);
                }
            }
        }
    }

    public void EquipPlayerSkin(int skinID)
    {
        PlayerPrefs.SetInt("Equipped_Item", skinID);
        PlayerPrefs.Save();
        LoadBagItems();
    }

    public void EquipEnemySkin(int skinID)
    {
        PlayerPrefs.SetInt("Equipped_Enemy_Item", skinID);
        PlayerPrefs.Save();
        LoadBagItems();
    }

    public void SellSkinFromUI(BuyItem_InShop item, bool isEnemy)
    {
        if (item == null) return;

        // Hoàn tiền 80%
        int refundAmount = item.itemPrice * 80 / 100;
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0) + refundAmount;
        PlayerPrefs.SetInt("TotalCoins", currentCoins);

        PlayerPrefs.SetInt("Bought_Item_" + item.itemID, 0);

        // Bán xong tháo skin ra (Về mặc định -1)
        if (isEnemy)
        {
            if (PlayerPrefs.GetInt("Equipped_Enemy_Item", -1) == item.itemID)
            {
                PlayerPrefs.SetInt("Equipped_Enemy_Item", -1);
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("Equipped_Item", -1) == item.itemID)
            {
                PlayerPrefs.SetInt("Equipped_Item", -1);
            }
        }

        PlayerPrefs.Save();

        if (GameManager.Instance != null) GameManager.Instance.UpdateCoinUI();

        Shop[] shops = Object.FindObjectsByType<Shop>(FindObjectsSortMode.None);
        foreach (var s in shops) s.UpdateShopUI();

        LoadBagItems();
    }
}