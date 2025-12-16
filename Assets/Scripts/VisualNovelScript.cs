using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class VisualNovelScript : MonoBehaviour
{
    [Header("UI Elements")]
    public Text dialogueText;
    public float typingSpeed = 0.05f;

    [Header("Dialogue")]
    public string[] dialogueLines;

    [Header("Audio")]
    public AudioSource typeSound;
    public bool playSound = true;

    [HideInInspector] public bool inputLocked = false;
    [HideInInspector] public bool isTyping = false;

    public Action<string> OnLineFinished;

    private int currentLine = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        ShowNextLine();
    }

    void Update()
    {
        if (inputLocked) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                SkipTyping();          // ⬅ Stufe 1
            else
                ShowNextLine();
        }
    }

    public void ShowNextLine()
    {
        if (currentLine >= dialogueLines.Length)
        {
            dialogueText.text = "";
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(dialogueLines[currentLine]));
        currentLine++;
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;

            if (playSound && typeSound != null)
                typeSound.Play();

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        OnLineFinished?.Invoke(line);
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        string line = dialogueLines[currentLine - 1];
        dialogueText.text = line;
        isTyping = false;

        OnLineFinished?.Invoke(line);
    }

    public void ForceNextLine()
    {
        inputLocked = false;
        ShowNextLine();
    }
}
