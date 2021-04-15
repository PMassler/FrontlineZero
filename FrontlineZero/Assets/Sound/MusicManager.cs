using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicAS;
    public AudioClip menuMusic;
    public AudioClip transitionClip;
    public AudioClip inGameMusic;
    public bool isInGame;
    public bool isInGameAfterTransition;


    // Singleton
    private static MusicManager _instance;
    public static MusicManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    // change seamlessly from menu music to ingamemusic
    void Update()
    {
        if (!musicAS.isPlaying)
        {
            if (isInGame)
            {
                if (isInGameAfterTransition)
                {
                    musicAS.clip = inGameMusic;
                    musicAS.Play();
                }
                else
                {
                    musicAS.clip = transitionClip;
                    musicAS.Play();
                    isInGameAfterTransition = true;
                }                
            }
            else
            {
                musicAS.clip = menuMusic;
                musicAS.Play();
            }
        }
    }    
}
