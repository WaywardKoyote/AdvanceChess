using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMusicPlayer : MonoBehaviour
{
    public AudioSource bgMusic;

    public static BGMusicPlayer Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayBGMusic()
    {
        bgMusic.Play();
    }

    public void StopBGMusic()
    {
        bgMusic.Stop();
    }

}
