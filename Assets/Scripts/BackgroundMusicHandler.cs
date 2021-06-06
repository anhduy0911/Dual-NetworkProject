using UnityEngine;

public class BackgroundMusicHandler : MonoBehaviour
{
    private static BackgroundMusicHandler instance = null;
    private AudioSource audioSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        if (instance == this) return;
        Destroy(gameObject);
    }

    void Start()
    {
        audioSound = GetComponent<AudioSource>();
        audioSound.Play();
    }
}
