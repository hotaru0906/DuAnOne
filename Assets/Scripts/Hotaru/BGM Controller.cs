using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    public static BGMController Instance;
    public AudioSource bgmSource;
    public AudioClip introBGM, castleBGM, forestBGM, bossBGM, gameOverBGM, endingBGM;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (bgmSource == null)
            bgmSource = GetComponent<AudioSource>();
        PlayBGMForScene(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    void PlayBGMForScene(string sceneName)
    {
        if (bgmSource == null) return;

        if (sceneName == "MainMenu" || sceneName == "CutScene1")
        {
            if (bgmSource.clip != introBGM)
            {
                bgmSource.clip = introBGM;
                bgmSource.Play();
            }
            else if (!bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
        }
        else if (sceneName == "Castle" || sceneName == "Village")
        {
            if (bgmSource.clip != castleBGM)
            {
                bgmSource.clip = castleBGM;
                bgmSource.Play();
            }
            else if (!bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
        }
        else if (sceneName == "Forest")
        {
            if (bgmSource.clip != forestBGM)
            {
                bgmSource.clip = forestBGM;
                bgmSource.Play();
            }
        }
        else if (sceneName == "GameOver")
        {
            if (bgmSource.clip != gameOverBGM)
            {
                bgmSource.clip = gameOverBGM;
                bgmSource.Play();
            }
        }
        else if (sceneName == "EndGame")
        {
            if (bgmSource.clip != endingBGM)
            {
                bgmSource.clip = endingBGM;
                bgmSource.Play();
            }
        }
        else
        {
            if (bgmSource.isPlaying)
                bgmSource.Stop();
        }
    }

    public void BossBGM()
    {
        if (bgmSource != null)
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }

            // Phát nhạc Boss
            if (bossBGM != null)
            {
                bgmSource.clip = bossBGM;
                bgmSource.Play();
            }
        }
    }

    public void UpdateVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = Mathf.Clamp(volume, 0f, 1f); // Apply volume directly to AudioSource
        }
    }
}

