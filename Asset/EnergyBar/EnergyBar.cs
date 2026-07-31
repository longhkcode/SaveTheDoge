using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public static EnergyBar Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private Image energyImage;

    [Header("Energy Sprites (Thứ tự: 0/Empty, 20, 40, 60, 80, 100/Full)")]
    [SerializeField] private List<Sprite> energySprites;

    private const string ENERGY_KEY = "CurrentEnergy";
    private const string LAST_REGEN_TIME_KEY = "LastEnergyRegenTimeBinary";

    private const int MAX_ENERGY = 100;
    private const int ENERGY_PER_STEP = 20;
    private const float REGEN_INTERVAL = 120f; // Test 10 giây (Sửa lại 120f khi hoàn thiện game)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitEnergyData();
    }

    void Start()
    {
        UpdateEnergyUI();
    }

    void Update()
    {
        CheckAndRegenerateEnergy();
    }

    private void InitEnergyData()
    {
        // 1. Nếu người chơi mới chơi lần đầu
        if (!PlayerPrefs.HasKey(ENERGY_KEY))
        {
            PlayerPrefs.SetInt(ENERGY_KEY, MAX_ENERGY);
            SetLastRegenTime(DateTime.Now);
            PlayerPrefs.Save();
        }

        // 2. Tính toán năng lượng hồi bù khi người chơi offline/mở lại game
        CalculateOfflineEnergy();
    }

    private DateTime GetLastRegenTime()
    {
        if (!PlayerPrefs.HasKey(LAST_REGEN_TIME_KEY))
        {
            SetLastRegenTime(DateTime.Now);
        }
        
        long binaryTime = Convert.ToInt64(PlayerPrefs.GetString(LAST_REGEN_TIME_KEY, DateTime.Now.ToBinary().ToString()));
        return DateTime.FromBinary(binaryTime);
    }

    private void SetLastRegenTime(DateTime time)
    {
        PlayerPrefs.SetString(LAST_REGEN_TIME_KEY, time.ToBinary().ToString());
    }

    private void CalculateOfflineEnergy()
    {
        int currentEnergy = PlayerPrefs.GetInt(ENERGY_KEY, MAX_ENERGY);
        if (currentEnergy >= MAX_ENERGY) return;

        DateTime lastTime = GetLastRegenTime();
        TimeSpan timePassed = DateTime.Now - lastTime;

        int stepsToAdd = (int)(timePassed.TotalSeconds / REGEN_INTERVAL);

        if (stepsToAdd > 0)
        {
            currentEnergy = Mathf.Min(MAX_ENERGY, currentEnergy + (stepsToAdd * ENERGY_PER_STEP));
            PlayerPrefs.SetInt(ENERGY_KEY, currentEnergy);

            // Cập nhật lại mốc thời gian (giữ lại thời gian dư chưa đủ 1 chu kỳ 10s)
            DateTime newLastTime = lastTime.AddSeconds(stepsToAdd * REGEN_INTERVAL);
            SetLastRegenTime(newLastTime);

            PlayerPrefs.Save();
            UpdateEnergyUI();
        }
    }

    private void CheckAndRegenerateEnergy()
    {
        int currentEnergy = PlayerPrefs.GetInt(ENERGY_KEY, MAX_ENERGY);
        if (currentEnergy >= MAX_ENERGY) return;

        DateTime lastTime = GetLastRegenTime();
        double secondsPassed = (DateTime.Now - lastTime).TotalSeconds;

        if (secondsPassed >= REGEN_INTERVAL)
        {
            currentEnergy = Mathf.Min(MAX_ENERGY, currentEnergy + ENERGY_PER_STEP);
            PlayerPrefs.SetInt(ENERGY_KEY, currentEnergy);

            // Tăng mốc thời gian lên đúng REGEN_INTERVAL thay vì gán DateTime.Now để không bị mất giây thừa
            SetLastRegenTime(lastTime.AddSeconds(REGEN_INTERVAL));
            PlayerPrefs.Save();

            UpdateEnergyUI();
            Debug.Log($"[Energy] Đã hồi +20 Energy! Hiện tại: {currentEnergy}");
        }
    }

    public bool UseEnergy(int amount = 20)
    {
        int currentEnergy = PlayerPrefs.GetInt(ENERGY_KEY, MAX_ENERGY);

        if (currentEnergy >= amount)
        {
            // Nếu đang đầy máu mà dùng năng lượng lần đầu -> Bắt đầu tính mốc thời gian hồi từ bây giờ
            if (currentEnergy >= MAX_ENERGY)
            {
                SetLastRegenTime(DateTime.Now);
            }

            currentEnergy -= amount;
            PlayerPrefs.SetInt(ENERGY_KEY, currentEnergy);
            PlayerPrefs.Save();

            UpdateEnergyUI();
            return true;
        }

        Debug.Log("Hết năng lượng");
        return false;
    }

    public void UpdateEnergyUI()
    {
        int energy = PlayerPrefs.GetInt(ENERGY_KEY, MAX_ENERGY);

        if (energyImage != null && energySprites != null && energySprites.Count > 0)
        {
            int index = Mathf.Clamp(energy / ENERGY_PER_STEP, 0, energySprites.Count - 1);
            energyImage.sprite = energySprites[index];
        }
    }
    
    public void AddEnergy(int amount = 20)
    {
        int currentEnergy = PlayerPrefs.GetInt(ENERGY_KEY, MAX_ENERGY);

        // Kiểm tra nếu đã đầy năng lượng thì không cần cộng thêm
        if (currentEnergy >= MAX_ENERGY)
        {
            Debug.Log("Energy đã đầy, không thể mua thêm!");
            return;
        }

        // Cộng năng lượng và giới hạn tối đa là 100 (MAX_ENERGY)
        currentEnergy = Mathf.Min(MAX_ENERGY, currentEnergy + amount);

        PlayerPrefs.SetInt(ENERGY_KEY, currentEnergy);
        PlayerPrefs.Save();

        // Cập nhật lại thanh UI hiển thị Energy
        UpdateEnergyUI();

        Debug.Log($"[Energy] Đã mua thành công +{amount} Energy! Hiện tại: {currentEnergy}");
    }
}