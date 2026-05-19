using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour

{
    [Header("Settings Panel")]
    public GameObject settingsPanel;

    [Header("UI")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

    [Header("Music")]
    public MusicManager musicManager;

    [Header("SFX")]
    public List<AudioSource> sfxSources =
        new List<AudioSource>();

    private const float DEFAULT_VOLUME = 0.4f;

    private float musicVolume;
    private float sfxVolume;

    
    private bool blockInput = false;

    void Start()
    {
       

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

       
        Time.timeScale = 1f;

       
        musicVolume =
            PlayerPrefs.GetFloat(
                "MusicVolume",
                DEFAULT_VOLUME
            );

        sfxVolume =
            PlayerPrefs.GetFloat(
                "SFXVolume",
                DEFAULT_VOLUME
            );

       

        if (musicSlider != null)
        {
            musicSlider
                .SetValueWithoutNotify(
                    musicVolume
                );

            musicSlider
                .onValueChanged
                .AddListener(
                    SetMusicVolume
                );
        }

       

        if (sfxSlider != null)
        {
            sfxSlider
                .SetValueWithoutNotify(
                    sfxVolume
                );

            sfxSlider
                .onValueChanged
                .AddListener(
                    SetSFXVolume
                );
        }

        

        bool fullscreen =
            PlayerPrefs.GetInt(
                "Fullscreen",
                1
            ) == 1;

        Screen.fullScreen =
            fullscreen;

        if (fullscreenToggle != null)
        {
            fullscreenToggle
                .SetIsOnWithoutNotify(
                    fullscreen
                );

            fullscreenToggle
                .onValueChanged
                .AddListener(
                    SetFullscreen
                );
        }

        

        ApplyMusicVolume();
        ApplySFXVolume();
    }

    void Update()
    {
       
        if (blockInput)
            return;

        
        if (
            Input.GetKeyDown(
                KeyCode.Escape
            )
        )
        {
            ToggleSettings();
        }
    }

   

    public void ToggleSettings()
    {
        if (settingsPanel == null)
            return;

        bool open =
            !settingsPanel.activeSelf;

        settingsPanel.SetActive(open);

        if (open)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }

       
        StartCoroutine(BlockClick());
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);

            PauseGame();

            StartCoroutine(BlockClick());
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);

            ResumeGame();

            StartCoroutine(BlockClick());
        }
    }

    

    IEnumerator BlockClick()
    {
        blockInput = true;

        
        yield return new WaitUntil(
            () => !Input.GetMouseButton(0)
        );

        
        yield return null;

        blockInput = false;
    }

   

    void PauseGame()
    {
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
    }

   

    public void SetMusicVolume(
        float volume
    )
    {
        musicVolume = volume;

        PlayerPrefs.SetFloat(
            "MusicVolume",
            musicVolume
        );

        PlayerPrefs.Save();

        ApplyMusicVolume();
    }

    void ApplyMusicVolume()
    {
        if (musicManager != null)
        {
            musicManager.SetVolume(
                musicVolume
            );
        }
    }

   

    public void SetSFXVolume(
        float volume
    )
    {
        sfxVolume = volume;

        PlayerPrefs.SetFloat(
            "SFXVolume",
            sfxVolume
        );

        PlayerPrefs.Save();

        ApplySFXVolume();
    }

    void ApplySFXVolume()
    {
        foreach (
            AudioSource source
            in sfxSources
        )
        {
            if (source != null)
            {
                source.volume =
                    sfxVolume;
            }
        }
    }

   

    public void SetFullscreen(
        bool fullscreen
    )
    {
        Screen.fullScreen =
            fullscreen;

        PlayerPrefs.SetInt(
            "Fullscreen",
            fullscreen ? 1 : 0
        );

        PlayerPrefs.Save();
    }

   

    public void AddSFXSource(
        AudioSource source
    )
    {
        if (
            source != null &&
            !sfxSources.Contains(
                source
            )
        )
        {
            sfxSources.Add(source);

            source.volume =
                sfxVolume;
        }
    }

   
    public void RemoveSFXSource(
        AudioSource source
    )
    {
        if (
            source != null &&
            sfxSources.Contains(
                source
            )
        )
        {
            sfxSources.Remove(
                source
            );
        }
    }

   
    public bool IsPointerOverUI()
    {
        return EventSystem.current
            .IsPointerOverGameObject();
    }
}


