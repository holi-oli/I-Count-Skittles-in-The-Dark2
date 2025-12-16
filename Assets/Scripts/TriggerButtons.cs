using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriggerButtons : MonoBehaviour
{
    [System.Serializable]
    public class ButtonTrigger
    {
        public string[] triggerTexts;
        public string button1Text;
        public string button2Text;
    }

    [Header("Dialogue Manager")]
    public VisualNovelScript dialogueManager;

    [Header("UI")]
    public Button button1;
    public Button button2;
    public TMP_Text button1Label;
    public TMP_Text button2Label;

    [Header("Settings")]
    public float delayBeforeButtons = 2f;

    [Header("Triggers")]
    public ButtonTrigger[] triggers;

    private bool buttonsActive = false;
    private Coroutine triggerCoroutine;
    private ButtonTrigger pendingTrigger;

    void Start()
    {
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);

        button1.onClick.AddListener(OnAnyButtonClicked);
        button2.onClick.AddListener(OnAnyButtonClicked);

        dialogueManager.OnLineFinished += HandleLineFinished;
    }

    void HandleLineFinished(string line)
    {
        if (buttonsActive) return;

        foreach (var trigger in triggers)
        {
            foreach (var t in trigger.triggerTexts)
            {
                if (!string.IsNullOrEmpty(t) && line.Contains(t))
                {
                    pendingTrigger = trigger;

                    // 🔒 Space SOFORT deaktivieren nach Triggertext
                    dialogueManager.inputLocked = true;

                    if (triggerCoroutine != null)
                        StopCoroutine(triggerCoroutine);

                    triggerCoroutine = StartCoroutine(WaitAndShowButtons());
                    return;
                }
            }
        }
    }

    IEnumerator WaitAndShowButtons()
    {
        // ⏱ 2 Sekunden warten (Text sichtbar)
        yield return new WaitForSeconds(delayBeforeButtons);

        // 🧹 Triggertext entfernen
        dialogueManager.dialogueText.text = "";

        yield return new WaitForSeconds(0.1f);

        ShowButtons(pendingTrigger);
    }

    void ShowButtons(ButtonTrigger trigger)
    {
        buttonsActive = true;

        button1Label.text = trigger.button1Text;
        button2Label.text = trigger.button2Text;

        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
    }

    void OnAnyButtonClicked()
    {
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);

        buttonsActive = false;

        // 🔓 Space wieder erlauben
        dialogueManager.inputLocked = false;

        // 🚀 sofort weiter
        dialogueManager.ForceNextLine();
    }
}
