using UnityEngine;
using UnityEngine.UI;
using Platformer;

public class VolumeSliderController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    void Start()
    {
        if (AudioManager.Instance != null && volumeSlider != null)
        {
            // Set slider to current AudioManager volume
            volumeSlider.value = AudioManager.Instance.backgroundMusicVolume;
            // Add listener for slider changes
            volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }
}
