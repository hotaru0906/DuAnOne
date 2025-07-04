using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public Image blackScreen;               // Màn hình đen dùng để fade
    public Image cutsceneImage;             // Ảnh cutscene hiển thị
    public Sprite[] cutsceneSprites;        // 4 ảnh PNG
    public float fadeDuration = 1f;         // Thời gian fade
    public string nextSceneName = "GameScene"; // Tên scene cần chuyển tới

    private int currentIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        blackScreen.gameObject.SetActive(true);
        blackScreen.color = new Color(0, 0, 0, 1);  // bắt đầu với màn hình đen
        StartCoroutine(FadeIn());
        ShowImage(0);
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            currentIndex++;

            if (currentIndex < cutsceneSprites.Length)
            {
                ShowImage(currentIndex);
            }
            else
            {
                StartCoroutine(FadeOutAndLoadScene());
            }
        }
    }

    void ShowImage(int index)
    {
        cutsceneImage.sprite = cutsceneSprites[index];
    }

    IEnumerator FadeIn()
    {
        isTransitioning = true;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 0);
        isTransitioning = false;
    }

    IEnumerator FadeOutAndLoadScene()
    {
        isTransitioning = true;
        float t = 0;
        blackScreen.gameObject.SetActive(true);
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.3f); // chờ tí cho đẹp
        SceneManager.LoadScene(nextSceneName);
    }
}
