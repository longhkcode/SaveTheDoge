using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [Header("Shop Item Data")]
    public BuyItem_InShop itemData; // Kéo file ScriptableObject (Skin_Frog, Enemy_Dark...) vào đây

    [Header("UI References")]
    public Image itemImage;         // Kéo Img_Item vào đây
    public TextMeshProUGUI priceText; // Kéo Text (TMP) hiển thị giá (số 500) vào đây
    
    [Header("Button Config")]
    public Button buyButton;        // Kéo GameObject 'Buy' vào đây
    public Sprite buySprite;        // Kéo ảnh nút BUY (xanh) từ Project vào đây
    public Sprite purchasedSprite;  // Kéo ảnh nút PURCHASED (đỏ) từ Project vào đây

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

        // Kiểm tra trong PlayerPrefs xem item này đã mua chưa (1 = Đã mua, 0 = Chưa mua)
        bool isBought = PlayerPrefs.GetInt("Bought_Item_" + itemData.itemID, 0) == 1;

        if (isBought)
        {
            // Đã mua: Chuyển sprite sang nút PURCHASED đỏ và khóa nút
            if (buyButton != null)
            {
                if (purchasedSprite != null) buyButton.image.sprite = purchasedSprite;
                buyButton.interactable = false;
            }
            
            if (priceText != null) priceText.gameObject.SetActive(false);
        }
        else
        {
            // Chưa mua: Giữ sprite nút BUY xanh và mở nút
            if (buyButton != null)
            {
                if (buySprite != null) buyButton.image.sprite = buySprite;
                buyButton.interactable = true;
            }

            // Hiện chữ giá tiền
            if (priceText != null) priceText.gameObject.SetActive(true);
        }
    }

    // Gán hàm này vào sự kiện On Click() của nút Buy
    public void Buy()
    {
        if (itemData == null) return;
        int coins = PlayerPrefs.GetInt("TotalCoins", 1000); 

        if (coins >= itemData.itemPrice)
        {
            // Trừ tiền
            coins -= itemData.itemPrice;
            PlayerPrefs.SetInt("TotalCoins", coins); // ĐỔI THÀNH "TotalCoins"

            // Đánh dấu item này đã mua
            PlayerPrefs.SetInt("Bought_Item_" + itemData.itemID, 1);
            PlayerPrefs.Save();

            // Cập nhật lại giao diện nút trong Shop
            UpdateShopUI();

            // CẬP NHẬT LẠI HIỂN THỊ TIỀN TRÊN MÀN HÌNH
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