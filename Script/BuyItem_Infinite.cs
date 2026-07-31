using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItem_Infinite : MonoBehaviour
{
    [Header("Shop Item Data")]
    public BuyItem_InShop itemData; // Kéo file ScriptableObject (Skin_Frog, Enemy_Dark...) vào đây

    [Header("UI References")]
    public Image itemImage;         // Kéo Img_Item vào đây
    public TextMeshProUGUI priceText; // Kéo Text (TMP) hiển thị giá (số 500) vào đây
    
    [Header("Button Config")]
    public Button buyButton;        // Kéo GameObject 'Buy' vào đây
    public Sprite buySprite;        // Kéo ảnh nút BUY (xanh) từ Project vào đây

    void Start()
    {
        SetupUI();
    }

    public void SetupUI()
    {
        if (itemData == null) return;

        // Load ảnh con chó và giá tiền từ ScriptableObject lên UI
        if (itemImage != null) itemImage.sprite = itemData.itemSprite;
        if (priceText != null) priceText.text = itemData.itemPrice.ToString();

        UpdateShopUI();
    }

    public void UpdateShopUI()
    {
        if (itemData == null) return;
        if (buyButton != null)
        {
            if (buySprite != null) buyButton.image.sprite = buySprite;
            buyButton.interactable = true;
        }

        // Hiện chữ giá tiền
        if (priceText != null) priceText.gameObject.SetActive(true);
    }

    // Gán hàm này vào sự kiện On Click() của nút Buy
    public void BuyEnergy()
    {
        if (itemData == null) return;

        // Kiểm tra nếu mua Năng Lượng mà thanh Energy đã đầy (100) thì không cho mua
        /* (Tùy chọn: Nếu bạn không muốn người chơi tốn tiền vô ích khi đã 100% Energy) */

        int coins = PlayerPrefs.GetInt("TotalCoins", 1000); 

        if (coins >= itemData.itemPrice)
        {
            // Trừ tiền
            coins -= itemData.itemPrice;
            PlayerPrefs.SetInt("TotalCoins", coins);

            // Đánh dấu item này đã mua
            PlayerPrefs.SetInt("Bought_Item_" + itemData.itemID, 1);
            PlayerPrefs.Save();

            // --- CỘNG NĂNG LƯỢNG KHI MUA ITEM SÉT ---
            if (EnergyBar.Instance != null)
            {
                EnergyBar.Instance.AddEnergy(20); // Cộng 20 năng lượng (tối đa 100)
            }

            // Cập nhật lại giao diện Shop & Coin
            UpdateShopUI();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateCoinUI();
            }

            Debug.Log("Mua thành công: " + itemData.itemName);
        }
        else
        {
            Debug.Log("Không đủ tiền mua!");
        }
    }
    
    void OnEnable()
    {
        UpdateShopUI();
    }
}
