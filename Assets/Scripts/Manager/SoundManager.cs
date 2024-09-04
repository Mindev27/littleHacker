using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum SoundType
{
    BGM,
    EFFECT,
}

public class SoundManager : Singleton<SoundManager>
{
    private AudioMixer audioMixer;
    private float currentBGMVolume, currentEffectVolume;
    private Dictionary<string, AudioClip> clipsDic;
    private SoundBox soundBox;
    private List<TemporaySoundPlayer> instantiatedSounds;

    // �ʱ� ���� ���� ���ҽ� �������� �ҷ��� �ʱⰪ ���� ���� ũ�⸦ ���̱� ���� ������ȭ
    public void Start()
    {
        soundBox = Resources.Load<SoundBox>("Sound/Sounds");
        audioMixer = soundBox.audioMixer;
        clipsDic = new Dictionary<string, AudioClip>();

        foreach (AudioClip clip in soundBox.preLoadClips)
        {
            clipsDic.Add(clip.name, clip);
        }
        instantiatedSounds = new List<TemporaySoundPlayer>();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        instantiatedSounds.Clear();
        if (scene.name == "1.StartScene")
        {
            PlaySound2D("Temp_BGM", 0, true, SoundType.BGM);
        }
    }

    private AudioClip GetClip(string clipName)
    {
        AudioClip clip = clipsDic[clipName];

        if (clip == null)
        {
            Debug.LogError(clipName + "is not find");
            return null;
        }

        return clip;
    }

    private void AddToList(TemporaySoundPlayer soundPlayer)
    {
        instantiatedSounds.Add(soundPlayer);
    }

    public void StopLoopSound(string clipName)
    {
        foreach (TemporaySoundPlayer audioPlayer in instantiatedSounds)
        {
            if (audioPlayer.ClipName == clipName)
            {
                instantiatedSounds.Remove(audioPlayer);
                Destroy(audioPlayer.gameObject);
                return;
            }
        }
        Debug.LogError(clipName + "is not find (StopLoopSound)");
    }

    public void PlaySound2D(string clipName, float delay = 0f, bool isLoop = false, SoundType type = SoundType.EFFECT)
    {
        GameObject soundObj = new GameObject("TemporarySoundPlayer 2D");
        TemporaySoundPlayer soundPlayer = soundObj.AddComponent<TemporaySoundPlayer>();

        if (isLoop)
        {
            AddToList(soundPlayer);
        }
        soundPlayer.InitSound2D(GetClip(clipName));
        soundPlayer.Play(audioMixer.FindMatchingGroups(type.ToString())[0], delay, isLoop);
    }

    public void InitVoumes(float bgm, float effect)
    {
        SetVolumes(SoundType.BGM, bgm);
        SetVolumes(SoundType.EFFECT, effect);
    }

    public void SetVolumes(SoundType type, float value)
    {
        audioMixer.SetFloat(type.ToString(), value);
    }
}
