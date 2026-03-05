using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    [Header("Música")]
    public AudioClip musicaMenu;
    public AudioClip musicaJuego;

    [Range(0f, 1f)]
    public float volumen = 0.3f;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton: solo una instancia persiste entre escenas
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = volumen;
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Cambiar música según la escena
        if (scene.buildIndex == 0 && musicaMenu != null) // Cortinilla 
        {
            PlayMusic(musicaMenu);
        }
        else if (scene.buildIndex == 1 && musicaJuego != null) // Game
        {
            PlayMusic(musicaJuego);
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}
