using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgSource;     // Background music
    [SerializeField] private AudioSource sfxSource;    // SFX sounds

    [Header("Audio Clips")]
    public AudioClip bgMusic;
    public AudioClip buttonClick;
    public AudioClip swipeCorrect;
    public AudioClip swipeWrong;
    public AudioClip gameOver;

    private void Awake()
    {
        // ✅ Singleton + Don't Destroy On Load
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);   // ✅ Keeps SoundManager alive across scenes
    }

    private void Start()
    {
        PlayBG();
    }

    // ✅ Background Music
    public void PlayBG()
    {
        if (bgMusic == null) return;

        bgSource.clip = bgMusic;
        bgSource.loop = true;
        bgSource.Play();
    }

    // ✅ Button Click
    public void PlayButton()
    {
        PlaySFX(buttonClick);
    }

    // ✅ Correct Swipe
    public void PlaySwipeCorrect()
    {
        PlaySFX(swipeCorrect);
    }

    // ✅ Wrong Swipe
    public void PlaySwipeWrong()
    {
        PlaySFX(swipeWrong);
    }

    // ✅ Game Over Sound
    public void PlayGameOver()
    {
        PlaySFX(gameOver);
    }

    // ✅ General SFX Player
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
