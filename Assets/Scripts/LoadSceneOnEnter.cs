using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnEnter : MonoBehaviour
{
    [SerializeField] private int nextSceneIndex;
    [SerializeField] private Name nameInput; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && nameInput.CurrentInput.Length > 0)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
