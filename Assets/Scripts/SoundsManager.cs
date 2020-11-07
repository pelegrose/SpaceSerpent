using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class SoundsManager : MonoBehaviour
{
    // Check to see if we're about to be destroyed.

    private static SoundsManager m_Instance;
    // Audio players components.
    public AudioSource EffectsSource1;
    public AudioSource EffectsSource2;
    public AudioSource EffectsSource3;
    public AudioSource MusicSource;


    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static SoundsManager Instance
    {
        get
        {
            return m_Instance;
        }
    }
    
    private void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(this.gameObject);
        } 
        else 
        {
            m_Instance = this;
        }
    }

    private void Play1(AudioClip clip)
    {
        EffectsSource1.clip = clip;
        EffectsSource1.Play();
    }

    private void Play2(AudioClip clip)
    {
        EffectsSource2.clip = clip;
        EffectsSource2.Play();
    }

    private void Play3(AudioClip clip)
    {
        EffectsSource3.clip = clip;
        EffectsSource3.Play();
    }

    // Play1 a single clip through the music source.
    private void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    public void PlayRandomPaddleHitSound()
    {
        int idx = Random.Range(0, 4);
        switch (idx)
        {
            case 0:
                Play1(Resources.Load<AudioClip>("PaddleHit1"));
                break;
            case 1:
                Play1(Resources.Load<AudioClip>("PaddleHit2"));
                break;
            case 2:
                Play1(Resources.Load<AudioClip>("PaddleHit3"));
                break;
            case 3:
                Play1(Resources.Load<AudioClip>("PaddleHit4"));
                break;
        }
    }

    public void PlayRandomWallHitSound()
    {
        int idx = Random.Range(0, 2);
        switch (idx)
        {
            case 0:
                Play2(Resources.Load<AudioClip>("WallHit1"));
                break;
            case 1:
                Play2(Resources.Load<AudioClip>("WallHit2"));
                break;
        }
    }

    public void PlayLoseRoundSound()
    {
        Play1(Resources.Load<AudioClip>("LostRound"));
    }

    public void PlaySnakeHitSelfSound()
    {
        Play3(Resources.Load<AudioClip>("SnakeHitSelf"));
    }

    public void PlayUsedLastLifeSound()
    {
        Play2(Resources.Load<AudioClip>("LostAllLife"));
    }

    public void PlayEatSound()
    {
        Play3(Resources.Load<AudioClip>("Eat"));
    }

    public void PlayMainGameBGM()
    {
        PlayMusic(Resources.Load<AudioClip>("BGM"));
    }
}