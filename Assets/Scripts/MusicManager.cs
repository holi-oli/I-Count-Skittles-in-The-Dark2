using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip music;

        [Range(0.5f, 1f)]
        public float volumeMultiplier = 1f;
    }

    [Header("Scene Music")]
    public List<SceneMusic> musicList;

    [Header("Settings")]
    public float fadeDuration = 2f;

    [Range(0f, 1f)]
    public float masterVolume = 0.4f;

    private Dictionary<string, SceneMusic> map;

    private AudioSource a;
    private AudioSource b;

    private AudioSource active;

    private float currentMultiplier = 1f;

    void Awake()
    {
        

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

       

        masterVolume =
            PlayerPrefs.GetFloat(
                "MusicVolume",
                0.4f
            );

        masterVolume =
            Mathf.Clamp(
                masterVolume,
                0f,
                1f
            );

     

        GameObject goA =
            new GameObject("Music_A");

        GameObject goB =
            new GameObject("Music_B");

        goA.transform.parent = transform;
        goB.transform.parent = transform;

        a = goA.AddComponent<AudioSource>();
        b = goB.AddComponent<AudioSource>();

        SetupSource(a);
        SetupSource(b);

        active = a;

        

        BuildMap();

        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }

    void Start()
    {
        PlayForScene(
            SceneManager
            .GetActiveScene()
            .name
        );
    }

    void SetupSource(AudioSource source)
    {
        source.loop = true;

        source.playOnAwake = false;

       
        source.spatialBlend = 0f;

        source.volume = 0f;
    }

    void BuildMap()
    {
        map =
            new Dictionary<string, SceneMusic>();

        foreach (var m in musicList)
        {
            if (
                m.music != null &&
                !map.ContainsKey(m.sceneName)
            )
            {
                map.Add(
                    m.sceneName,
                    m
                );
            }
        }
    }

    void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        PlayForScene(scene.name);
    }

    void PlayForScene(string sceneName)
    {
        if (map == null)
            return;

        if (
            !map.TryGetValue(
                sceneName,
                out SceneMusic data
            )
        )
        {
            Debug.LogWarning(
                "Keine Musik für: " +
                sceneName
            );

            return;
        }

        PlayMusic(
            data.music,
            data.volumeMultiplier
        );
    }

    public void PlayMusic(
        AudioClip clip,
        float multiplier
    )
    {
        if (clip == null)
            return;

        if (
            active != null &&
            active.clip == clip &&
            active.isPlaying
        )
        {
            return;
        }

        currentMultiplier = multiplier;

        AudioSource next =
            (active == a)
            ? b
            : a;

        next.clip = clip;

        next.volume = 0f;

        next.Play();

        StopAllCoroutines();

        StartCoroutine(
            CrossFade(
                next
            )
        );
    }

    IEnumerator CrossFade(
        AudioSource next
    )
    {
        AudioSource old = active;

        active = next;

        float t = 0f;

        float targetVolume =
            masterVolume *
            currentMultiplier;

        float oldStartVolume =
            old != null
            ? old.volume
            : 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float f =
                t / fadeDuration;

            
            next.volume =
                Mathf.Lerp(
                    0f,
                    targetVolume,
                    f
                );

            
            if (
                old != null &&
                old.isPlaying
            )
            {
                old.volume =
                    Mathf.Lerp(
                        oldStartVolume,
                        0f,
                        f
                    );
            }

            yield return null;
        }

        next.volume = targetVolume;

        if (
            old != null &&
            old.isPlaying
        )
        {
            old.Stop();
        }
    }

   

    public void SetVolume(float volume)
    {
        masterVolume =
            Mathf.Clamp(
                volume,
                0f,
                1f
            );

        PlayerPrefs.SetFloat(
            "MusicVolume",
            masterVolume
        );

        PlayerPrefs.Save();

       
        if (active != null)
        {
            active.volume =
                masterVolume *
                currentMultiplier;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }

}

