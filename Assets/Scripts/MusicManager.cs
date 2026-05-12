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
    public float masterVolume = 1f;

    private Dictionary<string, SceneMusic> map;

    private AudioSource a;
    private AudioSource b;
    private AudioSource active;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        GameObject goA = new GameObject("Music_A");
        GameObject goB = new GameObject("Music_B");

        goA.transform.parent = transform;
        goB.transform.parent = transform;

        a = goA.AddComponent<AudioSource>();
        b = goB.AddComponent<AudioSource>();

        a.loop = true;
        b.loop = true;

        a.playOnAwake = false;
        b.playOnAwake = false;

       
        a.bypassEffects = false;
        b.bypassEffects = false;

       
        a.spatialBlend = 0f;
        b.spatialBlend = 0f;

        active = a;

        BuildMap();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    void BuildMap()
    {
        map = new Dictionary<string, SceneMusic>();

        foreach (var m in musicList)
        {
            if (m.music != null && !map.ContainsKey(m.sceneName))
                map.Add(m.sceneName, m);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name);
    }

    void PlayForScene(string sceneName)
    {
        if (!map.TryGetValue(sceneName, out SceneMusic data))
        {
            Debug.LogWarning("Keine Musik für: " + sceneName);
            return;
        }

        PlayMusic(data.music, data.volumeMultiplier);
    }

    public void PlayMusic(AudioClip clip, float multiplier)
    {
        if (clip == null) return;

        if (active.clip == clip) return;

        AudioSource next = (active == a) ? b : a;

        next.clip = clip;
        next.volume = 0f;
        next.Play();

        StopAllCoroutines();
        StartCoroutine(CrossFade(next, multiplier));
    }

    IEnumerator CrossFade(AudioSource next, float multiplier)
    {
        AudioSource old = active;
        active = next;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float f = t / fadeDuration;

            next.volume = Mathf.Lerp(0, masterVolume * multiplier, f);
            old.volume = Mathf.Lerp(masterVolume, 0, f);

            yield return null;
        }

        next.volume = masterVolume * multiplier;
        old.Stop();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
