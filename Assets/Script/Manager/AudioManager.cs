using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // Assurez-vous d'importer le namespace DOTween

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public bool loop;
        public bool isMusic; // Booléen pour différencier la musique des SFX
        [Range(0f, 1f)]
        public float volume = 1f;
        [HideInInspector]
        public AudioSource source;
    }

    public List<Sound> sounds;
    public float fadeDuration = 1f; // Durée du fondu en secondes

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = 0f; // Commence à 0 volume pour le fade-in
        }
    }

    public void PlaySound(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s != null)
        {
            s.source.volume = 0f; // Assure que le volume commence à 0
            s.source.Play();
            s.source.DOFade(s.volume, fadeDuration); // Fade-in au volume défini
        }
        else
        {
            Debug.LogWarning($"Sound {name} not found!");
        }
    }

    public void StopSound(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s != null)
        {
            if (s.isMusic)
            {
                s.source.DOFade(0, fadeDuration).OnComplete(() => s.source.Stop());
            }
            else
            {
                s.source.Stop();
            }
        }
        else
        {
            Debug.LogWarning($"Sound {name} not found!");
        }
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s != null)
        {
            s.source.volume = volume;
        }
        else
        {
            Debug.LogWarning($"Sound {name} not found!");
        }
    }

    public void StopAllSounds()
    {
        foreach (Sound s in sounds)
        {
            if (s.isMusic)
            {
                s.source.DOFade(0, fadeDuration).OnComplete(() => s.source.Stop());
            }
            else
            {
                s.source.Stop();
            }
        }
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
