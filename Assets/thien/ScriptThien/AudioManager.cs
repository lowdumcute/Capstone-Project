using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý toàn bộ âm thanh trong game:
/// - Hiệu ứng va chạm, đánh quái
/// - Nhạc nền bình thường (BG music)
/// - Nhạc boss (boss battle music)
/// Khi hết danh sách nhạc → tự động phát lại từ đầu.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;       // Dùng để phát hiệu ứng (đánh quái, va chạm)
    public AudioSource musicSource;     // Dùng để phát nhạc nền

    [Header("SFX Clips")]
    public AudioClip hitSound;
    public AudioClip tiengchan;


    [Header("SFX Clips Nhan vat khiem")]// Âm thanh khi đánh trúng quái
    public AudioClip chieuE;         // chieu e nhan vat kiem 
    public AudioClip chieuRCmot;
    public AudioClip chieuRChai;


    [Header("SFX Clips Nhan vat khiem")]// am thanh nv cung
    public AudioClip bancung;
   
    [Header("Background Music")]
    public List<AudioClip> bgmList;     // Danh sách nhạc nền
    public List<AudioClip> bossList;    // Danh sách nhạc boss

    [Range(0f, 1f)] public float bgmVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private int currentBgmIndex = 0;
    private bool isBossMode = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        if (musicSource != null && bgmList.Count > 0)
        {
            StartCoroutine(PlayMusicLoop());
        }
    }

    // ===================== MUSIC =====================

    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            if (!musicSource.isPlaying)
            {
                AudioClip nextClip = isBossMode ? GetNextBossClip() : GetNextBgmClip();
                if (nextClip != null)
                {
                    musicSource.clip = nextClip;
                    musicSource.volume = bgmVolume;
                    musicSource.Play();
                }
            }
            yield return null;
        }
    }

    private AudioClip GetNextBgmClip()
    {
        if (bgmList.Count == 0) return null;
        AudioClip clip = bgmList[currentBgmIndex];
        currentBgmIndex = (currentBgmIndex + 1) % bgmList.Count; // khi hết → quay lại đầu
        return clip;
    }

    private AudioClip GetNextBossClip()
    {
        if (bossList.Count == 0) return null;
        AudioClip clip = bossList[currentBgmIndex];
        currentBgmIndex = (currentBgmIndex + 1) % bossList.Count;
        return clip;
    }

    // Chuyển qua nhạc boss
    public void StartBossMusic()
    {
        isBossMode = true;
        currentBgmIndex = 0;
        musicSource.Stop();
    }

    // Quay lại nhạc nền bình thường
    public void StopBossMusic()
    {
        isBossMode = false;
        currentBgmIndex = 0;
        musicSource.Stop();
    }

    // ===================== SFX =====================

    public void PlayHitSound()
    {
        if (hitSound && sfxSource)
            sfxSource.PlayOneShot(hitSound, sfxVolume);
    }

    public void tiengchannhanvat()
    {
        if (tiengchan && sfxSource)
            sfxSource.PlayOneShot(tiengchan, sfxVolume);
    }
    public void chieuEnhanvatkiem() 
    {
        if (chieuE && sfxSource)
            sfxSource.PlayOneShot(chieuE, sfxVolume);
    }
    public void RCmot()
    {
        if (chieuRCmot && sfxSource)
            sfxSource.PlayOneShot(chieuRCmot, sfxVolume);
    }
    public void RCHai()
    {
        if (chieuRChai && sfxSource)
            sfxSource.PlayOneShot(chieuRChai, sfxVolume);
    }
    // ===================== SFX cung =====================
    public void chieubancung()
    {
        if (bancung && sfxSource)
            sfxSource.PlayOneShot(bancung, sfxVolume);
    }
  

    public void PlayCustomSFX(AudioClip clip)
    {
        if (clip && sfxSource)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    /// <summary>
    /// Dừng tất cả hiệu ứng âm thanh hiện tại (SFX)
    /// </summary>
    public void StopSFX()
    {
        if (sfxSource != null)
            sfxSource.Stop();
    }
}
