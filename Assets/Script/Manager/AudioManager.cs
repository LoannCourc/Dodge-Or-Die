using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Audio;

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
    public AudioMixer audioMixer; // Référence à l'AudioMixer
    public string musicVolumeParameter = "MusicVolume"; // Le paramètre du volume de la musique dans l'AudioMixer
    private bool isMusicMuted = false; // Pour suivre l'état de la musique

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
            s.source.volume = s.isMusic ? 0f : s.volume; // Commence à 0 volume pour la musique pour le fade-in

            // Vérifiez si le groupe existe dans l'AudioMixer avant de l'assigner
            AudioMixerGroup[] matchingGroups = audioMixer.FindMatchingGroups(s.isMusic ? "Music" : "Master");
            if (matchingGroups.Length > 0)
            {
                s.source.outputAudioMixerGroup = matchingGroups[0];
                s.source.outputAudioMixerGroup = audioMixer.FindMatchingGroups(s.isMusic ? "Music" : "Master")[0];
            }
            else
            {
                Debug.LogWarning($"AudioMixerGroup {(s.isMusic ? "Music" : "Master")} not found!");
            }
        }
    }
    
    public void PlaySound(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s != null)
        {
            if (s.isMusic)
            {
                if (!s.source.isPlaying)
                {
                    s.source.volume = 0f; // Assure que le volume commence à 0 pour la musique
                    s.source.Play();
                    s.source.DOFade(s.volume, fadeDuration); // Fade-in au volume défini pour la musique
                }
                else
                {
                    s.source.DOFade(s.volume, fadeDuration); // Juste faire un fade-in si la musique est déjà en cours de lecture
                }
            }
            else
            {
                s.source.volume = s.volume;
                s.source.Play();
            }
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

    // Fonction pour basculer la musique
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        float targetVolume = isMusicMuted ? -80f : 0f; // -80dB pour mute, 0dB pour volume normal
        Debug.Log($"Setting Music Volume to: {targetVolume}");
        audioMixer.SetFloat(musicVolumeParameter, targetVolume);
    }
}
