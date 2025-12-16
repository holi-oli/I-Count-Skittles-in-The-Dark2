using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnEnter : MonoBehaviour
{
    [SerializeField] private int nextSceneIndex; // im Inspector einstellen

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
