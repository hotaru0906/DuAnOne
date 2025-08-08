using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneStep
    {
        public List<string> dialogueLines;
        public Sprite image;
    }

    public List<CutsceneStep> steps;
    public Image cutsceneImage;
    public Image blackScreen;
    public string nextSceneName = "GameScene";
    public float fadeDuration = 1f;
    public Button skipButton; // Assign this in the Inspector

    private int stepIndex = 0;
    private bool isWaitingForInput = false;

    void Start()
    {
        blackScreen.color = Color.black;
        cutsceneImage.enabled = false;
        StartCoroutine(FadeIn());

        // Add listener to skip button
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCutscene);
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackScreen.color = Color.clear;
        PlayNextStep();
    }

    void Update()
    {
        if (isWaitingForInput && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            isWaitingForInput = false;
            PlayNextStep();
        }
    }

    void PlayNextStep()
    {
        if (stepIndex >= steps.Count)
        {
            StartCoroutine(FadeOutAndLoadScene());
            return;
        }

        CutsceneStep step = steps[stepIndex];
        stepIndex++; // Tăng luôn ở đầu

        // Hiện ảnh nếu có
        if (step.image != null)
        {
            cutsceneImage.enabled = true;
            cutsceneImage.sprite = step.image;
        }
        else
        {
            cutsceneImage.enabled = false;
        }

        // Hiện thoại nếu có
        if (step.dialogueLines != null && step.dialogueLines.Count > 0)
        {
            DialogueManager.Instance.StartDialogue(step.dialogueLines);
            // Chờ DialogueManager gọi lại OnDialogueEnded()
        }
        else
        {
            // Không có thoại → chờ input
            isWaitingForInput = true;
        }
    }
    public void OnDialogueEnded()
    {
        StartCoroutine(DelayPlayNextStep());
    }

    IEnumerator DelayPlayNextStep()
    {
        yield return null; // chờ 1 frame
        PlayNextStep();    // gọi bước tiếp theo (ảnh sẽ hiện ra và chờ input mới)
    }

    IEnumerator FadeOutAndLoadScene()
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

        SceneManager.LoadScene(nextSceneName);
    }

    public void SkipCutscene()
    {
        StopAllCoroutines(); // Stop any ongoing coroutines
        StartCoroutine(FadeOutAndLoadScene()); // Directly fade out and load the next scene
    }
}
