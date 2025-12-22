using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Name : MonoBehaviour
{

    public Text displayText;
    private string currentInput = "";

    public string CurrentInput => currentInput; 

    void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (currentInput.Length > 0)
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
            else if (!char.IsControl(c)) 
            {
                currentInput += c;
            }

            displayText.text = currentInput;
        }
    }
}
