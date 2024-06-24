using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
        public float pitch = 1f;
        public bool loop = false;
    }

    public List<Sound> sounds;
    public GameObject audioSourcePrefab;
    public int poolSize = 10;

    private Dictionary<string, Sound> soundDictionary;
    private Queue<AudioSource> audioSourcePool;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundDictionary();
            InitializeAudioSourcePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<string, Sound>();
        foreach (Sound sound in sounds)
        {
            if (!soundDictionary.ContainsKey(sound.name))
            {
                soundDictionary.Add(sound.name, sound);
            }
            else
            {
                Debug.LogWarning($"Sound {sound.name} is already in the dictionary!");
            }
        }
    }

    private void InitializeAudioSourcePool()
    {
        audioSourcePool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
            source.gameObject.SetActive(false);
            audioSourcePool.Enqueue(source);
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            AudioSource source = GetPooledAudioSource();
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.gameObject.SetActive(true);
            source.Play();
            if (!sound.loop)
            {
                StartCoroutine(ReturnToPoolAfterPlaying(source, sound.clip.length));
            }
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found in AudioManager!");
        }
    }

    private AudioSource GetPooledAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            return audioSourcePool.Dequeue();
        }
        else
        {
            AudioSource newSource = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
            return newSource;
        }
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
    }

    public void StopSound(string soundName)
    {
        foreach (AudioSource source in GetComponentsInChildren<AudioSource>())
        {
            if (source.isPlaying && source.clip.name == soundName)
            {
                source.Stop();
                source.gameObject.SetActive(false);
                audioSourcePool.Enqueue(source);
                break;
            }
        }
    }
}
