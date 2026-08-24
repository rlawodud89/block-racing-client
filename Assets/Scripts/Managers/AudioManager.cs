using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _gameSceneEnterClip;
    [SerializeField] private AudioClip _gameStartClip;
    [SerializeField] private AudioClip _shootClip;
    [SerializeField] private AudioClip _winClip;
    [SerializeField] private AudioClip _loseClip;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayGameSceneEnter()
    {
        _audioSource.PlayOneShot(_gameSceneEnterClip);
    }

    public void PlayGameStart()
    {
        _audioSource.PlayOneShot(_gameStartClip);
    }

    public void PlayShoot()
    {
        _audioSource.PlayOneShot(_shootClip);
    }

    public void PlayWin()
    {
        _audioSource.PlayOneShot(_winClip);
    }

    public void PlayLose()
    {
        _audioSource.PlayOneShot(_loseClip);
    }
}