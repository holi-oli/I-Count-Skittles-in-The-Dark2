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

    // Verhindert Weiterklicks
    private bool blockInput = false;

    void Start()
    {
        // =========================
        // SETTINGS PANEL
        // =========================

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Sicherheitshalber
        Time.timeScale = 1f;

        // =========================
        // AUDIO SETTINGS LADEN
        // =========================

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

        // =========================
        // MUSIC SLIDER
        // =========================

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

        // =========================
        // SFX SLIDER
        // =========================

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

        // =========================
        // FULLSCREEN
        // =========================

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

        // =========================
        // STARTWERTE ANWENDEN
        // =========================

        ApplyMusicVolume();
        ApplySFXVolume();
    }

    void Update()
    {
        // Input blockiert?
        if (blockInput)
            return;

        // ESC öffnet/schließt Menü
        if (
            Input.GetKeyDown(
                KeyCode.Escape
            )
        )
        {
            ToggleSettings();
        }
    }

    // =========================
    // SETTINGS PANEL
    // =========================

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

        // Klick blockieren
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

    // =========================
    // INPUT BLOCK
    // =========================

    IEnumerator BlockClick()
    {
        blockInput = true;

        // Warten bis Maus losgelassen
        yield return new WaitUntil(
            () => !Input.GetMouseButton(0)
        );

        // Extra Sicherheitsframe
        yield return null;

        blockInput = false;
    }

    // =========================
    // GAME PAUSE
    // =========================

    void PauseGame()
    {
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    // =========================
    // MUSIC
    // =========================

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

    // =========================
    // SFX
    // =========================

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

    // =========================
    // FULLSCREEN
    // =========================

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

    // =========================
    // OPTIONAL:
    // SFX SOURCE HINZUFÜGEN
    // =========================

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

    // =========================
    // OPTIONAL:
    // SFX SOURCE ENTFERNEN
    // =========================

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

    // =========================
    // OPTIONAL:
    // UI BLOCKT DIALOG INPUT
    // =========================

    public bool IsPointerOverUI()
    {
        return EventSystem.current
            .IsPointerOverGameObject();
    }
}


