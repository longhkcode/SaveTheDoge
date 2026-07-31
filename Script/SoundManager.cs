using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Khởi tạo volume ngay khi game chạy
        LoadVolume();
    }

    // Hàm lưu & đổi Volume tổng
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SoundVolume", volume);
        PlayerPrefs.Save();
    }

    public void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        AudioListener.volume = savedVolume;
    }

    public float GetCurrentVolume()
    {
        return PlayerPrefs.GetFloat("SoundVolume", 1.0f);
    }
}