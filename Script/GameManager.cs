using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float survivalTime = 3f; // Thời gian cần thủ để thắng
    [SerializeField] private int energyCostPerPlay = 20; // Số năng lượng tốn khi Retry (chơi lại)
    
    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;  // Kéo GameWinUI vào đây
    [SerializeField] private GameObject losePanel; // Kéo GameOverUI vào đây
    [SerializeField] private GameObject pauseUI;
    
    [Header("UI Currency")]
    [SerializeField] private TextMeshProUGUI coinText; // Kéo object "Coins" vào đây
    
    [Header("UI Animation Settings")]
    [SerializeField] private float slideDuration = 3f; // Tốc độ trượt (giây)
    [SerializeField] private float startYPosition = 1200f; // Độ cao bắt đầu trượt ở ngoài màn hình

    private float _timer;
    private bool _isGamePlaying = false;
    private bool _isGameOver = false;
    private bool _isPauseGame = false;
    private int _currentCoins;

    private const string ENERGY_KEY = "CurrentEnergy";
    
    [SerializeField] private GameObject Story;
    [TextArea(3, 5)] // Tạo ô viết chữ rộng rãi trong Inspector
    [Header("Lời nói khi chiến thắng")]
    [SerializeField] private string loinoiWin = "Chúc mừng bạn đã giành chiến thắng!!!";
    [Header("Lời nói khi Thua")]
    [SerializeField] private string loinoiLose = "Tiếc quá!!!  bạn hãy cố gắng lên!";
        

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        // Đảm bảo tốc độ game bình thường khi mới vào màn chơi
        Time.timeScale = 1f;

        // Ẩn các panel khi mới vào game
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(false);
        if(Story != null) Story.SetActive(false);  
        
        _currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        _currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);

        if (coinText != null)
        {
            coinText.text = _currentCoins.ToString();
        }
    }

    // Gọi khi vẽ xong xuôi và nhấc chuột lên
    public void StartCountdown()
    {
        if (_isGameOver) return;
        
        // --- TỰ ĐỘNG TÌM ĐỐI TƯỢNG PLAYER VÀ BẬT LẠI VẬT LÝ ---
        Player_Controller[] players = FindObjectsByType<Player_Controller>(FindObjectsSortMode.None);
        foreach (Player_Controller player in players)
        {
            player.SetPhysicsActive(true);
        }
        
        // --- KÍCH HOẠT RIGIDBODY CHO VIÊN ĐÁ (DÙNG TAG) ---
        GameObject[] traps = GameObject.FindGameObjectsWithTag("TrapMove");
        foreach (GameObject trap in traps)
        {
            Rigidbody2D rb = trap.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic; // Chuyển sang Dynamic để đá rơi
            }
        }

        _timer = survivalTime;
        _isGamePlaying = true;
        Debug.Log("Bắt đầu đếm ngược sinh tồn: " + survivalTime + " giây!");
    }

    void Update()
    {
        if (_isGamePlaying && !_isGameOver)
        {
            _timer -= Time.deltaTime;
            
            // Nếu hết thời gian đếm ngược mà chưa thua -> Chiến thắng!
            if (_timer <= 0)
            {
                WinGame();
            }
        }
    }

    public void WinGame()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        _isGamePlaying = false;
        _isPauseGame = false;
        
        
        // --- XỬ LÝ CỘNG TIỀN KHI THẮNG ---
        _currentCoins += 100;
        PlayerPrefs.SetInt("TotalCoins", _currentCoins);
        PlayerPrefs.Save();
        UpdateCoinUI();
        // (Thắng MIỄN PHÍ - Không trừ Energy ở đây)
        Story.SetActive(true);
        TypeWriterEffect writer = Story.GetComponentInChildren<TypeWriterEffect>();
            
        if (writer != null)
        {
            // Truyền đoạn hội thoại riêng của con quái này vào hiệu ứng gõ chữ
            writer.StartTyping(loinoiWin);
        }
        Debug.Log("GAME WIN!, BAN DA CHIEN THANG");
        if (winPanel != null) 
        {
            StartCoroutine(SlideInPanel(winPanel));
        }
    }

    public void LoseGame()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        _isGamePlaying = false;
        _isPauseGame = false;

        Story.SetActive(true);
        TypeWriterEffect writer = Story.GetComponentInChildren<TypeWriterEffect>();
            
        if (writer != null)
        {
            // Truyền đoạn hội thoại riêng của con quái này vào hiệu ứng gõ chữ
            writer.StartTyping(loinoiLose);
        }
        Debug.Log("GAME OVER! DOGE BỊ ĐỐT RỒI!");
        if (losePanel != null) 
        {
            StartCoroutine(SlideInPanel(losePanel));
        }
    }

    // Coroutine xử lý trượt UI mượt mà từ trên xuống
    private IEnumerator SlideInPanel(GameObject panel)
    {
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            panel.SetActive(true);
            Time.timeScale = 0f;
            yield break;
        }

        Vector2 targetPosition = Vector2.zero; 
        rectTransform.anchoredPosition = new Vector2(0, startYPosition);
        
        panel.SetActive(true);

        float elapsedTime = 0f;
        Vector2 startPosition = rectTransform.anchoredPosition;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / slideDuration;
            
            t = Mathf.SmoothStep(0, 1, t);

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        // SAU KHI UI TRƯỢT XUỐNG XONG -> DỪNG GAME TOÀN BỘ
        Time.timeScale = 0f; 
        Debug.Log("Đã dừng game!");
    }

    // --- CÁC HÀM XỬ LÝ CHUYỂN SCENE & NĂNG LƯỢNG (ENERGY) ---

    // Nút RETRY (Bảng Lose) -> Chỉ khi THUA và bấm RETRY mới trừ Energy
    public void RestartLevel()
    {
        Time.timeScale = 1f;

        // Kiểm tra và trừ 20 energy khi bấm Retry
        if (EnergyBar.Instance != null && EnergyBar.Instance.UseEnergy(energyCostPerPlay))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (EnergyBar.Instance == null)
        {
            // Dự phòng nếu chưa khởi tạo EnergyBar
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("Không đủ năng lượng để Retry! Quay về Menu.");
            SceneManager.LoadScene("Menu");
        }
    }

    // Nút MENU
    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    // Nút NEXT LEVEL (Bảng Win) -> Thắng sang màn mới MIỄN PHÍ, không tốn Energy
    public void NextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Bạn đã phá đảo game! Quay lại Menu.");
            SceneManager.LoadScene("KetThucMan");
        }
    }
    
    // Gọi hàm này khi bắt đầu ấn chuột vẽ để đóng băng game
    public void PauseGameForDrawing()
    {
        Time.timeScale = 0f;
        Debug.Log("Đang vẽ - Game tạm dừng.");
    }

    // Gọi hàm này khi nhấc chuột lên (vẽ xong) để tiếp tục game
    public void ResumeGameAfterDrawing()
    {
        Time.timeScale = 1f;
        Debug.Log("Vẽ xong - Game tiếp tục.");
    }

    public void ContinueGame()
    {
        _isPauseGame = false;
        _isGamePlaying = true;
        if (pauseUI != null) pauseUI.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Tiep Tuc Game");
    }

    public void PauseGame()
    {
        if (_isPauseGame) return;
        _isGameOver = false;
        _isGamePlaying = false;
        _isPauseGame = true;

        Time.timeScale = 0f;
        Debug.Log("PAUSE GAME");
        if (pauseUI != null) 
        {
            StartCoroutine(SlideInPanel(pauseUI));
        }
    }
}