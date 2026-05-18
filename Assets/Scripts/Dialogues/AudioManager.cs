using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private List<TriggerSounds> triggerSounds;

    private Dictionary<string, AudioClip> soundMap;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        soundMap = new Dictionary<string, AudioClip>();

        foreach (var sound in triggerSounds)
        {
            soundMap[sound.triggerSoundName] = sound.triggerSound;
        }
    }

    public void Play(string soundName)
    {
        if (soundMap.TryGetValue(soundName, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound not found: {soundName}");
        }
    }
}
