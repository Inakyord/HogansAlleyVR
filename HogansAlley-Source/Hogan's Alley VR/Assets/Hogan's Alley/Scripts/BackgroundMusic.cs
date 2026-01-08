using UnityEngine;
using UnityEngine.SceneManagement; // Needed to detect scene changes

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    private AudioSource _audioSource;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    // Subscribe to the "SceneLoaded" event
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Unsubscribe to avoid errors
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Adjust this string if your scene is named differently (e.g. "Game")
        if (scene.name == "GameScene") 
        {
            // We are playing! Stop the menu music.
            _audioSource.Stop(); 
        }
        else
        {
            // We are in Menu or GameOver. If not playing, start music.
            if (!_audioSource.isPlaying) 
            {
                _audioSource.Play();
            }
        }
    }
}