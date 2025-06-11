using EntityService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public partial class SoundManager : MonoBehaviour {
    private static SoundManager m_instance;
    public static SoundManager Instance {
        get {
            if (m_instance == null) {
                var go = new GameObject(nameof(SoundManager));
                m_instance = go.AddComponent<SoundManager>();
                if (musicSource == null) {
                    musicSource = go.GetComponent<AudioSource>();
                    if (musicSource == null) {
                        musicSource = go.AddComponent<AudioSource>();
                    }

                    if (m_instance.efxSource == null) {
                        m_instance.efxSource = go.AddComponent<AudioSource>();
                        m_instance.efxSource.spatialBlend = 0;   //2D
                        m_instance.efxSource.reverbZoneMix = 0;
                        m_instance.efxSource.dopplerLevel = 0;
                    }
                }

                musicSource = m_instance.ConfigureAudioSource(musicSource);
                DontDestroyOnLoad(m_instance);
            }
            return m_instance;
        }
    }

    private readonly Dictionary<AudioClip, float> playingAudioDic = new Dictionary<AudioClip, float>();
    private const float PLAY_MIN_TIME = 0.1f;

    [Header("배경음 설정")]

    [Tooltip("배경음 On/Off")]
    [SerializeField]
    private bool m_bgmOn = true;
    public float BGMVolume = 1f;
    private Action m_bgmEndCallback;

    [Tooltip("Target Group 배경음 신호를 위한 설정, 사용 안 할경우 비워놓으면 됨.")]
    [SerializeField]
    private AudioMixerGroup _musicMixerGroup = null;

    [Tooltip("배경음 볼륨믹서 명")]
    [SerializeField]
    private string _volumeOfMusicMixer = string.Empty;

    [Space(3)]
    [Header("효과음 설정")]

    [Tooltip("효과음 On/Off")]
    [SerializeField]
    private bool m_efxFxOn = true;

    [Tooltip("효과음 볼륨")]
    [Range(0, 1)]
    [SerializeField] private float efxFxVolume = 1f;

    [Tooltip("시작 시 효과음 사용여부")]
    [SerializeField] private bool _useSfxVolOnStart = false;

    [Tooltip("Target Group 효과음 신호를 위한 설정, 사용 안 할경우 비워놓으면 됨.")]
    [SerializeField] private AudioMixerGroup _soundFxMixerGroup = null;

    [Tooltip("효과음 볼륨믹서 명")]
    [SerializeField] private string _volumeOfSFXMixer = string.Empty;

    private readonly List<AudioClip> _playlist = new List<AudioClip>();
    public AudioSource efxSource = null;
    public bool IsDebugLog = true;

    // 효과음 풀링을 위한 리스트
    private readonly List<SoundEffect> sfxPool = new List<SoundEffect>();

    // 오디오 매니저 배경음
    private static BackgroundMusic backgroundMusic;

    // 현재 오디오소스와 페이드를 위한 다음 오디오소스
    public static AudioSource musicSource = null;
    public static AudioSource crossfadeSource = null;

    // 현재 볼륨들과 제한 수치용 변수
    private static float currentMusicVol = 0;
    private static float currentSfxVol = 0;
    private static float musicVolCap = 0;
    private static readonly float savedPitch = 1f;

    // On/Off 변수
    private static bool musicOn = false, sfxOn = false;

    // 전환시간 변수
    private static float transitionTime;

    public float ThemeTransitionDuration = 1f;
    public float BattleTransitionDuration = 0.3f;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private AudioSource ConfigureAudioSource(AudioSource audioSource)
    {
        audioSource.outputAudioMixerGroup = _musicMixerGroup;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;   //2D
        audioSource.reverbZoneMix = 0;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.loop = true;
        audioSource.volume = BGMVolume;
        audioSource.mute = !m_bgmOn;

        return audioSource;
    }

    private void ManageSoundEffects()
    {
        for (var i = sfxPool.Count - 1; i >= 0; i--) {
            var sfx = sfxPool[i];
            // 재생 중
            if (sfx.Source.isPlaying && !float.IsPositiveInfinity(sfx.Time)) {
                sfx.Time -= Time.deltaTime;
                sfxPool[i] = sfx;
            }

            // 끝났을 때
            if (sfxPool[i].Time <= 0.0001f || HasPossiblyFinished(sfxPool[i])) {
                sfxPool[i].Source.Stop();
                // 콜백함수 실행
                if (sfxPool[i].Callback != null) {
                    sfxPool[i].Callback.Invoke();
                }

                // 클립 제거 후
                Destroy(sfxPool[i].gameObject);

                // 풀에서 항목빼기
                sfxPool.RemoveAt(i);
                break;
            }
        }
    }

    // 완전히 끝났는 지 체크용 함수
    private bool HasPossiblyFinished(SoundEffect soundEffect)
    {
        return !soundEffect.Source.isPlaying && (FloatEquals(soundEffect.PlaybackPosition, 0) && soundEffect.Time <= 0.09f);
    }

    private bool FloatEquals(float num1, float num2, float threshold = .0001f)
    {
        return Math.Abs(num1 - num2) < threshold;
    }

    /// <summary>
    /// 배경음 볼륨 상태가 변했는지 체크하는 함수
    /// </summary>
    private bool IsMusicAltered()
    {
        var flag = musicOn != m_bgmOn || musicOn != !musicSource.mute || !FloatEquals(currentMusicVol, BGMVolume);

        // 믹서 그룹을 사용할 경우
        if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer.Trim())) {
            _musicMixerGroup.audioMixer.GetFloat(_volumeOfMusicMixer, out var vol);
            vol = NormaliseVolume(vol);

            return flag || !FloatEquals(currentMusicVol, vol);
        }

        return flag;
    }

    /// <summary>
    /// 효과음 볼륨 상태가 변했는지 체크하는 함수
    /// </summary>
    private bool IsSoundFxAltered()
    {
        var flag = m_efxFxOn != sfxOn || !FloatEquals(currentSfxVol, efxFxVolume);

        // 믹서 그룹을 사용할 경우
        if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer.Trim())) {
            _soundFxMixerGroup.audioMixer.GetFloat(_volumeOfSFXMixer, out var vol);
            vol = NormaliseVolume(vol);

            return flag || !FloatEquals(currentSfxVol, vol);
        }

        return flag;
    }

    /// <summary>
    /// 크로스 페이드 인 아웃 함수
    /// </summary>
    private void CrossFadeBackgroundMusic()
    {
        if (backgroundMusic.MusicTransition == Music_Transition.CrossFade) {
            // 전환이 진행중일 경우
            if (musicSource.clip.name != backgroundMusic.NextClip.name) {
                transitionTime -= Time.deltaTime;

                musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime / backgroundMusic.TransitionDuration);

                crossfadeSource.volume = Mathf.Clamp01(musicVolCap - musicSource.volume);
                crossfadeSource.mute = musicSource.mute;

                if (musicSource.volume <= 0.00f) {
                    SetBGMVolume(musicVolCap);
                    PlayBackgroundMusic(backgroundMusic.NextClip, crossfadeSource.time, crossfadeSource.pitch);
                }
            }
        }
    }

    /// <summary> 페이드 인/아웃 함수 </summary>
    private void FadeOutFadeInBackgroundMusic()
    {
        if (backgroundMusic.MusicTransition == Music_Transition.LinearFade) {
            // 페이드 인
            if (musicSource.clip.name == backgroundMusic.NextClip.name) {
                transitionTime += Time.deltaTime;

                musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime / backgroundMusic.TransitionDuration);

                if (musicSource.volume >= musicVolCap) {
                    SetBGMVolume(musicVolCap);
                    PlayBackgroundMusic(backgroundMusic.NextClip, musicSource.time, savedPitch);
                }
            }
            // 페이드 아웃
            else {
                transitionTime -= Time.deltaTime;

                musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime / backgroundMusic.TransitionDuration);

                // 페이드 아웃 끝나는 시점 페이드 인 시작
                if (musicSource.volume <= 0.00f) {
                    musicSource.volume = transitionTime = 0;
                    PlayMusicFromSource(ref musicSource, backgroundMusic.NextClip, 0, musicSource.pitch);
                }
            }
        }
    }

    public bool IsRemainMinTime(AudioClip _clip)
    {
        var isRemain = playingAudioDic.ContainsKey(_clip);
        return isRemain;
    }

    public void AddPlayingList(AudioClip _clip)
    {
        if (playingAudioDic.ContainsKey(_clip) == false) {
            playingAudioDic.Add(_clip, 0);
        }
    }

    /// <summary> 업데이트 함수 용 Enumerator </summary>
    private void Update()
    {
        if (musicSource == null) {
            return;
        }

        //중복으로 들어온 사운드 체크
        var playingClips = playingAudioDic.Keys.ToList();

        for (var i = 0; i < playingClips.Count; ++i) {
            var elapsedTime = playingAudioDic[playingClips[i]];
            elapsedTime += Time.deltaTime;

            //시간 증가시킴
            playingAudioDic[playingClips[i]] = elapsedTime;

            if (playingAudioDic[playingClips[i]] >= PLAY_MIN_TIME) {
                playingAudioDic.Remove(playingClips[i]);
            }
        }

        ManageSoundEffects();

        // 배경음 볼륨 바뀌었나 체크
        if (IsMusicAltered()) {
            // ToggleBGMMute (!musicOn);

            if (!FloatEquals(currentMusicVol, BGMVolume)) {
                currentMusicVol = BGMVolume;
            }

            if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer)) {
                _musicMixerGroup.audioMixer.GetFloat(_volumeOfMusicMixer, out var vol);
                vol = NormaliseVolume(vol);
                currentMusicVol = vol;
            }

            SetBGMVolume(currentMusicVol);
        }

        // 효과음 볼륨 바뀌었나 체크
        if (IsSoundFxAltered()) {
            // ToggleSFXMute (!sfxOn);

            if (!FloatEquals(currentSfxVol, efxFxVolume)) {
                currentSfxVol = efxFxVolume;
            }

            if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer)) {
                _soundFxMixerGroup.audioMixer.GetFloat(_volumeOfSFXMixer, out var vol);
                vol = NormaliseVolume(vol);
                currentSfxVol = vol;
            }

            SetSFXVolume(currentSfxVol);
        }

        // 크로스 페이드일 경우
        if (crossfadeSource != null) {
            CrossFadeBackgroundMusic();
        } else {
            // 페이드 인/ 아웃일 경우
            if (backgroundMusic.NextClip != null) {
                FadeOutFadeInBackgroundMusic();
            }
        }

        if (backgroundMusic.CurrentClip != null && backgroundMusic.EndCall == false) {
            backgroundMusic.ElapsedTime += Time.deltaTime;
            if (backgroundMusic.CurrentClip.length > backgroundMusic.ElapsedTime) {
                backgroundMusic.EndCall = true;
                backgroundMusic.EndCallback?.Invoke();
            }
        }
    }

    public void PlayBGM(string soundName, Music_Transition transition = Music_Transition.Swift, float transition_duration = 1f, float volume = 1f, float pitch = 1f, float playback_position = 0, Action endCallback = null)
    {
        var clip = GetClipFromPlaylist(soundName);
        if (clip == null) {
            ResourceManager.Load<AudioClip>(soundName, (clip) => {
                PlayBGM(clip, transition, transition_duration, volume, pitch, playback_position, endCallback);
            });
            return;
        }

        PlayBGM(clip, transition, transition_duration, volume, pitch, playback_position, endCallback);
    }

    public AudioSource PlaySFX(string soundName, Vector2 location, float duration = 1f, float volume = 1f, bool singleton = false, float pitch = 1f, Action callback = null)
    {
        if (string.IsNullOrEmpty(soundName)) {
            return null;
        }

        var audioClip = GetClipFromPlaylist(soundName);
        if (audioClip == null) {
            ResourceManager.Load<AudioClip>(soundName, (clip) => {
                AddToPlaylist(clip);
                PlaySFX(clip, location, duration, volume, singleton, pitch, callback);
            });
            return null;
        }

        return PlaySFX(audioClip, location, duration, volume, singleton, pitch, callback);
    }

    public AudioSource RepeatSFX(string clip, Vector2 location, int repeat, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
    {
        return RepeatSFX(GetClipFromPlaylist(clip), location, repeat, volume, singleton, pitch, callback);
    }

    public AudioSource PlayOneShot(string clip, Vector2 location, float volume, float pitch = 1f, Action callback = null)
    {
        return PlayOneShot(GetClipFromPlaylist(clip), location, volume, pitch, callback);
    }

    public void PlayOneShot(string soundName, Action callback = null)
    {
        if (string.IsNullOrEmpty(soundName)) {
            return;
        }

        var audioClip = GetClipFromPlaylist(soundName);
        if (audioClip == null) {
            ResourceManager.Load<AudioClip>(soundName, (clip) => {
                AddToPlaylist(clip);
                PlayOneShot(clip);
            });
            return;
        }
        PlayOneShot(audioClip);
    }


    private void PlayMusicFromSource(ref AudioSource audio_source, AudioClip clip, float playback_position, float pitch)
    {
        try {
            audio_source.clip = clip;
            audio_source.time = playback_position;
            audio_source.pitch = Mathf.Clamp(pitch, -3f, 3f);
            audio_source.Play();
        } catch (NullReferenceException nre) {
            Debug.LogError(nre.Message);
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    private void PlayBackgroundMusic(AudioClip clip, float playback_position, float pitch)
    {
        PlayMusicFromSource(ref musicSource, clip, playback_position, pitch);
        // 다음 클립변수에 있는 클립 제거
        backgroundMusic.NextClip = null;
        // 현재 클립변수에 넣어두기
        backgroundMusic.CurrentClip = clip;
        // 크로스페이드에 있는 클립도 비우기
        if (crossfadeSource != null) {
            Destroy(crossfadeSource);
            crossfadeSource = null;
        }
    }

    public void PlayBGM(AudioClip clip, Music_Transition transition, float transition_duration, float volume, float pitch, float playback_position = 0, Action endCallback = null)
    {
        // 요구클립이 없거나 똑같은 클립이면 재생하지 않음.
        if (clip == null || backgroundMusic.CurrentClip == clip) {
            return;
        }

        // 첫 번째로 플레이한 음악이거나 전환시간이 0이면 - 전환효과 없는 케이스
        if (backgroundMusic.CurrentClip == null || transition_duration <= 0) {
            transition = Music_Transition.Swift;
        }

        // 전환효과 없는 케이스 시작
        if (transition == Music_Transition.Swift) {
            PlayBackgroundMusic(clip, playback_position, pitch);
            SetBGMVolume(volume);
        } else {
            // 전환효과 진행중일 때 막음
            if (backgroundMusic.NextClip != null) {
                Debug.LogWarning("Trying to perform a transition on the background music while one is still active");
                return;
            }

            // 전환효과 변수에 전환방법대로 지정, 그 외 변수들도..
            backgroundMusic.MusicTransition = transition;
            transitionTime = backgroundMusic.TransitionDuration = transition_duration;
            musicVolCap = BGMVolume;
            backgroundMusic.NextClip = clip;
            backgroundMusic.EndCallback = endCallback;
            backgroundMusic.EndCall = false;
            backgroundMusic.ElapsedTime = 0;
            // 크로스페이드 처리
            if (backgroundMusic.MusicTransition == Music_Transition.CrossFade) {
                // 전환효과 진행중일 때 막음
                if (crossfadeSource != null) {
                    Debug.LogWarning("Trying to perform a transition on the background music while one is still active");
                    return;
                }

                // 크로스페이드 오디오 초기화
                crossfadeSource = ConfigureAudioSource(gameObject.AddComponent<AudioSource>());
                crossfadeSource.volume = Mathf.Clamp01(musicVolCap - currentMusicVol);
                crossfadeSource.priority = 0;

                PlayMusicFromSource(ref crossfadeSource, backgroundMusic.NextClip, playback_position, pitch);
            }
        }
    }

    public void StopBGM()
    {
        if (musicSource.isPlaying) {
            musicSource.Stop();
        }
    }

    public void PauseBGM()
    {
        if (musicSource.isPlaying) {
            musicSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (!musicSource.isPlaying) {
            musicSource.UnPause();
        }
    }

    private GameObject CreateSoundFx(AudioClip audio_clip, Vector2 location)
    {
        // 임시 오브젝트
        var host = new GameObject("TempAudio");
        host.transform.SetParent(transform);
        host.transform.position = location;
        host.AddComponent<SoundEffect>();

        // 오디오소스 추가
        var audioSource = host.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
        audioSource.reverbZoneMix = 0;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        // 믹서 그룹을 사용할 경우
        audioSource.outputAudioMixerGroup = _soundFxMixerGroup;

        audioSource.clip = audio_clip;
        audioSource.mute = !m_efxFxOn;

        return host;
    }

    /// <summary> 효과음이 효과음 풀에 존재하면 인덱스 알려주는 함수 </summary>
    public int IndexOfSoundFxPool(string name, bool singleton = false)
    {
        var index = 0;
        while (index < sfxPool.Count) {
            if (sfxPool[index].Name == name && singleton == sfxPool[index].Singleton) {
                return index;
            }

            index++;
        }

        return -1;
    }

    public AudioSource PlaySFX(AudioClip clip, Vector2 location, float duration, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
    {
        if (duration <= 0 || clip == null) {
            return null;
        }

        var index = IndexOfSoundFxPool(clip.name, true);

        if (index >= 0) {
            // 효과음 풀에 존재하면 재생시간 재설정해서 내보냄
            var singletonSFx = sfxPool[index];
            singletonSFx.Duration = singletonSFx.Time = duration;
            sfxPool[index] = singletonSFx;

            return sfxPool[index].Source;
        }

        GameObject host = null;
        AudioSource source = null;

        host = CreateSoundFx(clip, location);
        source = host.GetComponent<AudioSource>();
        source.loop = duration > clip.length;
        source.volume = efxFxVolume * volume;
        source.pitch = pitch;

        // 재사용 가능한 사운드 생성
        var sfx = host.GetComponent<SoundEffect>();
        sfx.Singleton = singleton;
        sfx.Source = source;
        sfx.OriginalVolume = volume;
        sfx.Duration = sfx.Time = duration;
        sfx.Callback = callback;

        // 풀에 넣는다.
        sfxPool.Add(sfx);

        source.Play();

        return source;
    }

    public AudioSource RepeatSFX(AudioClip clip, Vector2 location, int repeat, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
    {
        if (clip == null) {
            return null;
        }
        // Debug.Log(clip.name);

        if (repeat != 0) {
            var index = IndexOfSoundFxPool(clip.name, true);

            if (index >= 0) {
                // 효과음 풀에 존재하면 재생시간 재설정해서 내보냄
                var singletonSFx = sfxPool[index];
                singletonSFx.Duration = singletonSFx.Time = repeat > 0 ? clip.length * repeat : float.PositiveInfinity;
                sfxPool[index] = singletonSFx;

                return sfxPool[index].Source;
            }

            var host = CreateSoundFx(clip, location);
            var source = host.GetComponent<AudioSource>();
            source.loop = repeat != 0;
            source.volume = efxFxVolume * volume;
            source.pitch = pitch;

            // 재사용 가능한 사운드 생성
            var sfx = host.GetComponent<SoundEffect>();
            sfx.Singleton = singleton;
            sfx.Source = source;
            sfx.OriginalVolume = volume;
            sfx.Duration = sfx.Time = repeat > 0 ? clip.length * repeat : float.PositiveInfinity;
            sfx.Callback = callback;

            // 풀에 넣는다.
            sfxPool.Add(sfx);

            source.Play();

            return source;
        }

        // repeat 길이가 1보다 작거나 같으면 재생
        return PlayOneShot(clip, location, volume, pitch, callback);
    }

    public AudioSource PlayOneShot(AudioClip clip, Vector2 location, float volume, float pitch = 1f, Action callback = null)
    {
        if (clip == null) {
            return null;
        }

        var host = CreateSoundFx(clip, location);
        var source = host.GetComponent<AudioSource>();
        source.loop = false;
        source.volume = efxFxVolume * volume;
        source.pitch = pitch;

        // 재사용 가능한 사운드 생성
        var sfx = host.GetComponent<SoundEffect>();
        sfx.Singleton = false;
        sfx.Source = source;
        sfx.OriginalVolume = volume;
        sfx.Duration = sfx.Time = clip.length;
        sfx.Callback = callback;

        // 풀에 넣는다.
        sfxPool.Add(sfx);

        source.Play();

        return source;
    }

    public void PlayOneShot(AudioClip clip, Action callback = null)
    {
        if (m_efxFxOn) {
            if (clip == null) {
                return;
            }

            //check minimum time
            if (IsRemainMinTime(clip) == false) {
                if (IsDebugLog) {
                    //StartCoroutine(DebugLog(clip));
                    var audioSource = PlaySFX(clip, gameObject.transform.position, clip.length, EfxVolume);
                    AddPlayingList(clip);
                } else {
                    efxSource.PlayOneShot(clip, efxFxVolume);
                    AddPlayingList(clip);
                }
            }
        }

        //return PlayOneShot (clip, Vector2.zero, efxFxVolume, 1f, callback);
    }

    public void PlayOneShot(AudioClip clip, float _delay, Action callback = null)
    {
        if (m_efxFxOn) {
            StartCoroutine(CoDelayPlayOneShot(clip, _delay, callback));
        }
    }

    private IEnumerator CoDelayPlayOneShot(AudioClip clip, float _delay, Action callback = null)
    {
        yield return new WaitForSeconds(_delay);
        PlayOneShot(clip, callback);
    }

    /// <summary> 모든 효과음을 일시정지 </summary>
    public void PauseAllSFX()
    {
        // SoundEffect 다 돌기
        foreach (var sfx in FindObjectsOfType<SoundEffect>()) {
            if (sfx.Source.isPlaying) {
                sfx.Source.Pause();
            }
        }
    }

    /// <summary> 모든 효과음을 다시재생 </summary>
    public void ResumeAllSFX()
    {
        foreach (var sfx in FindObjectsOfType<SoundEffect>()) {
            if (!sfx.Source.isPlaying) {
                sfx.Source.UnPause();
            }
        }
    }

    /// <summary> 모든 효과음을 중지 </summary>
    public void StopAllSFX()
    {
        foreach (var sfx in FindObjectsOfType<SoundEffect>()) {
            if (sfx.Source) {
                sfx.Source.Stop();
                Destroy(sfx.gameObject);
            }
        }

        sfxPool.Clear();
    }


    public void LoadClip(string path, AudioType audio_type, bool add_to_playlist, Action<AudioClip> callback)
    {
        StartCoroutine(LoadAudioClipFromUrl(path, audio_type, (downloadedContent) => {
            if (downloadedContent != null && add_to_playlist) {
                AddToPlaylist(downloadedContent);
            }

            callback.Invoke(downloadedContent);
        }));
    }


    private IEnumerator LoadAudioClipFromUrl(string audio_url, AudioType audio_type, Action<AudioClip> callback)
    {
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(audio_url, audio_type)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError) {
                Debug.Log(string.Format("Error downloading audio clip at {0} : {1}", audio_url, www.error));
            }

            callback.Invoke(UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www));
        }
    }

    private void ToggleMute(bool flag)
    {
        ToggleBGMMute(flag);
        ToggleSFXMute(flag);
    }

    private void ToggleBGMMute(bool flag)
    {
        musicOn = m_bgmOn = flag;
        musicSource.mute = !musicOn;
    }

    private void ToggleSFXMute(bool flag)
    {
        sfxOn = m_efxFxOn = flag;

        foreach (var sfx in FindObjectsOfType<SoundEffect>()) {
            sfx.Source.mute = !sfxOn;
        }
    }

    private void SetBGMVolume(float volume)
    {
        try {
            volume = Mathf.Clamp01(volume);
            // 모든 사운드 크기 변수에 할당
            musicSource.volume = currentMusicVol = BGMVolume = volume;

            if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer.Trim())) {
                var mixerVol = -80f + (volume * 100f);
                _musicMixerGroup.audioMixer.SetFloat(_volumeOfMusicMixer, mixerVol);
            }
        } catch (NullReferenceException nre) {
            Debug.LogError(nre.Message);
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    private void SetSFXVolume(float volume)
    {
        try {
            volume = Mathf.Clamp01(volume);
            currentSfxVol = efxFxVolume = volume;

            foreach (var sfx in FindObjectsOfType<SoundEffect>()) {
                sfx.Source.volume = efxFxVolume * sfx.OriginalVolume;
                sfx.Source.mute = !m_efxFxOn;
            }

            if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer.Trim())) {
                var mixerVol = -80f + (volume * 100f);
                _soundFxMixerGroup.audioMixer.SetFloat(_volumeOfSFXMixer, mixerVol);
            }
        } catch (NullReferenceException nre) {
            Debug.LogError(nre.Message);
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    private float NormaliseVolume(float vol)
    {
        vol += 80f;
        vol /= 100f;
        return vol;
    }

    public void ClearAllPreferences()
    {

    }

    public void AddToPlaylist(AudioClip clip)
    {
        if (clip != null && _playlist.Contains(clip) == false) {
            _playlist.Add(clip);
        }
    }

    public AudioClip GetClipFromPlaylist(string clipName)
    {
        for (var i = 0; i < _playlist.Count; i++) {
            if (string.Equals(clipName, _playlist[i].name, StringComparison.CurrentCultureIgnoreCase)) {
                return _playlist[i];
            }
        }

        //Debug.LogWarning(clipName + " does not exist in the playlist.");
        return null;
    }

    /// <summary> 현재 배경음 클립을 가져오는 속성 </summary>
    public AudioClip CurrentMusicClip => backgroundMusic.CurrentClip;

    /// <summary> 효과음 풀을 가져오는 속성 </summary>
    public List<SoundEffect> SoundFxPool => sfxPool;

    /// <summary> 오디오 매니저의 클립 리스트를 가져오는 속성 </summary>
    public List<AudioClip> Playlist => _playlist;

    /// <summary> 배경음이 재생중인지 체크하는 속성 </summary>
    public bool IsMusicPlaying => musicSource != null && musicSource.isPlaying;

    /// <summary> 배경음 사운드 크기를 가져오거나 지정하는 속성 </summary>
    public float BgmVolume {
        get => BGMVolume;
        set => SetBGMVolume(value);
    }

    /// <summary> 효과음 사운드 크기를 가져오거나 지정하는 속성 </summary>
    public float EfxVolume {
        get => efxFxVolume;
        set => SetSFXVolume(value);
    }

    /// <summary> 배경음 On/Off 체크하거나 지정하는 속성 </summary>
    public bool IsBgmOn {
        get => m_bgmOn;
        set => ToggleBGMMute(value);
    }

    /// <summary> 효과음 On/Off 체크하거나 지정하는 속성 </summary>
    public bool IsEfxOn {
        get => m_efxFxOn;
        set => ToggleSFXMute(value);
    }

    /// <summary> 배경음과 효과음 On/Off 체크하거나 지정하는 속성 </summary>
    public bool IsMasterMute {
        get => !m_bgmOn && !m_efxFxOn;
        set => ToggleMute(value);
    }

    public IEnumerator DebugLog(AudioClip audioClip)
    {
        if (audioClip == null) {
            yield break;
        }

        var audioSource = PlaySFX(audioClip, gameObject.transform.position, audioClip.length, EfxVolume);
        AddPlayingList(audioClip);
        var name = audioClip.name;
        var now = DateTime.Now;
        GameLogger.Log.Info("SoundManager", $"{name} - On - {now:HH:mm:ss.fff}", GameLogger.LogColor.Olive);
        float e = 0f;
        while (audioSource.isPlaying) {
            yield return null;
            e += Time.deltaTime;
            if (e < 5f) {
                break;
            }
        }
        var stopTime = DateTime.Now;
        var duration = stopTime - now;
        GameLogger.Log.Info("SoundManager", $"{name} - Off - {stopTime:HH:mm:ss.fff} -> milliseconds:{duration.TotalMilliseconds}", GameLogger.LogColor.Olive);
    }
}

public enum Music_Transition {
    Swift,
    LinearFade,
    CrossFade
}

[Serializable]
public struct BackgroundMusic {
    public AudioClip CurrentClip;
    public AudioClip NextClip;
    public Music_Transition MusicTransition;
    public float TransitionDuration;
    public Action EndCallback;
    public bool EndCall;
    public float ElapsedTime;
}

[Serializable]
public class SoundEffect : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float originalVolume;
    [SerializeField] private float duration;
    [SerializeField] private float time;
    [SerializeField] private Action callback;
    [SerializeField] private bool singleton;

    /// <summary> 효과음 이름 속성 </summary>
    public string Name => audioSource.clip.name;

    /// <summary> 효과음 길이 속성 (초 단위) </summary>
    public float Length => audioSource.clip.length;

    /// <summary> 효과음 재생된 시간 속성 (초 단위) </summary>
    public float PlaybackPosition => audioSource.time;

    /// <summary> 효과음 클립 속성 </summary>
    public AudioSource Source {
        get => audioSource;
        set => audioSource = value;
    }

    /// <summary> 효과음 원본 볼륨 속성 </summary>
    public float OriginalVolume {
        get => originalVolume;
        set => originalVolume = value;
    }

    /// <summary> 효과음 총 재생시간 속성 (초단위) </summary>
    public float Duration {
        get => duration;
        set => duration = value;
    }

    /// <summary> 효과음 남은 재생시간 속성 (초단위) </summary>
    public float Time {
        get => time;
        set => time = value;
    }

    /// <summary> 효과음 정규화된 재생진행도 속성 (정규화 0~1) </summary>
    public float NormalisedTime => Time / Duration;

    /// <summary> 효과음 완료 시 콜백 액션 속성 </summary>
    public Action Callback {
        get => callback;
        set => callback = value;
    }

    /// <summary> 효과음 반복 시 싱글톤 여부, 반복할 경우에 true 아니면 false </summary>
    public bool Singleton {
        get => singleton;
        set => singleton = value;
    }
}