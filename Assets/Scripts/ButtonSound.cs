using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource clickSound;

    public void PlayClickSound()
    {
        clickSound.Play();
    }
}
