using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Slider sliderVolume;

    private void OnEnable()
    {
        if (sliderVolume != null)
        {
            // Lấy volume hiện tại từ PlayerPrefs hoặc SoundManager để set cho Slider
            float currentVolume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
            sliderVolume.value = currentVolume;

            // Lắng nghe sự kiện kéo slider
            sliderVolume.onValueChanged.RemoveAllListeners();
            sliderVolume.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    private void OnSliderChanged(float value)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetVolume(value);
        }
    }
}