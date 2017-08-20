using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    public enum AudioChannel {Master, Sfx, Music};

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;
    int musicSourceCount = 2;

    SoundLibrary library;

    void Awake()
    {
        musicSources = new AudioSource[musicSourceCount];
        for (int i = 0; i < musicSourceCount; i++)
        {
            GameObject newMusicSource = new GameObject ("Music source " + (i + 1));
            newMusicSource.hideFlags = HideFlags.DontSave;
            musicSources [i] = newMusicSource.AddComponent<AudioSource> ();
            newMusicSource.transform.parent = transform;
        }

        GameObject newSfx2Dsource = new GameObject ("2D sfx source");
        newSfx2Dsource.hideFlags = HideFlags.DontSave;
        sfx2DSource = newSfx2Dsource.AddComponent<AudioSource> ();
        newSfx2Dsource.transform.parent = transform;

        masterVolumePercent = PlayerPrefs.GetFloat ("master vol", 1);
        sfxVolumePercent = PlayerPrefs.GetFloat ("sfx vol", 1);
        musicVolumePercent = PlayerPrefs.GetFloat ("music vol", 1);
    }

    public void InitLibrary()
    {
        GameObject soundLibObj = Resources.Load("SoundLibrary") as GameObject;
        library = soundLibObj.GetComponent<SoundLibrary>();
        if (library)
        {
            library.InitLibrary();
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        musicSources [0].volume = musicVolumePercent * masterVolumePercent;
        musicSources [1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat ("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat ("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat ("music vol", musicVolumePercent);
        PlayerPrefs.Save ();

        Debug.Log("set voluem : " + volumePercent + " channel : " + channel);
    }

    public SoundLibrary GetSoundLib()
    {
        return library;
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        activeMusicSourceIndex = Mathf.Min(activeMusicSourceIndex, musicSourceCount);

        musicSources [activeMusicSourceIndex].clip = clip;
        musicSources [activeMusicSourceIndex].Play ();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlayRandomMusic(float fadeDuration = 1)
    {
        AudioClip audioClip = library.GetClipFromName("Bgm");

        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        activeMusicSourceIndex = Mathf.Min(activeMusicSourceIndex, musicSourceCount);

        musicSources [activeMusicSourceIndex].clip = audioClip;
        musicSources [activeMusicSourceIndex].Play ();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint (clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound (library.GetClipFromName (soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot (library.GetClipFromName (soundName), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources [activeMusicSourceIndex].volume = Mathf.Lerp (0, musicVolumePercent * masterVolumePercent, percent);
            musicSources [1-activeMusicSourceIndex].volume = Mathf.Lerp (musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
