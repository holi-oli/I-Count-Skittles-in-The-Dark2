using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [Header("TEXT-TRIGGER SCENE CHANGE")]
    public Text dialogueText;
    public string[] triggerTexts;
    public string[] scenesToLoad;
    public bool exactMatch = false;

    [Header("BUTTON SCENE CHANGE")]
    public string[] buttonSceneNames;   // Szene per Name
    public int[] buttonSceneIndexes;    // Szene per Index


    // ----- Button Funktion -----
    public void LoadSceneByButtonIndex(int buttonId)
    {
        // Prüfen ob Index gültig ist
        if (buttonId < 0)
        {
            Debug.LogError("Button ID ist kleiner als 0!");
            return;
        }

        // Priorität: Zuerst Name → dann Index

        if (buttonSceneNames != null && buttonId < buttonSceneNames.Length)
        {
            string sceneName = buttonSceneNames[buttonId];
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
                return;
            }
        }

        if (buttonSceneIndexes != null && buttonId < buttonSceneIndexes.Length)
        {
            int sceneIndex = buttonSceneIndexes[buttonId];
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        Debug.LogError("Für Button " + buttonId + " ist kein gültiger Szenenname oder Szenenindex gesetzt!");
    }


    // ----- Text Trigger -----
    void Update()
    {
        if (dialogueText == null || triggerTexts.Length == 0 || scenesToLoad.Length == 0)
            return;

        if (triggerTexts.Length != scenesToLoad.Length)
        {
            Debug.LogError("triggerTexts und scenesToLoad müssen gleiche Länge haben!");
            return;
        }

        string current = dialogueText.text;

        for (int i = 0; i < triggerTexts.Length; i++)
        {
            string trigger = triggerTexts[i];

            if (exactMatch)
            {
                if (current == trigger)
                    SceneManager.LoadScene(scenesToLoad[i]);
            }
            else
            {
                if (current.Contains(trigger))
                    SceneManager.LoadScene(scenesToLoad[i]);
            }
        }
    }
}
