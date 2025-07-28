using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Import UI namespace

public class SceneController : MonoBehaviour
{
    public string sceneName; // Tên của scene cần chuyển đến
    public Image blackScreen; // Assign this in the Inspector
    public float fadeDuration = 1f;

    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(FadeOutAndLoadScene(sceneName));
        }
        else
        {
            Debug.LogWarning("Scene name is not set.");
        }
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        float t = 0;
        blackScreen.gameObject.SetActive(true);
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting.");
    }
}
