using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public enum CutsceneStepType { Dialogue, Image }

    [System.Serializable]
    public class CutsceneStep
    {
        public CutsceneStepType type;
        public List<string> dialogueLines;
        public Sprite image;
    }

    public List<CutsceneStep> steps;
    public Image cutsceneImage;
    public Image blackScreen;
    public string nextSceneName = "GameScene";
    public float fadeDuration = 1f;

    private int stepIndex = 0;
    private bool isWaitingForInput = false;

    void Start()
    {
        blackScreen.color = Color.black;
        cutsceneImage.enabled = false;
        StartCoroutine(FadeIn());
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

        // Không tăng stepIndex ngay lập tức!
        // Chỉ tăng sau khi bước hiện tại hoàn tất

        if (step.type == CutsceneStepType.Dialogue)
        {
            cutsceneImage.enabled = false;
            DialogueManager.Instance.StartDialogue(step.dialogueLines);
            stepIndex++; // Tăng sau khi gọi thoại
        }
        else if (step.type == CutsceneStepType.Image)
        {
            cutsceneImage.enabled = true;
            cutsceneImage.sprite = step.image;
            isWaitingForInput = true;
            stepIndex++; // Tăng sau khi hiện ảnh
        }
    }


    public void OnDialogueEnded()
    {
        // Chờ 1 frame để tránh skip ngay lập tức do phím còn đang giữ
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
}
