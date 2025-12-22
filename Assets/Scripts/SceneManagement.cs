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

    [Header("TEXT-TRIGGER END GAME")]
    public string[] endGameTriggerTexts;

    [Header("END GAME SETTINGS")]
    public float endGameDelay = 2f;

    [Header("BUTTON SCENE CHANGE")]
    public string[] buttonSceneNames;
    public int[] buttonSceneIndexes;

    private bool endGameTriggered = false;

    public void LoadSceneByButtonIndex(int buttonId)
    {
        if (buttonId < 0)
        {
            Debug.LogError("Button ID ist kleiner als 0!");
            return;
        }

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
            SceneManager.LoadScene(buttonSceneIndexes[buttonId]);
            return;
        }

        Debug.LogError("Für Button " + buttonId + " ist keine Szene gesetzt!");
    }

    void Update()
    {
        if (dialogueText == null)
            return;

        string currentText = dialogueText.text;

        if (!endGameTriggered && endGameTriggerTexts != null)
        {
            foreach (string trigger in endGameTriggerTexts)
            {
                if (string.IsNullOrEmpty(trigger)) continue;

                if ((exactMatch && currentText == trigger) ||
                    (!exactMatch && currentText.Contains(trigger)))
                {
                    EndGame();
                    return;
                }
            }
        }

        if (triggerTexts == null || scenesToLoad == null)
            return;

        if (triggerTexts.Length != scenesToLoad.Length)
        {
            Debug.LogError("triggerTexts und scenesToLoad müssen gleich lang sein!");
            return;
        }

        for (int i = 0; i < triggerTexts.Length; i++)
        {
            if ((exactMatch && currentText == triggerTexts[i]) ||
                (!exactMatch && currentText.Contains(triggerTexts[i])))
            {
                SceneManager.LoadScene(scenesToLoad[i]);
                return;
            }
        }
    }

    void EndGame()
    {
        if (endGameTriggered) return;

        endGameTriggered = true;
        StartCoroutine(EndGameRoutine());
    }

    IEnumerator EndGameRoutine()
    {
        Debug.Log("End Game in " + endGameDelay + " Sekunden...");

        yield return new WaitForSeconds(endGameDelay);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
