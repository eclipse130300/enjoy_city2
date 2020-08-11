using UnityEngine;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    private Loader loader;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        loader = Loader.Instance;
        loader.AllSceneLoaded += PlayBackgoundMusic;
    }

    private void PlayBackgoundMusic()
    {
        var currentConfig = loader.curentScene;
        audioSource.clip = currentConfig.backGruondMusic;
        audioSource.Play();

    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        loader.AllSceneLoaded -= PlayBackgoundMusic;
    }
}
