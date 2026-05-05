using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndingManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public VideoClip goodEndingVideo;
    public VideoClip badEndingVideo;

    public int badEndingThreshold = 5; 

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("Kein VideoPlayer zugewiesen!");
            return;
        }

        int badChoices = 0;

        if (EndingCounter.instance != null)
        {
            badChoices = EndingCounter.instance.badChoiceCount;
        }
        else
        {
            Debug.LogWarning("EndingCounter instance ist null!");
        }

        Debug.Log("Bad Choices Gesamt: " + badChoices);

        
        if (badChoices < badEndingThreshold)
        {
            PlayGoodEnding();
        }
        else
        {
            PlayBadEnding();
        }
    }

    void PlayBadEnding()
    {
        Debug.Log("Schlechtes Ende wird abgespielt");

        if (badEndingVideo != null)
        {
            videoPlayer.clip = badEndingVideo;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("Bad Ending Video fehlt!");
        }
    }

    void PlayGoodEnding()
    {
        Debug.Log("Gutes Ende wird abgespielt");

        if (goodEndingVideo != null)
        {
            videoPlayer.clip = goodEndingVideo;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("Good Ending Video fehlt!");
        }
    }
}
