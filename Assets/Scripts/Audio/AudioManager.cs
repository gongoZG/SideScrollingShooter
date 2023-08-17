using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音频管理器，负责播放游戏中的所有音效
/// </summary>
public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField] AudioSource sFXPlayer;  // 音效播放器
    const float MIN_PITCH = 0.9f;  // 随机音效的最小音调
    const float MAX_PITCH = 1.1f;  // 随机音效的最大音调

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="audioData"></param>
    public void PlaySFX(AudioData audioData) {
        sFXPlayer.PlayOneShot(audioData.audioClip, audioData.volume);  // 会覆盖掉其他声音，适合音效播放
    }
 
    /// <summary>
    /// 播放随机音调的音效，适合播放连续的普通音效，比如枪声
    /// </summary>
    /// <param name="audioData"></param>
    public void PlayRandomSFX(AudioData audioData) {
        sFXPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlaySFX(audioData);
    }

    /// <summary>
    /// 从多个音效中随机播放一个
    /// </summary>
    /// <param name="audioData"></param>
    public void PlayRandomSFX(AudioData[] audioData) {
        PlayRandomSFX(audioData[Random.Range(0, audioData.Length)]);
    }

}

/// <summary>
/// 音频数据，包含音频剪辑和音量
/// </summary>
[System.Serializable]  // 使得该类可以在 Inspector 面板中显示
public class AudioData {
    public AudioClip audioClip;
    public float volume;
}