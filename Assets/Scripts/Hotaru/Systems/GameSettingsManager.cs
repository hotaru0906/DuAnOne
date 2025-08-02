using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class GameSettingsManagers : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public TMP_Dropdown fpsDropdown; // Change to TMP_Dropdown
    public Toggle fullscreenToggle;

    void Start()
    {
        // Update TMP_Dropdown options
        fpsDropdown.options.Clear();
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("30"));
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("60"));
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("120"));

        // Load settings from PlayerPrefs
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        fpsDropdown.value = PlayerPrefs.GetInt("FPSIndex", 1); // default 60
        fullscreenToggle.isOn = PlayerPrefs.GetInt("FullScreen", 1) == 1;

        volumeSlider.onValueChanged.AddListener(UpdateVolume); // Link slider to immediate update
        fpsDropdown.onValueChanged.AddListener(UpdateFPS); // Link dropdown to immediate update
        fullscreenToggle.onValueChanged.AddListener(UpdateFullscreen); // Link toggle to immediate update

        ApplySettings(); // Apply on start
    }

    public void ApplySettings()
    {
        // Volume
        float volume = Mathf.Clamp(volumeSlider.value, 0f, 1f); // Ensure volume is within valid range
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);

        // FPS
        int[] fpsOptions = { 30, 60, 120 };
        int selectedFPS = fpsOptions[fpsDropdown.value];
        Application.targetFrameRate = selectedFPS;
        PlayerPrefs.SetInt("FPSIndex", fpsDropdown.value);

        // Fullscreen
        bool isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullScreen", isFullscreen ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void UpdateVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp(volume, 0f, 1f); // Update volume immediately
        PlayerPrefs.SetFloat("Volume", volume); // Save the value
    }

    public void UpdateFPS(int index)
    {
        int[] fpsOptions = { 30, 60, 120 };
        int selectedFPS = fpsOptions[index];
        Application.targetFrameRate = selectedFPS; // Update FPS immediately
        PlayerPrefs.SetInt("FPSIndex", index); // Save the value
    }

    public void UpdateFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen; // Update fullscreen immediately
        PlayerPrefs.SetInt("FullScreen", isFullscreen ? 1 : 0); // Save the value
    }
}
