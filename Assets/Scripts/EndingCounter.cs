using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCounter : MonoBehaviour
{
    public static EndingCounter instance;

    public int badChoiceCount = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // bleibt über Szenen hinweg bestehen
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddBadChoice()
    {
        badChoiceCount++;
        Debug.Log("Bad Choices: " + badChoiceCount);
    }
}
