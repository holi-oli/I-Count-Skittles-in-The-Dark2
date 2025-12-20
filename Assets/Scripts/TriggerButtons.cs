using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TriggerButtons : MonoBehaviour
{
    public enum TriggerAction
    {
        ShowButtons,
        PlayVideo
    }

    public enum ButtonCount
    {
        One = 1,
        Two = 2
    }

    [System.Serializable]
    public class ButtonTrigger
    {
        [Tooltip("Dialog- oder Buttontexte, die diesen Trigger auslösen")]
        public string[] triggerTexts;

        [Header("Action")]
        public TriggerAction action;

        [Header("Buttons (nur bei ShowButtons)")]
        public ButtonCount buttonCount = ButtonCount.One;
        public string button1Text;
        public string button2Text;

        [Header("Video (nur bei PlayVideo)")]
        public VideoClip videoClip;
    }

    /* ---------------- REFERENCES ---------------- */

    [Header("Dialogue Manager")]
    public VisualNovelScript dialogueManager;

    [Header("UI Panels")]
    public GameObject dialoguePanel;
    public GameObject buttonsPanel;

    [Header("Buttons")]
    public Button button1;
    public Button button2;
    public TMP_Text button1Label;
    public TMP_Text button2Label;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    public GameObject videoPanel; // optionaler Hintergrund

    [Header("Settings")]
    public float delayBeforeButtons = 1.5f;

    [Header("Triggers")]
    public ButtonTrigger[] triggers;

    /* ---------------- INTERNAL ---------------- */

    private bool buttonsActive;
    private Coroutine triggerCoroutine;
    private ButtonTrigger pendingTrigger;

    /* ---------------- UNITY ---------------- */

    void Start()
    {
        HideButtons();

        videoRawImage.gameObject.SetActive(false);
        if (videoPanel != null)
            videoPanel.SetActive(false);

        button1.onClick.AddListener(() => OnButtonClicked(button1Label.text));
        button2.onClick.AddListener(() => OnButtonClicked(button2Label.text));

        videoPlayer.loopPointReached += OnVideoFinished;
        dialogueManager.OnLineFinished += HandleLineFinished;
    }

    /* ---------------- TRIGGER ENTRY ---------------- */

    void HandleLineFinished(string line)
    {
        if (buttonsActive) return;
        TryTrigger(line);
    }

    void OnButtonClicked(string buttonText)
    {
        HideButtons();

        if (TryTrigger(buttonText))
            return;

        dialogueManager.ForceNextLine();
    }

    /* ---------------- TRIGGER LOGIC ---------------- */

    bool TryTrigger(string sourceText)
    {
        foreach (var trigger in triggers)
        {
            foreach (var t in trigger.triggerTexts)
            {
                if (!string.IsNullOrEmpty(t) && sourceText.Contains(t))
                {
                    pendingTrigger = trigger;
                    dialogueManager.inputLocked = true;

                    if (triggerCoroutine != null)
                        StopCoroutine(triggerCoroutine);

                    triggerCoroutine = StartCoroutine(ExecuteTrigger());
                    return true;
                }
            }
        }

        dialogueManager.inputLocked = false;
        return false;
    }

    /* ---------------- EXECUTION ---------------- */

    IEnumerator ExecuteTrigger()
    {
        yield return new WaitForSeconds(0.1f);

        switch (pendingTrigger.action)
        {
            case TriggerAction.ShowButtons:
                yield return new WaitForSeconds(delayBeforeButtons);
                dialogueManager.dialogueText.text = "";
                ShowButtons(pendingTrigger);
                break;

            case TriggerAction.PlayVideo:
                PlayVideo(pendingTrigger.videoClip);
                break;
        }
    }

    /* ---------------- VIDEO ---------------- */

    void PlayVideo(VideoClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Kein VideoClip zugewiesen!");
            dialogueManager.inputLocked = false;
            return;
        }

        // UI ausblenden
        dialoguePanel.SetActive(false);
        buttonsPanel.SetActive(false);

        // Video UI einblenden
        videoRawImage.gameObject.SetActive(true);
        if (videoPanel != null)
            videoPanel.SetActive(true);

        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.Stop();

        videoRawImage.gameObject.SetActive(false);
        if (videoPanel != null)
            videoPanel.SetActive(false);

        dialoguePanel.SetActive(true);
        buttonsPanel.SetActive(false);

        dialogueManager.inputLocked = false;
        dialogueManager.ForceNextLine();
    }

    /* ---------------- BUTTONS ---------------- */

    void ShowButtons(ButtonTrigger trigger)
    {
        buttonsActive = true;
        buttonsPanel.SetActive(true);

        button1Label.text = trigger.button1Text;
        button1.gameObject.SetActive(true);

        if (trigger.buttonCount == ButtonCount.Two)
        {
            button2Label.text = trigger.button2Text;
            button2.gameObject.SetActive(true);
        }
        else
        {
            button2.gameObject.SetActive(false);
        }
    }

    void HideButtons()
    {
        buttonsActive = false;
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        buttonsPanel.SetActive(false);
    }
}
