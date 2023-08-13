using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [NonReorderable]
    public Sound[] sounds;
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.audioMixer;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    private void Start()
    {
        Play("MainSoundtrack");
        Play("Wind");
    }

    public void Play (string Name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == Name);
        if(s == null)
        {
            Debug.LogWarning("Sound: " + Name + "not found");
            return;
        }
        s.source.Play();
    }

    public void Stop(string Name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == Name);

        if (s == null) {
            Debug.LogWarning("Sound: " + Name + "not found");
            return;
        }

        s.source.Stop();
    }
}
