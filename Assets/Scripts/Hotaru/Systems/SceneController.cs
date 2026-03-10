using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static string lastSceneName;
    public string sceneName;
    public Image blackScreen;
    public float fadeDuration = 1f;

    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Nếu chuyển sang death scene thì lưu lại tên scene hiện tại
            if (sceneName == "DeathScene")
            {
                lastSceneName = SceneManager.GetActiveScene().name;
            }
            StartCoroutine(FadeOutAndLoadScene(sceneName));
        }
        else
        {
            Debug.LogWarning("Scene name is not set.");
        }
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (GameManager.Instance != null)
        {
            // Save player stats and gold before fading out

            yield return new WaitForEndOfFrame(); // Ensure save operations complete
        }

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
    public void RestartCurrentScene()
    {
        // Nếu đang ở death scene thì restart lại scene mà player vừa chết
        if (!string.IsNullOrEmpty(lastSceneName))
        {
            StartCoroutine(FadeOutAndLoadScene(lastSceneName));
        }
        else
        {
            // Nếu không có thì restart lại scene hiện tại
            string currentScene = SceneManager.GetActiveScene().name;
            StartCoroutine(FadeOutAndLoadScene(currentScene));
        }
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting.");
    }
}
