using UnityEngine;
using System.Collections.Generic;

public class Audio : MonoBehaviour
{
    public static Audio instances;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<AudioClip> MusicList;
    [SerializeField] private List<AudioClip> SFXList;
    private Dictionary<string, AudioClip> MusicDictionaries = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> SFXDictionaries = new Dictionary<string, AudioClip>();

    public string currentMusic { get; private set; }

    private void Awake()
    {
        instances = this;

        foreach(AudioClip c in MusicList)
            MusicDictionaries.Add(c.name, c);

        foreach(AudioClip c in SFXList)
            SFXDictionaries.Add(c.name, c);

        //Will play the first song on the LIST
        PlayMusic(MusicList[0].name);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusic(string tag)
    {
        if (!MusicDictionaries.ContainsKey(tag))
            return;

        musicSource.clip = MusicDictionaries[tag];
        currentMusic = tag;
        musicSource.Play();
    }

    public void PlaySFX(string tag)
    {
        if (!SFXDictionaries.ContainsKey(tag))
            return;

        sfxSource.PlayOneShot(SFXDictionaries[tag], 0.5f);
    }
}
